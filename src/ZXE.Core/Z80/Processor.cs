﻿using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.System.Interfaces;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;

public partial class Processor
{
    private State _state;

    private readonly Instruction?[] _instructions;

    private readonly ITracer? _tracer;

    // TODO: Remove - not good.
    public Instruction?[] Instructions => _instructions;

    public State State => _state;
    
    public IProcessorExtension? ProcessorExtension { get; set; }

    public Processor()
    {
        _state = new State();

        _instructions = InitialiseInstructions();
    }

    public Processor(ITracer tracer)
    {
        _state = new State();

        _instructions = InitialiseInstructions();

        _tracer = tracer;
    }

    public (int Cycles, string Mnemonic) ProcessInstruction(Ram ram, Ports ports, Bus bus)
    {
        if (_state.Halted)
        {
            HandleInterrupts(ram, bus);

            return (_instructions[0x00]!.Action(new Input(Array.Empty<byte>(), _state, ram, ports)), "NOP");
        }

        var opcode = (int) ram[_state.ProgramCounter];

        if (_state.OpcodePrefix != 0 && _state.OpcodePrefix <= 0xFF)
        {
            opcode = _state.OpcodePrefix << 8 | opcode;

            _state.OpcodePrefix = 0;
        }

        if (opcode >= _instructions.Length)
        {
            throw new OpcodeNotImplementedException($"Opcode not implemented: {opcode:X6}.");
        }

        Instruction? instruction;

        byte[]? data;

        if (_state.OpcodePrefix > 0xFF)
        {
            data = ram.GetData(_state.ProgramCounter, 2);

            instruction = _instructions[(_state.OpcodePrefix << 8) | data[1]];

            _state.OpcodePrefix = 0;

            if (instruction == null)
            {
                throw new OpcodeNotImplementedException($"Opcode not implemented: {opcode:X6}.");
            }
        }
        else
        {
            instruction = _instructions[opcode];

            if (instruction == null)
            {
                throw new OpcodeNotImplementedException($"Opcode not implemented: {opcode:X6}.");
            }

            data = ram.GetData(_state.ProgramCounter, instruction.Length);
        }

        if (_tracer != null)
        {
            _tracer.TraceBefore(instruction, data, _state, ram);
        }

        UpdateR(instruction);

        var additionalCycles = instruction.Action(new Input(data, _state, ram, ports));

        if (additionalCycles > -1)
        {
            _state.ProgramCounter += instruction.Length;
        }

        if (ProcessorExtension != null)
        {
            ProcessorExtension.InstructionProcessed(_state, ram);
        }

        if (! instruction.Mnemonic.StartsWith("SOPSET") && _state.OpcodePrefix == 0)
        {
            HandleInterrupts(ram, bus);
        }

        if (_tracer != null)
        {
            _tracer.TraceAfter(instruction, data, _state, ram);
        }

        return (instruction.ClockCycles + (additionalCycles > -1 ? additionalCycles : 0), instruction.Mnemonic);
    }

    public void Reset(int programCounter = 0x0000)
    {
        _state.Registers[Register.A] = _state.Registers[Register.A_] = 0xFF;

        _state.Registers[Register.F] = _state.Registers[Register.F_] = 0xFF;

        _state.Flags = Flags.FromByte(0xFF);

        _state.Registers[Register.I] = _state.Registers[Register.R] = 0x00;

        _state.ProgramCounter = programCounter;

        _state.StackPointer = 0xFFFF;

        _state.InterruptMode = InterruptMode.Mode0;

        _state.Halted = false;

        _state.Registers[Register.B] = _state.Registers[Register.C] = _state.Registers[Register.D] = _state.Registers[Register.E] = _state.Registers[Register.H] = _state.Registers[Register.L] = 0x00;

        _state.Registers[Register.B_] = _state.Registers[Register.C_] = _state.Registers[Register.D_] = _state.Registers[Register.E_] = _state.Registers[Register.H_] = _state.Registers[Register.L_] = 0x00;

        _state.Registers.WritePair(Register.IX, 0x00);

        _state.Registers.WritePair(Register.IY, 0x00);

        _state.InterruptFlipFlop1 = false;

        _state.InterruptFlipFlop2 = false;

        _state.OpcodePrefix = 0;

        _state.MemPtr = 0;

        _state.Q = 0;
    }
    
