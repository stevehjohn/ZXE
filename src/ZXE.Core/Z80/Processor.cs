﻿using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;

public class Processor
{
    private State _state;

    private readonly Instruction[] _instructions;

    private readonly ITracer? _tracer;

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

    public string ProcessInstruction(Ram ram)
    {
        var instruction = _instructions[ram[_state.ProgramCounter]];

        var data = ram[_state.ProgramCounter..(_state.ProgramCounter + instruction.Length)];

        if (_tracer != null)
        {
            _tracer.TraceBefore(instruction.Mnemonic, data, _state, ram);
        }

        instruction.Action(new Input(data, _state, ram));

        _state.ProgramCounter += instruction.Length;

        if (_tracer != null)
        {
            _tracer.TraceAfter(instruction.Mnemonic, data, _state, ram);
        }

        return string.Empty;
    }

    internal void SetState(State state)
    {
        _state = state;
    }

    private Instruction[] InitialiseInstructions()
    {
        var instructions = new Dictionary<int, Instruction>();

        InitialiseInstructions(instructions);

        var instructionArray = new Instruction[instructions.Max(i => i.Key) + 1];

        foreach (var instruction in instructions)
        {
            instructionArray[instruction.Key] = instruction.Value;
        }

        return instructionArray;
    }

    private static void InitialiseInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0x00] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => LD_RR_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => LD_addr_RR_A(i, Register.BC), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_RR(i, Register.BC), 6);
        
        instructions[0x04] = new Instruction("INC B", 1, i => INC_R(i, Register.B), 4);
        
        instructions[0x05] = new Instruction("DEC B", 1, i => DEC_R(i, Register.B), 4);
        
        instructions[0x06] = new Instruction("LD B, n", 2, i => LD_R_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, RLCA, 4);

        instructions[0x08] = new Instruction("EX AF, AF'", 1, i => EX_RR_RaRa(i, Register.A, Register.F), 4);

        instructions[0x09] = new Instruction("ADD HL, BC'", 1, i => ADD_RR_RR(i, Register.HL, Register.BC), 11);

        instructions[0x0A] = new Instruction("LD A, (BC)'", 1, i => LD_R_addr_RR(i, Register.A, Register.BC), 7);

        instructions[0x0B] = new Instruction("DEC BC", 1, i => DEC_RR(i, Register.BC), 6);

        instructions[0x0C] = new Instruction("INC C", 1, i => INC_R(i, Register.C), 4);
        
        instructions[0x0D] = new Instruction("DEC C", 1, i => DEC_R(i, Register.C), 4);

        instructions[0x0E] = new Instruction("LD C, n", 2, i => LD_R_n(i, Register.C), 7);

        instructions[0x0F] = new Instruction("RRCA", 2, RRCA, 4);

        instructions[0x32] = new Instruction("LD (nn), A", 3, i => LD_addr_nn_R(i, Register.A), 13);

        instructions[0x3A] = new Instruction("LD A, (nn)", 3, i => LD_R_addr_nn(i, Register.A), 13);

        instructions[0x3E] = new Instruction("LD A, n", 2, i => LD_R_n(i, Register.A), 7);

        instructions[0x76] = new Instruction("HALT", 1, HALT, 4);
    }

    private static void NOP()
    {
        // Flags unaffected
    }

    private static void LD_RR_nn(Input input, Register register)
    {
        input.State.Registers.LoadFromRam(register, input.Data[1..3]);

        // Flags unaffected
    }

    private static void LD_addr_RR_A(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.State.Registers[Register.A];

        // Flags unaffected
    }

    private static void INC_RR(Input input, Register register)
    {
        input.State.Registers.WritePair(register, (ushort) (input.State.Registers.ReadPair(register) + 1));

        // Flags unaffected
    }

    private static void INC_R(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value + 1);

        input.State.Registers[register] = result;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value == 0x7F;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void DEC_R(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value - 1);

        input.State.Registers[register] = result;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = value == 0x80;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) < 1;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void LD_R_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];

        // Flags unaffected
    }

    private static void RLCA(Input input)
    {
        var topBit = (byte) ((input.State.Registers[Register.A] & 0x80) >> 7);

        var result = (byte) (((input.State.Registers[Register.A] << 1) & 0xFE) | topBit);

        input.State.Registers[Register.A] = result;

        // Flags
        input.State.Flags.Carry = topBit == 1;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void LD_addr_nn_R(Input input, Register register)
    {
        input.Ram[(input.Data[2] << 8) | input.Data[1]] = input.State.Registers[register];

        // Flags unaffected
    }

    private static void LD_R_addr_nn(Input input, Register register)
    {
        input.State.Registers[register] = input.Ram[(input.Data[2] << 8) | input.Data[1]];

        // Flags unaffected
    }

    private static void EX_RR_RaRa(Input input, Register register1, Register register2)
    {
        var alternate1 = Enum.Parse<Register>($"{register1}a");

        var alternate2 = Enum.Parse<Register>($"{register2}a");

        (input.State.Registers[register1], input.State.Registers[alternate1]) = (input.State.Registers[alternate1], input.State.Registers[register1]);

        (input.State.Registers[register2], input.State.Registers[alternate2]) = (input.State.Registers[alternate2], input.State.Registers[register2]);

        // Flags unaffected
    }

    private static void ADD_RR_RR(Input input, Register target, Register operand)
    {
        var source = (int) input.State.Registers.ReadPair(target);

        var result = source + input.State.Registers.ReadPair(operand);

        input.State.Registers.WritePair(target, (ushort) result);

        // Flags
        input.State.Flags.Carry = result > 0xFFFF;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (source & 0x0800) > 0 && (result & 0x1000) > 0;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    public static void LD_R_addr_RR(Input input, Register target, Register source)
    {
        input.State.Registers[target] = input.Ram[input.State.Registers.ReadPair(source)];

        // Flags unaffected
    }

    private static void DEC_RR(Input input, Register register)
    {
        var result = input.State.Registers.ReadPair(register);

        result--;

        input.State.Registers.WritePair(register, result);

        // Flags unaffected
    }

    private static void RRCA(Input input)
    {
        var bottomBit = input.State.Registers[Register.A] & 0x01;

        var result = (byte) (0x0F & (input.State.Registers[Register.A] >> 1));

        input.State.Registers[Register.A] = result;

        // FLAGS
        input.State.Flags.Carry = bottomBit > 0;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected
    }

    private static void HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected
    }
}