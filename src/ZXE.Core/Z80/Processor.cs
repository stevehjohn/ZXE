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
            _tracer.TraceBefore(instruction, data, _state, ram);
        }

        instruction.Action(new Input(data, _state, ram));

        _state.ProgramCounter += instruction.Length;

        if (_tracer != null)
        {
            _tracer.TraceAfter(instruction, data, _state, ram);
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

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => LD_addr_RR_R(i, Register.BC, Register.A), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_RR(i, Register.BC), 6);

        instructions[0x04] = new Instruction("INC B", 1, i => INC_R(i, Register.B), 4);

        instructions[0x05] = new Instruction("DEC B", 1, i => DEC_R(i, Register.B), 4);

        instructions[0x06] = new Instruction("LD B, n", 2, i => LD_R_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, RLCA, 4, "RLCA A");

        instructions[0x08] = new Instruction("EX AF, AF'", 1, i => EX_RR_RaRa(i, Register.A, Register.F), 4);

        instructions[0x09] = new Instruction("ADD HL, BC", 1, i => ADD_RR_RR(i, Register.HL, Register.BC), 11);

        instructions[0x0A] = new Instruction("LD A, (BC)'", 1, i => LD_R_addr_RR(i, Register.A, Register.BC), 7);

        instructions[0x0B] = new Instruction("DEC BC", 1, i => DEC_RR(i, Register.BC), 6);

        instructions[0x0C] = new Instruction("INC C", 1, i => INC_R(i, Register.C), 4);

        instructions[0x0D] = new Instruction("DEC C", 1, i => DEC_R(i, Register.C), 4);

        instructions[0x0E] = new Instruction("LD C, n", 2, i => LD_R_n(i, Register.C), 7);

        instructions[0x0F] = new Instruction("RRCA", 2, RRCA, 4, "RRCA A");

        instructions[0x10] = new Instruction("DJNZ e", 2, DJNZ_e, 8, "DJNZ B, e");

        instructions[0x11] = new Instruction("LD DE, nn", 3, i => LD_RR_nn(i, Register.DE), 10);

        instructions[0x12] = new Instruction("LD (DE), A", 3, i => LD_addr_RR_R(i, Register.DE, Register.A), 7);

        instructions[0x13] = new Instruction("INC DE", 3, i => INC_RR(i, Register.DE), 6);

        instructions[0x14] = new Instruction("INC D", 3, i => INC_R(i, Register.D), 4);

        instructions[0x15] = new Instruction("DEC D", 3, i => DEC_R(i, Register.D), 4);

        instructions[0x16] = new Instruction("LD D, n", 3, i => LD_R_n(i, Register.D), 7);

        instructions[0x17] = new Instruction("RLA", 1, RLA, 4, "RLA A");

        instructions[0x18] = new Instruction("JR e", 2, JR_e, 12);

        instructions[0x19] = new Instruction("ADD HL, DE", 1, i => ADD_RR_RR(i, Register.HL, Register.DE), 11);

        instructions[0x1A] = new Instruction("LD A, (DE)", 1, i => LD_R_addr_RR(i, Register.A, Register.DE), 7);

        instructions[0x1B] = new Instruction("DEC DE", 1, i => DEC_RR(i, Register.DE), 6);

        instructions[0x1C] = new Instruction("INC E", 1, i => INC_R(i, Register.E), 4);

        instructions[0x1D] = new Instruction("DEC E", 1, i => DEC_R(i, Register.E), 4);

        instructions[0x1E] = new Instruction("LD E, n", 1, i => LD_R_n(i, Register.E), 7);

        instructions[0x1F] = new Instruction("RRA", 1, RRA, 4, "RRA A");

        instructions[0x20] = new Instruction("JR NZ, e", 2, JR_NZ_e, 7);

        instructions[0x21] = new Instruction("LD HL, nn", 3, i => LD_RR_nn(i, Register.HL), 10);

        instructions[0x22] = new Instruction("LD (nn), HL", 3, i => LD_addr_nn_RR(i, Register.HL), 16);

        instructions[0x23] = new Instruction("INC HL", 1, i => INC_RR(i, Register.HL), 6);

        instructions[0x24] = new Instruction("INC H", 1, i => INC_R(i, Register.H), 4);

        instructions[0x25] = new Instruction("DEC H", 1, i => DEC_R(i, Register.H), 4);

        instructions[0x26] = new Instruction("LD H, n", 2, i => LD_R_n(i, Register.H), 7);

        instructions[0x27] = new Instruction("DAA", 1, DAA, 4);

        instructions[0x28] = new Instruction("JR Z, e", 2, JR_Z_e, 7);

        instructions[0x29] = new Instruction("ADD HL, HL", 2, i => ADD_RR_RR(i, Register.HL, Register.HL), 11);

        instructions[0x2A] = new Instruction("LD HL, (nn)", 3, i => LD_RR_addr_nn(i, Register.HL), 16);

        instructions[0x2B] = new Instruction("DEC HL", 1, i => DEC_RR(i, Register.HL), 6);

        instructions[0x2C] = new Instruction("INC L", 1, i => INC_R(i, Register.L), 4);

        instructions[0x2D] = new Instruction("DEC L", 1, i => DEC_R(i, Register.L), 4);

        instructions[0x2E] = new Instruction("LD L, n", 2, i => LD_R_n(i, Register.L), 7);

        instructions[0x2F] = new Instruction("CPL", 1, CPL, 4);

        instructions[0x30] = new Instruction("JR NC, e", 2, JR_NC_e, 7);

        instructions[0x31] = new Instruction("LD SP, nn", 3, LD_SP_nn, 10);

        instructions[0x32] = new Instruction("LD (nn), A", 3, i => LD_addr_nn_R(i, Register.A), 13);

        instructions[0x33] = new Instruction("INC SP", 1, INC_SP, 6);

        instructions[0x34] = new Instruction("INC (HL)", 1, i => INC_addr_RR(i, Register.HL), 11);

        instructions[0x35] = new Instruction("DEC (HL)", 1, i => DEC_addr_RR(i, Register.HL), 11);

        instructions[0x36] = new Instruction("LD (HL), n", 2, i => LD_addr_RR_n(i, Register.HL), 10);

        instructions[0x37] = new Instruction("SCF", 1, SCF, 4);

        instructions[0x38] = new Instruction("JR C, e", 2, JR_C_e, 7);

        instructions[0x39] = new Instruction("ADD HL, SP", 1, i => ADD_RR_SP(i, Register.HL), 11);

        instructions[0x3A] = new Instruction("LD A, (nn)", 3, i => LD_R_addr_nn(i, Register.A), 13);

        instructions[0x3B] = new Instruction("DEC SP", 1, DEC_SP, 6);

        instructions[0x3C] = new Instruction("INC A", 1, i => INC_R(i, Register.A), 4);

        instructions[0x3D] = new Instruction("DEC A", 1, i => DEC_R(i, Register.A), 4);

        instructions[0x3E] = new Instruction("LD A, n", 2, i => LD_R_n(i, Register.A), 7);

        instructions[0x3F] = new Instruction("CCF", 1, CCF, 4);

        instructions[0x40] = new Instruction("LD B, B", 1, i => LD_R_R(i, Register.B, Register.B), 4);

        instructions[0x41] = new Instruction("LD B, C", 1, i => LD_R_R(i, Register.B, Register.C), 4);

        instructions[0x42] = new Instruction("LD B, D", 1, i => LD_R_R(i, Register.B, Register.D), 4);
        
        instructions[0x43] = new Instruction("LD B, E", 1, i => LD_R_R(i, Register.B, Register.E), 4);

        instructions[0x44] = new Instruction("LD B, H", 1, i => LD_R_R(i, Register.B, Register.H), 4);

        instructions[0x45] = new Instruction("LD B, L", 1, i => LD_R_R(i, Register.B, Register.L), 4);
        
        instructions[0x46] = new Instruction("LD B, (HL)", 1, i => LD_R_addr_nn(i, Register.B), 7);

        instructions[0x47] = new Instruction("LD B, A", 1, i => LD_R_R(i, Register.B, Register.A), 4);

        instructions[0x48] = new Instruction("LD C, B", 1, i => LD_R_R(i, Register.C, Register.B), 4);

        instructions[0x49] = new Instruction("LD C, C", 1, i => LD_R_R(i, Register.C, Register.C), 4);

        instructions[0x4A] = new Instruction("LD C, D", 1, i => LD_R_R(i, Register.C, Register.D), 4);
        
        instructions[0x4B] = new Instruction("LD C, E", 1, i => LD_R_R(i, Register.C, Register.E), 4);

        instructions[0x4C] = new Instruction("LD C, H", 1, i => LD_R_R(i, Register.C, Register.H), 4);

        instructions[0x4D] = new Instruction("LD C, L", 1, i => LD_R_R(i, Register.C, Register.L), 4);
        
        instructions[0x4E] = new Instruction("LD C, (HL)", 1, i => LD_R_addr_nn(i, Register.C), 7);

        instructions[0x4F] = new Instruction("LD C, A", 1, i => LD_R_R(i, Register.C, Register.A), 4);

        instructions[0x50] = new Instruction("LD D, B", 1, i => LD_R_R(i, Register.D, Register.B), 4);

        instructions[0x51] = new Instruction("LD D, C", 1, i => LD_R_R(i, Register.D, Register.C), 4);

        instructions[0x52] = new Instruction("LD D, D", 1, i => LD_R_R(i, Register.D, Register.D), 4);
        
        instructions[0x53] = new Instruction("LD D, E", 1, i => LD_R_R(i, Register.D, Register.E), 4);

        instructions[0x54] = new Instruction("LD D, H", 1, i => LD_R_R(i, Register.D, Register.H), 4);

        instructions[0x55] = new Instruction("LD D, L", 1, i => LD_R_R(i, Register.D, Register.L), 4);
        
        instructions[0x56] = new Instruction("LD D, (HL)", 1, i => LD_R_addr_nn(i, Register.D), 7);

        instructions[0x57] = new Instruction("LD D, A", 1, i => LD_R_R(i, Register.D, Register.A), 4);

        instructions[0x58] = new Instruction("LD E, B", 1, i => LD_R_R(i, Register.E, Register.B), 4);

        instructions[0x59] = new Instruction("LD E, C", 1, i => LD_R_R(i, Register.E, Register.C), 4);

        instructions[0x5A] = new Instruction("LD E, D", 1, i => LD_R_R(i, Register.E, Register.D), 4);
        
        instructions[0x5B] = new Instruction("LD E, E", 1, i => LD_R_R(i, Register.E, Register.E), 4);

        instructions[0x5C] = new Instruction("LD E, H", 1, i => LD_R_R(i, Register.E, Register.H), 4);

        instructions[0x5D] = new Instruction("LD E, L", 1, i => LD_R_R(i, Register.E, Register.L), 4);
        
        instructions[0x5E] = new Instruction("LD E, (HL)", 1, i => LD_R_addr_nn(i, Register.E), 7);

        instructions[0x5F] = new Instruction("LD E, A", 1, i => LD_R_R(i, Register.E, Register.A), 4);

        instructions[0x60] = new Instruction("LD H, B", 1, i => LD_R_R(i, Register.H, Register.B), 4);

        instructions[0x61] = new Instruction("LD H, C", 1, i => LD_R_R(i, Register.H, Register.C), 4);

        instructions[0x62] = new Instruction("LD H, D", 1, i => LD_R_R(i, Register.H, Register.D), 4);
        
        instructions[0x63] = new Instruction("LD H, E", 1, i => LD_R_R(i, Register.H, Register.E), 4);

        instructions[0x64] = new Instruction("LD H, H", 1, i => LD_R_R(i, Register.H, Register.H), 4);

        instructions[0x65] = new Instruction("LD H, L", 1, i => LD_R_R(i, Register.H, Register.L), 4);
        
        instructions[0x66] = new Instruction("LD H, (HL)", 1, i => LD_R_addr_nn(i, Register.H), 7);

        instructions[0x67] = new Instruction("LD H, A", 1, i => LD_R_R(i, Register.H, Register.A), 4);

        instructions[0x68] = new Instruction("LD L, B", 1, i => LD_R_R(i, Register.L, Register.B), 4);

        instructions[0x69] = new Instruction("LD L, C", 1, i => LD_R_R(i, Register.L, Register.C), 4);

        instructions[0x6A] = new Instruction("LD L, D", 1, i => LD_R_R(i, Register.L, Register.D), 4);
        
        instructions[0x6B] = new Instruction("LD L, E", 1, i => LD_R_R(i, Register.L, Register.E), 4);

        instructions[0x6C] = new Instruction("LD L, H", 1, i => LD_R_R(i, Register.L, Register.H), 4);

        instructions[0x6D] = new Instruction("LD L, L", 1, i => LD_R_R(i, Register.L, Register.L), 4);
        
        instructions[0x6E] = new Instruction("LD L, (HL)", 1, i => LD_R_addr_nn(i, Register.L), 7);

        instructions[0x6F] = new Instruction("LD L, A", 1, i => LD_R_R(i, Register.L, Register.A), 4);

        instructions[0x70] = new Instruction("LD (HL), B", 1, i => LD_addr_RR_R(i, Register.HL, Register.B), 7);

        instructions[0x71] = new Instruction("LD (HL), C", 1, i => LD_addr_RR_R(i, Register.HL, Register.C), 7);

        instructions[0x72] = new Instruction("LD (HL), D", 1, i => LD_addr_RR_R(i, Register.HL, Register.D), 7);

        instructions[0x73] = new Instruction("LD (HL), E", 1, i => LD_addr_RR_R(i, Register.HL, Register.E), 7);

        instructions[0x74] = new Instruction("LD (HL), H", 1, i => LD_addr_RR_R(i, Register.HL, Register.H), 7);

        instructions[0x75] = new Instruction("LD (HL), L", 1, i => LD_addr_RR_R(i, Register.HL, Register.L), 7);

        instructions[0x76] = new Instruction("HALT", 1, HALT, 4);

        instructions[0x77] = new Instruction("LD (HL), A", 1, i => LD_addr_RR_R(i, Register.HL, Register.A), 7);

        instructions[0x78] = new Instruction("LD A, B", 1, i => LD_R_R(i, Register.A, Register.B), 4);

        instructions[0x79] = new Instruction("LD A, C", 1, i => LD_R_R(i, Register.A, Register.C), 4);

        instructions[0x7A] = new Instruction("LD A, D", 1, i => LD_R_R(i, Register.A, Register.D), 4);

        instructions[0x7B] = new Instruction("LD A, E", 1, i => LD_R_R(i, Register.A, Register.E), 4);

        instructions[0x7C] = new Instruction("LD A, H", 1, i => LD_R_R(i, Register.A, Register.H), 4);

        instructions[0x7D] = new Instruction("LD A, L", 1, i => LD_R_R(i, Register.A, Register.L), 4);

        instructions[0x77] = new Instruction("LD A, (HL)", 1, i => LD_addr_RR_R(i, Register.HL, Register.A), 7);
        
        instructions[0x7F] = new Instruction("LD A, A", 1, i => LD_R_R(i, Register.A, Register.A), 4);

        instructions[0x80] = new Instruction("ADD A, B", 1, i => ADD_R_R(i, Register.A, Register.B), 4);

        instructions[0x81] = new Instruction("ADD A, C", 1, i => ADD_R_R(i, Register.A, Register.C), 4);

        instructions[0x82] = new Instruction("ADD A, D", 1, i => ADD_R_R(i, Register.A, Register.D), 4);

        instructions[0x83] = new Instruction("ADD A, E", 1, i => ADD_R_R(i, Register.A, Register.E), 4);

        instructions[0x84] = new Instruction("ADD A, H", 1, i => ADD_R_R(i, Register.A, Register.H), 4);

        instructions[0x85] = new Instruction("ADD A, L", 1, i => ADD_R_R(i, Register.A, Register.L), 4);

        instructions[0x86] = new Instruction("ADD A, (HL)", 1, i => ADD_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x87] = new Instruction("ADD A, A", 1, i => ADD_R_R(i, Register.A, Register.A), 4);
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

    private static void LD_addr_RR_R(Input input, Register target, Register source)
    {
        input.Ram[input.State.Registers.ReadPair(target)] = input.State.Registers[source];

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
        var alternate1 = Enum.Parse<Register>($"{register1}1");

        var alternate2 = Enum.Parse<Register>($"{register2}1");

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

        var result = (byte) (0xFF & (input.State.Registers[Register.A] >> 1));

        if (bottomBit == 1)
        {
            result |= 0x80;
        }

        input.State.Registers[Register.A] = result;

        // Flags
        input.State.Flags.Carry = bottomBit == 1;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void DJNZ_e(Input input)
    {
        // TODO: If B != 0, 5 more cycles... how to do this?
        DEC_R(input, Register.B);

        // TODO: Compensate for twice-incremented program counter?
        if (! input.State.Flags.Zero)
        {
            input.State.ProgramCounter += (sbyte) input.Data[1];
        }

        // Flags unaffected
    }

    private static void RLA(Input input)
    {
        var topBit = (input.State.Registers[Register.A] & 0x80) >> 7;

        var result = (byte) (input.State.Registers[Register.A] << 1);

        result |= (byte) (input.State.Flags.Carry ? 1 : 0);

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

    public static void JR_e(Input input)
    {
        // TODO: Compensate for twice-incremented program counter?
        input.State.ProgramCounter += (sbyte) input.Data[1];
    }

    private static void RRA(Input input)
    {
        var bottomBit = input.State.Registers[Register.A] & 0x01;

        var result = (byte) (input.State.Registers[Register.A] >> 1);

        result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

        input.State.Registers[Register.A] = result;

        // Flags
        input.State.Flags.Carry = bottomBit == 1;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void JR_NZ_e(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            // TODO: If Z == false, 5 more cycles... how to do this?

            input.State.ProgramCounter += (sbyte) input.Data[1];
        }

        // Flags unaffected
    }

    private static void LD_addr_nn_RR(Input input, Register register)
    {
        var address = input.Data[2] << 8 | input.Data[1];

        var data = input.State.Registers.ReadPair(register);

        input.Ram[address] = (byte) (data & 0x00FF);
        input.Ram[address + 1] = (byte) ((data & 0xFF00) >> 8);

        // Flags unaffected
    }

    private static void DAA(Input i)
    {
        // TODO: Wooooah.
        // TODO: DO!
    }

    private static void JR_Z_e(Input input)
    {
        if (input.State.Flags.Zero)
        {
            // TODO: If Z == true, 5 more cycles... how to do this?

            input.State.ProgramCounter += (sbyte) input.Data[1];
        }

        // Flags unaffected
    }

    private static void LD_RR_addr_nn(Input input, Register register)
    {
        var address = input.Data[2] << 8 | input.Data[1];

        input.State.Registers.WritePair(register, (ushort) (input.Ram[address + 1] << 8 | input.Ram[address]));

        input.State.Registers[Register.L] = input.Ram[address];
        input.State.Registers[Register.H] = input.Ram[address + 1];

        // Flags unaffected
    }

    private static void CPL(Input input)
    {
        var result = input.State.Registers[Register.A] ^ 0xFF;

        input.State.Registers[Register.A] = (byte) result;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = true;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void JR_NC_e(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            // TODO: If C == false, 5 more cycles... how to do this?

            input.State.ProgramCounter += (sbyte) input.Data[1];
        }

        // Flags unaffected
    }

    private static void LD_SP_nn(Input input)
    {
        input.State.StackPointer = input.Data[2] << 8 | input.Data[1];

        // Flags unaffected
    }

    private static void INC_SP(Input input)
    {
        input.State.StackPointer++;

        // Flags unaffected
    }

    private static void INC_addr_RR(Input input, Register register)
    {
        var address = input.State.Registers.ReadPair(register);

        var value = input.Ram[address];

        var result = ++input.Ram[address];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value == 0x7F;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0x0F;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void DEC_addr_RR(Input input, Register register)
    {
        var address = input.State.Registers.ReadPair(register);

        var value = input.Ram[address];

        var result = --input.Ram[address];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = value == 0x80;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) < 1;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
    }

    private static void LD_addr_RR_n(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.Data[1];

        // Flags unaffected
    }

    private static void SCF(Input input)
    {
        input.State.Flags.Carry = true;

        // TODO: XOR with Q register?
        var xFlags = input.State.Flags.ToByte() | input.State.Registers[Register.A];

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (xFlags & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (xFlags & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected
    }

    private static void JR_C_e(Input input)
    {
        if (input.State.Flags.Carry)
        {
            // TODO: If C == false, 5 more cycles... how to do this?

            input.State.ProgramCounter += (sbyte) input.Data[1];
        }

        // Flags unaffected
    }

    private static void ADD_RR_SP(Input input, Register register)
    {
        var source = input.State.StackPointer;

        var result = source + input.State.Registers.ReadPair(register);

        input.State.Registers.WritePair(register, (ushort) result);

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

    private static void DEC_SP(Input input)
    {
        input.State.StackPointer--;

        // Flags unaffected
    }

    private static void CCF(Input input)
    {
        var value = input.State.Flags.Carry;

        input.State.Flags.Carry = ! input.State.Flags.Carry;
        
        // TODO: XOR with Q register?
        var xFlags = input.State.Flags.ToByte() | input.State.Registers[Register.A];

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (xFlags & 0x08) > 0;
        input.State.Flags.HalfCarry = value;
        input.State.Flags.X2 = (xFlags & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected
    }

    private static void LD_R_R(Input input, Register destination, Register source)
    {
        input.State.Registers[destination] = input.State.Registers[source];

        // Flags unaffected
    }

    private static void HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected
    }

    public static void ADD_R_R(Input input, Register destination, Register source)
    {
        var valueD = input.State.Registers[destination];
        
        var valueS = input.State.Registers[source];

        var result = valueD + valueS;

        input.State.Registers[destination] = (byte) result;

        // Flags
        input.State.Flags.Carry = result > 0xFF;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result > 0xFF;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }

    private static void ADD_R_addr_RR(Input input, Register destination, Register source)
    {
        var valueD = input.State.Registers[destination];

        var valueS = input.Ram[input.State.Registers.ReadPair(source)];

        var result = valueD + valueS;

        input.State.Registers[destination] = (byte) result;

        // Flags
        input.State.Flags.Carry = result > 0xFF;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result > 0xFF;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }
}