    public void HandleInterrupts(Ram ram, Bus bus)
    {
        if (bus.NonMaskableInterrupt)
        {
            HandleNonMaskableInterrupt(ram);

            bus.NonMaskableInterrupt = false;
        }

        if (bus.Interrupt && _state.InterruptFlipFlop1)
        {
            HandleMaskableInterrupt(ram, bus);

            bus.Interrupt = false;
        }
    }

    internal void SetState(State state)
    {
        _state = state;
    }
    
    private int SetOpcodePrefix(int prefix)
    {
        _state.OpcodePrefix = prefix;

        return 0;
    }

    private void HandleNonMaskableInterrupt(Ram ram)
    {
        _state.Halted = false;

        PushProgramCounter(ram);

        _state.InterruptFlipFlop2 = _state.InterruptFlipFlop1;

        _state.InterruptFlipFlop1 = false;

        _state.ProgramCounter = 0x0066;
    }

    private void HandleMaskableInterrupt(Ram ram, Bus bus)
    {
        _state.Halted = false;

        if (_state.InterruptFlipFlop1)
        {
            _state.InterruptFlipFlop1 = _state.InterruptFlipFlop2 = false;

            int address;

            switch (_state.InterruptMode)
            {
                case InterruptMode.Mode0:
                    var instructionOpcode = bus.Data!;

                    var instruction = _instructions[(int) instructionOpcode];

                    if (instruction!.Mnemonic.StartsWith("RST"))
                    {
                        PushProgramCounter(ram);

                        address = (int) instructionOpcode & 0x38;

                        _state.ProgramCounter = address;

                        _state.ResetQ();
                    }
                    else if (instruction.Mnemonic.StartsWith("CALL"))
                    {
                        PushProgramCounter(ram);

                        address = ram[_state.ProgramCounter + 2] << 8;

                        address |= ram[_state.ProgramCounter + 1];

                        _state.ProgramCounter = address;

                        _state.MemPtr = (ushort) _state.ProgramCounter;

                        _state.ResetQ();
                    }

                    break;

                case InterruptMode.Mode1:
                    PushProgramCounter(ram);

                    _state.ProgramCounter = 0x0038;

                    break;

                case InterruptMode.Mode2:
                    if (bus.Data != null)
                    {
                        PushProgramCounter(ram);

                        address = (_state.Registers[Register.I] << 8) + bus.Data.Value;

                        bus.Data = null;

                        _state.ProgramCounter = ram[address] | (ram[address + 1] << 8);
                    }

                    break;
            }
        }
    }

    private void PushProgramCounter(Ram ram)
    {
        _state.StackPointer--;

        ram[_state.StackPointer] = (byte) ((_state.ProgramCounter & 0xFF00) >> 8);

        _state.StackPointer--;

        ram[_state.StackPointer] = (byte) (_state.ProgramCounter & 0x00FF);
    }

    private void UpdateR(Instruction instruction)
    {
        if (instruction.Mnemonic.StartsWith("SOPSET"))
        {
            return;
        }

        var increment = 1;

        if (instruction.Opcode > 0xFF)
        {
            increment = 2;
        }

        var value = (byte) (_state.Registers[Register.R] & 0x7F);

        var topBit = _state.Registers[Register.R] & 0x80;

        value = (byte) (value + increment);

        _state.Registers[Register.R] = value;

        if (topBit > 0)
        {
            _state.Registers[Register.R] |= 0x80;
        }
        else
        {
            _state.Registers[Register.R] &= 0x7F;
        }
    }

    private Instruction[] InitialiseInstructions()
    {
        var instructions = new Dictionary<int, Instruction>();

        InitialiseBaseInstructions(instructions);
        
        InitialiseCBInstructions(instructions);

        InitialiseDDInstructions(instructions);
        
        InitialiseEDInstructions(instructions);
        
        InitialiseFDInstructions(instructions);

        InitialiseDDCBInstructions(instructions);

        InitialiseFDCBInstructions(instructions);

        var instructionArray = new Instruction[instructions.Max(i => i.Key) + 1];

        foreach (var instruction in instructions)
        {
            if (instruction.Value.Opcode == null)
            {
                instruction.Value.Opcode = instruction.Key;
            }

            instructionArray[instruction.Key] = instruction.Value;
        }

        return instructionArray;
    }
}