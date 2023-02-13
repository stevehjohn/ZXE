using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;

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

    public void ProcessInstruction(Ram ram, Ports ports)
    {
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

        if (instruction.Action(new Input(data, _state, ram, ports)))
        {
            _state.ProgramCounter += instruction.Length;
        }

        if (_state.ProgramCounter > 0xFFFF)
        {
            _state.ProgramCounter -= 0x10000;
        }

        if (_tracer != null)
        {
            _tracer.TraceAfter(instruction, data, _state, ram);
        }
    }

    internal void SetState(State state)
    {
        _state = state;
    }
    
    private bool SetOpcodePrefix(int prefix)
    {
        _state.OpcodePrefix = prefix;

        return true;
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