﻿using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;

public class Processor
{
    private State _state;

    private readonly Instruction?[] _instructions;

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

    public void ProcessInstruction(Ram ram)
    {
        var opcode = (int) ram[_state.ProgramCounter];

        if (_state.OpcodePrefix != 0)
        {
            opcode = _state.OpcodePrefix << 8 | opcode;

            _state.OpcodePrefix = 0;
        }

        if (opcode >= _instructions.Length)
        {
            throw new OpcodeNotImplementedException($"Opcode not implemented: {opcode:X6}.");
        }

        var instruction = _instructions[opcode];

        if (instruction == null)
        {
            throw new OpcodeNotImplementedException($"Opcode not implemented: {opcode:X6}.");
        }

        var data = ram.GetData(_state.ProgramCounter, instruction.Length);

        if (_tracer != null)
        {
            _tracer.TraceBefore(instruction, data, _state, ram);
        }

        if (instruction.Action(new Input(data, _state, ram)))
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

    private Instruction[] InitialiseInstructions()
    {
        var instructions = new Dictionary<int, Instruction>();

        InitialiseBaseInstructions(instructions);

        InitialiseDDInstructions(instructions);
        
        InitialiseFDInstructions(instructions);
        
        InitialiseEDInstructions(instructions);
        
        InitialiseCBInstructions(instructions);

        var instructionArray = new Instruction[instructions.Max(i => i.Key) + 1];

        foreach (var instruction in instructions)
        {
            instructionArray[instruction.Key] = instruction.Value;
        }

        return instructionArray;
    }

    private void InitialiseBaseInstructions(Dictionary<int, Instruction> instructions)
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

        instructions[0x0F] = new Instruction("RRCA", 1, RRCA, 4, "RRCA A");

        instructions[0x10] = new Instruction("DJNZ e", 2, DJNZ_e, 8, "DJNZ B, e");

        instructions[0x11] = new Instruction("LD DE, nn", 3, i => LD_RR_nn(i, Register.DE), 10);

        instructions[0x12] = new Instruction("LD (DE), A", 1, i => LD_addr_RR_R(i, Register.DE, Register.A), 7);

        instructions[0x13] = new Instruction("INC DE", 1, i => INC_RR(i, Register.DE), 6);

        instructions[0x14] = new Instruction("INC D", 1, i => INC_R(i, Register.D), 4);

        instructions[0x15] = new Instruction("DEC D", 1, i => DEC_R(i, Register.D), 4);

        instructions[0x16] = new Instruction("LD D, n", 2, i => LD_R_n(i, Register.D), 7);

        instructions[0x17] = new Instruction("RLA", 1, RLA, 4, "RLA A");

        instructions[0x18] = new Instruction("JR e", 2, JR_e, 12);

        instructions[0x19] = new Instruction("ADD HL, DE", 1, i => ADD_RR_RR(i, Register.HL, Register.DE), 11);

        instructions[0x1A] = new Instruction("LD A, (DE)", 1, i => LD_R_addr_RR(i, Register.A, Register.DE), 7);

        instructions[0x1B] = new Instruction("DEC DE", 1, i => DEC_RR(i, Register.DE), 6);

        instructions[0x1C] = new Instruction("INC E", 1, i => INC_R(i, Register.E), 4);

        instructions[0x1D] = new Instruction("DEC E", 1, i => DEC_R(i, Register.E), 4);

        instructions[0x1E] = new Instruction("LD E, n", 2, i => LD_R_n(i, Register.E), 7);

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

        instructions[0x29] = new Instruction("ADD HL, HL", 1, i => ADD_RR_RR(i, Register.HL, Register.HL), 11);

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

        instructions[0x46] = new Instruction("LD B, (HL)", 1, i => LD_R_addr_RR(i, Register.B, Register.HL), 7);

        instructions[0x47] = new Instruction("LD B, A", 1, i => LD_R_R(i, Register.B, Register.A), 4);

        instructions[0x48] = new Instruction("LD C, B", 1, i => LD_R_R(i, Register.C, Register.B), 4);

        instructions[0x49] = new Instruction("LD C, C", 1, i => LD_R_R(i, Register.C, Register.C), 4);

        instructions[0x4A] = new Instruction("LD C, D", 1, i => LD_R_R(i, Register.C, Register.D), 4);

        instructions[0x4B] = new Instruction("LD C, E", 1, i => LD_R_R(i, Register.C, Register.E), 4);

        instructions[0x4C] = new Instruction("LD C, H", 1, i => LD_R_R(i, Register.C, Register.H), 4);

        instructions[0x4D] = new Instruction("LD C, L", 1, i => LD_R_R(i, Register.C, Register.L), 4);

        instructions[0x4E] = new Instruction("LD C, (HL)", 1, i => LD_R_addr_RR(i, Register.C, Register.HL), 7);

        instructions[0x4F] = new Instruction("LD C, A", 1, i => LD_R_R(i, Register.C, Register.A), 4);

        instructions[0x50] = new Instruction("LD D, B", 1, i => LD_R_R(i, Register.D, Register.B), 4);

        instructions[0x51] = new Instruction("LD D, C", 1, i => LD_R_R(i, Register.D, Register.C), 4);

        instructions[0x52] = new Instruction("LD D, D", 1, i => LD_R_R(i, Register.D, Register.D), 4);

        instructions[0x53] = new Instruction("LD D, E", 1, i => LD_R_R(i, Register.D, Register.E), 4);

        instructions[0x54] = new Instruction("LD D, H", 1, i => LD_R_R(i, Register.D, Register.H), 4);

        instructions[0x55] = new Instruction("LD D, L", 1, i => LD_R_R(i, Register.D, Register.L), 4);

        instructions[0x56] = new Instruction("LD D, (HL)", 1, i => LD_R_addr_RR(i, Register.D, Register.HL), 7);

        instructions[0x57] = new Instruction("LD D, A", 1, i => LD_R_R(i, Register.D, Register.A), 4);

        instructions[0x58] = new Instruction("LD E, B", 1, i => LD_R_R(i, Register.E, Register.B), 4);

        instructions[0x59] = new Instruction("LD E, C", 1, i => LD_R_R(i, Register.E, Register.C), 4);

        instructions[0x5A] = new Instruction("LD E, D", 1, i => LD_R_R(i, Register.E, Register.D), 4);

        instructions[0x5B] = new Instruction("LD E, E", 1, i => LD_R_R(i, Register.E, Register.E), 4);

        instructions[0x5C] = new Instruction("LD E, H", 1, i => LD_R_R(i, Register.E, Register.H), 4);

        instructions[0x5D] = new Instruction("LD E, L", 1, i => LD_R_R(i, Register.E, Register.L), 4);

        instructions[0x5E] = new Instruction("LD E, (HL)", 1, i => LD_R_addr_RR(i, Register.E, Register.HL), 7);

        instructions[0x5F] = new Instruction("LD E, A", 1, i => LD_R_R(i, Register.E, Register.A), 4);

        instructions[0x60] = new Instruction("LD H, B", 1, i => LD_R_R(i, Register.H, Register.B), 4);

        instructions[0x61] = new Instruction("LD H, C", 1, i => LD_R_R(i, Register.H, Register.C), 4);

        instructions[0x62] = new Instruction("LD H, D", 1, i => LD_R_R(i, Register.H, Register.D), 4);

        instructions[0x63] = new Instruction("LD H, E", 1, i => LD_R_R(i, Register.H, Register.E), 4);

        instructions[0x64] = new Instruction("LD H, H", 1, i => LD_R_R(i, Register.H, Register.H), 4);

        instructions[0x65] = new Instruction("LD H, L", 1, i => LD_R_R(i, Register.H, Register.L), 4);

        instructions[0x66] = new Instruction("LD H, (HL)", 1, i => LD_R_addr_RR(i, Register.H, Register.HL), 7);

        instructions[0x67] = new Instruction("LD H, A", 1, i => LD_R_R(i, Register.H, Register.A), 4);

        instructions[0x68] = new Instruction("LD L, B", 1, i => LD_R_R(i, Register.L, Register.B), 4);

        instructions[0x69] = new Instruction("LD L, C", 1, i => LD_R_R(i, Register.L, Register.C), 4);

        instructions[0x6A] = new Instruction("LD L, D", 1, i => LD_R_R(i, Register.L, Register.D), 4);

        instructions[0x6B] = new Instruction("LD L, E", 1, i => LD_R_R(i, Register.L, Register.E), 4);

        instructions[0x6C] = new Instruction("LD L, H", 1, i => LD_R_R(i, Register.L, Register.H), 4);

        instructions[0x6D] = new Instruction("LD L, L", 1, i => LD_R_R(i, Register.L, Register.L), 4);

        instructions[0x6E] = new Instruction("LD L, (HL)", 1, i => LD_R_addr_RR(i, Register.L, Register.HL), 7);

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

        instructions[0x7E] = new Instruction("LD A, (HL)", 1, i => LD_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x7F] = new Instruction("LD A, A", 1, i => LD_R_R(i, Register.A, Register.A), 4);

        instructions[0x80] = new Instruction("ADD A, B", 1, i => ADD_R_R(i, Register.A, Register.B), 4);

        instructions[0x81] = new Instruction("ADD A, C", 1, i => ADD_R_R(i, Register.A, Register.C), 4);

        instructions[0x82] = new Instruction("ADD A, D", 1, i => ADD_R_R(i, Register.A, Register.D), 4);

        instructions[0x83] = new Instruction("ADD A, E", 1, i => ADD_R_R(i, Register.A, Register.E), 4);

        instructions[0x84] = new Instruction("ADD A, H", 1, i => ADD_R_R(i, Register.A, Register.H), 4);

        instructions[0x85] = new Instruction("ADD A, L", 1, i => ADD_R_R(i, Register.A, Register.L), 4);

        instructions[0x86] = new Instruction("ADD A, (HL)", 1, i => ADD_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x87] = new Instruction("ADD A, A", 1, i => ADD_R_R(i, Register.A, Register.A), 4);

        instructions[0x88] = new Instruction("ADC A, B", 1, i => ADC_R_R(i, Register.A, Register.B), 4);

        instructions[0x89] = new Instruction("ADC A, C", 1, i => ADC_R_R(i, Register.A, Register.C), 4);

        instructions[0x8A] = new Instruction("ADC A, D", 1, i => ADC_R_R(i, Register.A, Register.D), 4);

        instructions[0x8B] = new Instruction("ADC A, E", 1, i => ADC_R_R(i, Register.A, Register.E), 4);

        instructions[0x8C] = new Instruction("ADC A, H", 1, i => ADC_R_R(i, Register.A, Register.H), 4);

        instructions[0x8D] = new Instruction("ADC A, L", 1, i => ADC_R_R(i, Register.A, Register.L), 4);

        instructions[0x8E] = new Instruction("ADC A, (HL)", 1, i => ADC_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x8F] = new Instruction("ADC A, A", 1, i => ADC_R_R(i, Register.A, Register.A), 4);

        instructions[0x90] = new Instruction("SUB A, B", 1, i => SUB_R_R(i, Register.A, Register.B), 4);

        instructions[0x91] = new Instruction("SUB A, C", 1, i => SUB_R_R(i, Register.A, Register.C), 4);

        instructions[0x92] = new Instruction("SUB A, D", 1, i => SUB_R_R(i, Register.A, Register.D), 4);

        instructions[0x93] = new Instruction("SUB A, E", 1, i => SUB_R_R(i, Register.A, Register.E), 4);

        instructions[0x94] = new Instruction("SUB A, H", 1, i => SUB_R_R(i, Register.A, Register.H), 4);

        instructions[0x95] = new Instruction("SUB A, L", 1, i => SUB_R_R(i, Register.A, Register.L), 4);

        instructions[0x96] = new Instruction("SUB A, (HL)", 1, i => SUB_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x97] = new Instruction("SUB A, A", 1, i => SUB_R_R(i, Register.A, Register.A), 4);

        instructions[0x98] = new Instruction("SBC A, B", 1, i => SBC_R_R(i, Register.A, Register.B), 4);

        instructions[0x99] = new Instruction("SBC A, C", 1, i => SBC_R_R(i, Register.A, Register.C), 4);

        instructions[0x9A] = new Instruction("SBC A, D", 1, i => SBC_R_R(i, Register.A, Register.D), 4);

        instructions[0x9B] = new Instruction("SBC A, E", 1, i => SBC_R_R(i, Register.A, Register.E), 4);

        instructions[0x9C] = new Instruction("SBC A, H", 1, i => SBC_R_R(i, Register.A, Register.H), 4);

        instructions[0x9D] = new Instruction("SBC A, L", 1, i => SBC_R_R(i, Register.A, Register.L), 4);

        instructions[0x9E] = new Instruction("SBC A, (HL)", 1, i => SBC_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x9F] = new Instruction("SBC A, A", 1, i => SBC_R_R(i, Register.A, Register.A), 4);

        instructions[0xA0] = new Instruction("AND A, B", 1, i => AND_R_R(i, Register.A, Register.B), 4);

        instructions[0xA1] = new Instruction("AND A, C", 1, i => AND_R_R(i, Register.A, Register.C), 4);

        instructions[0xA2] = new Instruction("AND A, D", 1, i => AND_R_R(i, Register.A, Register.D), 4);

        instructions[0xA3] = new Instruction("AND A, E", 1, i => AND_R_R(i, Register.A, Register.E), 4);

        instructions[0xA4] = new Instruction("AND A, H", 1, i => AND_R_R(i, Register.A, Register.H), 4);

        instructions[0xA5] = new Instruction("AND A, L", 1, i => AND_R_R(i, Register.A, Register.L), 4);

        instructions[0xA6] = new Instruction("AND A, (HL)", 1, i => AND_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xA7] = new Instruction("AND A, A", 1, i => AND_R_R(i, Register.A, Register.A), 4);

        instructions[0xA8] = new Instruction("XOR A, B", 1, i => XOR_R_R(i, Register.A, Register.B), 4);

        instructions[0xA9] = new Instruction("XOR A, C", 1, i => XOR_R_R(i, Register.A, Register.C), 4);

        instructions[0xAA] = new Instruction("XOR A, D", 1, i => XOR_R_R(i, Register.A, Register.D), 4);

        instructions[0xAB] = new Instruction("XOR A, E", 1, i => XOR_R_R(i, Register.A, Register.E), 4);

        instructions[0xAC] = new Instruction("XOR A, H", 1, i => XOR_R_R(i, Register.A, Register.H), 4);

        instructions[0xAD] = new Instruction("XOR A, L", 1, i => XOR_R_R(i, Register.A, Register.L), 4);

        instructions[0xAE] = new Instruction("XOR A, (HL)", 1, i => XOR_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xAF] = new Instruction("XOR A, A", 1, i => XOR_R_R(i, Register.A, Register.A), 4);

        instructions[0xB0] = new Instruction("OR A, B", 1, i => OR_R_R(i, Register.A, Register.B), 4);

        instructions[0xB1] = new Instruction("OR A, C", 1, i => OR_R_R(i, Register.A, Register.C), 4);

        instructions[0xB2] = new Instruction("OR A, D", 1, i => OR_R_R(i, Register.A, Register.D), 4);

        instructions[0xB3] = new Instruction("OR A, E", 1, i => OR_R_R(i, Register.A, Register.E), 4);

        instructions[0xB4] = new Instruction("OR A, H", 1, i => OR_R_R(i, Register.A, Register.H), 4);

        instructions[0xB5] = new Instruction("OR A, L", 1, i => OR_R_R(i, Register.A, Register.L), 4);

        instructions[0xB6] = new Instruction("OR A, (HL)", 1, i => OR_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xB7] = new Instruction("OR A, A", 1, i => OR_R_R(i, Register.A, Register.A), 4);

        instructions[0xB8] = new Instruction("CP A, B", 1, i => CP_R_R(i, Register.A, Register.B), 4);

        instructions[0xB9] = new Instruction("CP A, C", 1, i => CP_R_R(i, Register.A, Register.C), 4);

        instructions[0xBA] = new Instruction("CP A, D", 1, i => CP_R_R(i, Register.A, Register.D), 4);

        instructions[0xBB] = new Instruction("CP A, E", 1, i => CP_R_R(i, Register.A, Register.E), 4);

        instructions[0xBC] = new Instruction("CP A, H", 1, i => CP_R_R(i, Register.A, Register.H), 4);

        instructions[0xBD] = new Instruction("CP A, L", 1, i => CP_R_R(i, Register.A, Register.L), 4);

        instructions[0xBE] = new Instruction("CP A, (HL)", 1, i => CP_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xBF] = new Instruction("CP A, A", 1, i => CP_R_R(i, Register.A, Register.A), 4);

        instructions[0xC0] = new Instruction("RET NZ", 1, RET_NZ, 5);

        instructions[0xC1] = new Instruction("POP BC", 1, i => POP_RR(i, Register.BC), 10);

        instructions[0xC2] = new Instruction("JP NZ, nn", 3, JP_NZ_nn, 10);

        instructions[0xC3] = new Instruction("JP nn", 3, JP_nn, 10);

        instructions[0xC4] = new Instruction("CALL NZ, nn", 3, CALL_NZ_nn, 10);

        instructions[0xC5] = new Instruction("PUSH BC", 1, i => PUSH_RR(i, Register.BC), 11);

        instructions[0xC6] = new Instruction("ADD A, n", 2, i => ADD_R_n(i, Register.A), 7);

        instructions[0xC7] = new Instruction("RST 0x00", 1, i => RST(i, 0x00), 11);

        instructions[0xC8] = new Instruction("RET Z", 1, RET_Z, 5);

        instructions[0xC9] = new Instruction("RET", 1, RET, 10);

        instructions[0xCA] = new Instruction("JP Z, nn", 3, JP_Z_nn, 10);
                
        // Switch opcode set to CB
        instructions[0xCB] = new Instruction("SOPSET CB", 1, _ => SetOpcodePrefix(0xCB), 4);

        instructions[0xCC] = new Instruction("CALL Z, nn", 3, CALL_Z_nn, 10);

        instructions[0xCD] = new Instruction("CALL nn", 3, CALL_nn, 17);

        instructions[0xCE] = new Instruction("ADC A, n", 2, i => ADC_R_n(i, Register.A), 7);

        instructions[0xCF] = new Instruction("RST 0x08", 1, i => RST(i, 0x08), 11);

        instructions[0xD0] = new Instruction("RET NC", 1, RET_NC, 5);

        instructions[0xD1] = new Instruction("POP DE", 1, i => POP_RR(i, Register.DE), 10);

        instructions[0xD2] = new Instruction("JP NC, nn", 3, JP_NC_nn, 10);

        // instructions[0xD3] = new Instruction("OUT (n), A", 2, i => OUT_addr_n_R(i, Register.A), 11);

        instructions[0xD4] = new Instruction("CALL NC, nn", 3, CALL_NC_nn, 10);

        instructions[0xD5] = new Instruction("PUSH DE", 1, i => PUSH_RR(i, Register.DE), 11);

        instructions[0xD6] = new Instruction("SUB A, n", 2, i => SUB_R_n(i, Register.A), 7);

        instructions[0xD7] = new Instruction("RST 0x10", 1, i => RST(i, 0x10), 11);

        instructions[0xD8] = new Instruction("RET C", 1, RET_C, 5);

        instructions[0xD9] = new Instruction("EXX", 1, EXX, 4);

        instructions[0xDA] = new Instruction("JP C, nn", 3, JP_C_nn, 10);

        //instructions[0xDB] = new Instruction("IN A, (n)", 2, IN_R_addr_N, 11);

        instructions[0xDC] = new Instruction("CALL C, nn", 3, CALL_C_nn, 10);

        // Switch opcode set to DD
        instructions[0xDD] = new Instruction("SOPSET DD", 1, _ => SetOpcodePrefix(0xDD), 4);

        instructions[0xDE] = new Instruction("SBC A, n", 2, i => SBC_R_n(i, Register.A), 7);

        instructions[0xDF] = new Instruction("RST 0x18", 1, i => RST(i, 0x18), 11);

        instructions[0xE0] = new Instruction("RET PO", 1, RET_PO, 5);

        instructions[0xE1] = new Instruction("POP HL", 1, i => POP_RR(i, Register.HL), 10);

        instructions[0xE2] = new Instruction("JP PO, nn", 3, JP_PO_nn, 10);

        instructions[0xE3] = new Instruction("EX (SP), HL", 1, i => EX_addr_SP_RR(i, Register.HL), 19);

        instructions[0xE4] = new Instruction("CALL PO, nn", 3, CALL_PO_nn, 10);

        instructions[0xE5] = new Instruction("PUSH HL", 1, i => PUSH_RR(i, Register.HL), 11);

        instructions[0xE6] = new Instruction("AND A, n", 2, i => AND_R_n(i, Register.A), 7);

        instructions[0xE7] = new Instruction("RST 0x20", 1, i => RST(i, 0x20), 11);

        instructions[0xE8] = new Instruction("RET PE", 1, RET_PE, 5);

        instructions[0xE9] = new Instruction("JP (HL)", 1, i => JP_addr_RR(i, Register.HL), 4);

        instructions[0xEA] = new Instruction("JP PE, nn", 3, JP_PE_nn, 10);

        instructions[0xEB] = new Instruction("EX DE, HL", 1, i => EX_RR_RR(i, Register.DE, Register.HL), 4);

        instructions[0xEC] = new Instruction("CALL PE, nn", 3, CALL_PE_nn, 10);
        
        // Switch opcode set to ED
        instructions[0xED] = new Instruction("SOPSET ED", 1, _ => SetOpcodePrefix(0xED), 4);

        instructions[0xEE] = new Instruction("XOR A, n", 2, i => XOR_R_n(i, Register.A), 7);

        instructions[0xEF] = new Instruction("RST 0x28", 1, i => RST(i, 0x28), 11);

        instructions[0xF0] = new Instruction("RET NS", 1, RET_NS, 5);

        instructions[0xF1] = new Instruction("POP AF", 1, i => POP_RR(i, Register.AF), 10);

        instructions[0xF2] = new Instruction("JP NS, nn", 3, JP_NS_nn, 10);

        instructions[0xF3] = new Instruction("DI", 1, DI, 4);

        instructions[0xF4] = new Instruction("CALL S, nn", 3, CALL_NS_nn, 10);

        instructions[0xF5] = new Instruction("PUSH AF", 1, i => PUSH_RR(i, Register.AF), 11);

        instructions[0xF6] = new Instruction("OR A, n", 2, i => OR_R_n(i, Register.A), 7);

        instructions[0xF7] = new Instruction("RST 0x30", 1, i => RST(i, 0x30), 11);

        instructions[0xF8] = new Instruction("RET S", 1, RET_S, 5);

        instructions[0xF9] = new Instruction("LD SP, HL", 1, LD_RR_RR, 6);

        instructions[0xFA] = new Instruction("JP S, nn", 3, JP_S_nn, 10);

        instructions[0xFB] = new Instruction("EI", 1, EI, 4);

        instructions[0xFC] = new Instruction("CALL S, nn", 3, CALL_S_nn, 10);
        
        // Switch opcode set to FD
        instructions[0xFD] = new Instruction("SOPSET FD", 1, _ => SetOpcodePrefix(0xFD), 4);

        instructions[0xFE] = new Instruction("CP A, n", 2, i => CP_R_n(i, Register.A), 7);

        instructions[0xFF] = new Instruction("RST 0x38", 1, i => RST(i, 0x38), 11);
    }

    private static void InitialiseDDInstructions(Dictionary<int, Instruction> instructions)
    {
        // INFO: These are basically timing equivalent to base instructions with an extra 4 cycles at the start. These are added by the prefix itself, so ignore here.

        instructions[0xDD09] = new Instruction("ADD IX, BC", 1, i => ADD_RR_RR(i, Register.IX, Register.BC), 11);

        instructions[0xDD19] = new Instruction("ADD IX, DE", 1, i => ADD_RR_RR(i, Register.IX, Register.DE), 11);

        instructions[0xDD21] = new Instruction("LD IX, nn", 3, i => LD_RR_nn(i, Register.IX), 10);

        instructions[0xDD22] = new Instruction("LD (nn), IX", 3, i => LD_addr_nn_RR(i, Register.IX), 16);

        instructions[0xDD23] = new Instruction("INC IX", 1, i => INC_RR(i, Register.IX), 6);

        instructions[0xDD24] = new Instruction("INC IXh", 1, i => INC_RRh(i, Register.IX), 4);

        instructions[0xDD25] = new Instruction("DEC IXh", 1, i => DEC_RRh(i, Register.IX), 4);

        instructions[0xDD26] = new Instruction("LD IXh, n", 2, i => LD_RRh_n(i, Register.IX), 7);

        instructions[0xDD29] = new Instruction("ADD IX, IX", 1, i => ADD_RR_RR(i, Register.IX, Register.IX), 11);

        instructions[0xDD2A] = new Instruction("LD IX, (nn)", 3, i => LD_RR_addr_nn(i, Register.IX), 16);

        instructions[0xDD2B] = new Instruction("DEC IX", 1, i => DEC_RR(i, Register.IX), 6);

        instructions[0xDD2C] = new Instruction("INC IXl", 1, i => INC_RRl(i, Register.IX), 4);

        instructions[0xDD2D] = new Instruction("DEC IXl", 1, i => DEC_RRl(i, Register.IX), 4);

        instructions[0xDD2E] = new Instruction("LD IXl, n", 2, i => LD_RRl_n(i, Register.IX), 7);

        instructions[0xDD34] = new Instruction("INC (IX + d)", 2, i => INC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD35] = new Instruction("DEC (IX + d)", 2, i => DEC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD36] = new Instruction("LD (IX + d), n", 3, i => LD_addr_RR_plus_d_n(i, Register.IX), 19);

        instructions[0xDD39] = new Instruction("ADD IX, SP", 1, i => ADD_RR_SP(i, Register.IX), 11);

        instructions[0xDD44] = new Instruction("LD B, IXh", 1, i => LD_R_RRh(i, Register.B, Register.IX), 4);

        instructions[0xDD45] = new Instruction("LD B, IXl", 1, i => LD_R_RRl(i, Register.B, Register.IX), 4);

        instructions[0xDD46] = new Instruction("LD B, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.B, Register.IX), 15);

        instructions[0xDD4C] = new Instruction("LD C, IXh", 1, i => LD_R_RRh(i, Register.C, Register.IX), 4);

        instructions[0xDD4D] = new Instruction("LD C, IXl", 1, i => LD_R_RRl(i, Register.C, Register.IX), 4);

        instructions[0xDD4E] = new Instruction("LD C, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.C, Register.IX), 15);

        instructions[0xDD54] = new Instruction("LD D, IXh", 1, i => LD_R_RRh(i, Register.D, Register.IX), 4);

        instructions[0xDD55] = new Instruction("LD D, IXl", 1, i => LD_R_RRl(i, Register.D, Register.IX), 4);

        instructions[0xDD56] = new Instruction("LD D, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.D, Register.IX), 15);

        instructions[0xDD5C] = new Instruction("LD E, IXh", 1, i => LD_R_RRh(i, Register.E, Register.IX), 4);

        instructions[0xDD5D] = new Instruction("LD E, IXl", 1, i => LD_R_RRl(i, Register.E, Register.IX), 4);

        instructions[0xDD5E] = new Instruction("LD E, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.E, Register.IX), 15);

        instructions[0xDD60] = new Instruction("LD IXh, B", 1, i => LD_RRh_R(i, Register.IX, Register.B), 4);

        instructions[0xDD61] = new Instruction("LD IXh, C", 1, i => LD_RRh_R(i, Register.IX, Register.C), 4);

        instructions[0xDD62] = new Instruction("LD IXh, D", 1, i => LD_RRh_R(i, Register.IX, Register.D), 4);

        instructions[0xDD63] = new Instruction("LD IXh, E", 1, i => LD_RRh_R(i, Register.IX, Register.E), 4);

        instructions[0xDD64] = new Instruction("LD IXh, IXh", 1, i => LD_RRh_RRh(i, Register.IX, Register.IX), 4);

        instructions[0xDD65] = new Instruction("LD IXh, IXl", 1, i => LD_RRh_RRl(i, Register.IX, Register.IX), 4);

        instructions[0xDD66] = new Instruction("LD H, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.H, Register.IX), 15);

        instructions[0xDD67] = new Instruction("LD IXh, A", 1, i => LD_RRh_R(i, Register.IX, Register.A), 4);

        instructions[0xDD68] = new Instruction("LD IXl, B", 1, i => LD_RRl_R(i, Register.IX, Register.B), 4);

        instructions[0xDD69] = new Instruction("LD IXl, C", 1, i => LD_RRl_R(i, Register.IX, Register.C), 4);

        instructions[0xDD6A] = new Instruction("LD IXl, D", 1, i => LD_RRl_R(i, Register.IX, Register.D), 4);

        instructions[0xDD6B] = new Instruction("LD IXl, E", 1, i => LD_RRl_R(i, Register.IX, Register.E), 4);

        instructions[0xDD6C] = new Instruction("LD IXl, IXh", 1, i => LD_RRl_RRh(i, Register.IX, Register.IX), 4);

        instructions[0xDD6D] = new Instruction("LD IXl, IXl", 1, i => LD_RRl_RRl(i, Register.IX, Register.IX), 4);

        instructions[0xDD6E] = new Instruction("LD L, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.L, Register.IX), 15);

        instructions[0xDD6F] = new Instruction("LD IXl, A", 1, i => LD_RRl_R(i, Register.IX, Register.A), 4);

        instructions[0xDD70] = new Instruction("LD (IX + d), B", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.B), 4);

        instructions[0xDD71] = new Instruction("LD (IX + d), C", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.C), 4);

        instructions[0xDD72] = new Instruction("LD (IX + d), D", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.D), 4);

        instructions[0xDD73] = new Instruction("LD (IX + d), E", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.E), 4);

        instructions[0xDD74] = new Instruction("LD (IX + d), H", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.H), 4);

        instructions[0xDD75] = new Instruction("LD (IX + d), L", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.L), 4);

        instructions[0xDD77] = new Instruction("LD (IX + d), A", 2, i => LD_addr_RR_plus_d_R(i, Register.IX, Register.A), 15);

        instructions[0xDD7C] = new Instruction("LD A, IXh", 1, i => LD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD7D] = new Instruction("LD A, IXl", 1, i => LD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD7E] = new Instruction("LD A, (IX + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD84] = new Instruction("ADD A, IXh", 1, i => ADD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD85] = new Instruction("ADD A, IXl", 1, i => ADD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD86] = new Instruction("ADD A, (IX + d)", 2, i => ADD_R_addr_RR_plus_d(i, Register.A, Register.IX), 4);

        instructions[0xDD8C] = new Instruction("ADC A, IXh", 1, i => ADC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD8D] = new Instruction("ADC A, IXl", 1, i => ADC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD8E] = new Instruction("ADC A, (IX + d)", 2, i => ADC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD94] = new Instruction("SUB A, IXh", 1, i => SUB_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD95] = new Instruction("SUB A, IXl", 1, i => SUB_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD96] = new Instruction("SUB A, (IX + d)", 2, i => SUB_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD9C] = new Instruction("SBC A, IXh", 1, i => SBC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD9D] = new Instruction("SBC A, IXl", 1, i => SBC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD9E] = new Instruction("SBC A, (IX + d)", 2, i => SBC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDA4] = new Instruction("AND A, IXh", 1, i => AND_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDA5] = new Instruction("AND A, IXl", 1, i => AND_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDA6] = new Instruction("AND A, (IX + d)", 2, i => AND_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDAC] = new Instruction("XOR A, IXh", 1, i => XOR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDAD] = new Instruction("XOR A, IXl", 1, i => XOR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDAE] = new Instruction("XOR A, (IX + d)", 2, i => XOR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDB4] = new Instruction("OR A, IXh", 1, i => OR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDB5] = new Instruction("OR A, IXh", 1, i => OR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDB6] = new Instruction("OR A, (IX + d)", 2, i => OR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDBC] = new Instruction("CP A, IXh", 1, i => CP_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDBD] = new Instruction("CP A, IXh", 1, i => CP_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDBE] = new Instruction("CP A, (IX + d)", 2, i => CP_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDDD] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0xDDE1] = new Instruction("POP IX", 1, i => POP_RR(i, Register.IX), 10);

        instructions[0xDDE3] = new Instruction("EX (SP), IX", 1, i => EX_addr_SP_RR(i, Register.IX), 19);

        instructions[0xDDE5] = new Instruction("PUSH IX", 1, i => PUSH_RR(i, Register.IX), 11);

        instructions[0xDDE9] = new Instruction("JP (IX)", 1, i => JP_addr_RR(i, Register.IX), 4);

        instructions[0xDDF9] = new Instruction("LD SP, IX", 1, i => LD_SP_RR(i, Register.IX), 6);

        instructions[0xDDFD] = new Instruction("NOP", 1, _ => NOP(), 4);
    }

    private static void InitialiseFDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xFD09] = new Instruction("ADD IY, BC", 1, i => ADD_RR_RR(i, Register.IY, Register.BC), 11);

        instructions[0xFD19] = new Instruction("ADD IY, DE", 1, i => ADD_RR_RR(i, Register.IY, Register.DE), 11);

        instructions[0xFD21] = new Instruction("LD IY, nn", 3, i => LD_RR_nn(i, Register.IY), 10);

        instructions[0xFD22] = new Instruction("LD (nn), IY", 3, i => LD_addr_nn_RR(i, Register.IY), 16);

        instructions[0xFD23] = new Instruction("INC IY", 1, i => INC_RR(i, Register.IY), 6);

        instructions[0xFD24] = new Instruction("INC IYh", 1, i => INC_RRh(i, Register.IY), 4);

        instructions[0xFD25] = new Instruction("DEC IYl", 1, i => DEC_RRh(i, Register.IY), 4);

        instructions[0xFD26] = new Instruction("LD IYh, n", 2, i => LD_RRh_n(i, Register.IY), 4);

        instructions[0xFD29] = new Instruction("ADD IY, IY", 1, i => ADD_RR_RR(i, Register.IY, Register.IY), 11);

        instructions[0xFD2A] = new Instruction("LD IY, (nn)", 3, i => LD_RR_addr_nn(i, Register.IY), 11);

        instructions[0xFD2B] = new Instruction("DEC IY", 1, i => DEC_RR(i, Register.IY), 6);

        instructions[0xFD2C] = new Instruction("INC IYl", 1, i => INC_RRl(i, Register.IY), 4);

        instructions[0xFD2D] = new Instruction("DEC IYl", 1, i => DEC_RRl(i, Register.IY), 4);

        instructions[0xFD2E] = new Instruction("LD IYl, n", 2, i => LD_RRl_n(i, Register.IY), 7);

        instructions[0xFD34] = new Instruction("INC (IY + d)", 2, i => INC_addr_RR_plus_d(i, Register.IY), 19);

        instructions[0xFD35] = new Instruction("DEC (IY + d)", 2, i => DEC_addr_RR_plus_d(i, Register.IY), 19);

        instructions[0xFD36] = new Instruction("LD (IY + d), n", 3, i => LD_addr_RR_plus_d_n(i, Register.IY), 15);

        instructions[0xFD39] = new Instruction("ADD IY, SP", 1, i => ADD_RR_SP(i, Register.IY), 11);

        instructions[0xFD44] = new Instruction("LD B, IYh", 1, i => LD_R_RRh(i, Register.B, Register.IY), 4);

        instructions[0xFD45] = new Instruction("LD B, IYl", 1, i => LD_R_RRl(i, Register.B, Register.IY), 4);

        instructions[0xFD46] = new Instruction("LD B, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.B, Register.IY), 15);

        instructions[0xFD4C] = new Instruction("LD C, IYh", 1, i => LD_R_RRh(i, Register.C, Register.IY), 4);

        instructions[0xFD4D] = new Instruction("LD C, IYl", 1, i => LD_R_RRl(i, Register.C, Register.IY), 4);

        instructions[0xFD4E] = new Instruction("LD C, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.C, Register.IY), 15);

        instructions[0xFD54] = new Instruction("LD D, IYh", 1, i => LD_R_RRh(i, Register.D, Register.IY), 4);

        instructions[0xFD55] = new Instruction("LD D, IYl", 1, i => LD_R_RRl(i, Register.D, Register.IY), 4);

        instructions[0xFD56] = new Instruction("LD D, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.D, Register.IY), 15);

        instructions[0xFD5C] = new Instruction("LD E, IYh", 1, i => LD_R_RRh(i, Register.E, Register.IY), 4);

        instructions[0xFD5D] = new Instruction("LD E, IYl", 1, i => LD_R_RRl(i, Register.E, Register.IY), 4);

        instructions[0xFD5E] = new Instruction("LD E, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.E, Register.IY), 15);

        instructions[0xFD60] = new Instruction("LD IYh, B", 1, i => LD_RRh_R(i, Register.IY, Register.B), 4);

        instructions[0xFD61] = new Instruction("LD IYh, C", 1, i => LD_RRh_R(i, Register.IY, Register.C), 4);

        instructions[0xFD62] = new Instruction("LD IYh, D", 1, i => LD_RRh_R(i, Register.IY, Register.D), 4);

        instructions[0xFD63] = new Instruction("LD IYh, E", 1, i => LD_RRh_R(i, Register.IY, Register.E), 4);

        instructions[0xFD64] = new Instruction("LD IYh, IYh", 1, i => LD_RRh_RRh(i, Register.IY, Register.IY), 4);

        instructions[0xFD65] = new Instruction("LD IYh, IYl", 1, i => LD_RRh_RRl(i, Register.IY, Register.IY), 4);

        instructions[0xFD66] = new Instruction("LD H, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.H, Register.IY), 15);

        instructions[0xFD67] = new Instruction("LD IYh, A", 1, i => LD_RRh_R(i, Register.IY, Register.A), 4);

        instructions[0xFD68] = new Instruction("LD IYl, B", 1, i => LD_RRh_R(i, Register.IY, Register.A), 4);

        instructions[0xFD69] = new Instruction("LD IYl, C", 1, i => LD_RRh_R(i, Register.IY, Register.B), 4);

        instructions[0xFD6A] = new Instruction("LD IYl, D", 1, i => LD_RRh_R(i, Register.IY, Register.D), 4);

        instructions[0xFD6B] = new Instruction("LD IYl, E", 1, i => LD_RRh_R(i, Register.IY, Register.E), 4);

        instructions[0xFD6C] = new Instruction("LD IYl, IYh", 1, i => LD_RRl_RRh(i, Register.IY, Register.IY), 4);

        instructions[0xFD6D] = new Instruction("LD IYl, IYl", 1, i => LD_RRl_RRl(i, Register.IY, Register.IY), 4);

        instructions[0xFD6E] = new Instruction("LD L, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.L, Register.IY), 4);

        instructions[0xFD6F] = new Instruction("LD IYl, A", 1, i => LD_RRl_R(i, Register.IY, Register.A), 4);

        instructions[0xFD70] = new Instruction("LD (IY + d), B", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.B), 15);

        instructions[0xFD71] = new Instruction("LD (IY + d), C", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.C), 15);

        instructions[0xFD72] = new Instruction("LD (IY + d), D", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.D), 15);

        instructions[0xFD73] = new Instruction("LD (IY + d), E", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.E), 15);

        instructions[0xFD74] = new Instruction("LD (IY + d), H", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.H), 15);

        instructions[0xFD75] = new Instruction("LD (IY + d), L", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.L), 15);

        instructions[0xFD77] = new Instruction("LD (IY + d), A", 2, i => LD_addr_RR_plus_d_R(i, Register.IY, Register.A), 15);

        instructions[0xFD7C] = new Instruction("LD A, IYh", 1, i => LD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD7D] = new Instruction("LD A, IYl", 1, i => LD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD7E] = new Instruction("LD A, (IY + d)", 2, i => LD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD84] = new Instruction("ADD A, IYh", 1, i => ADD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD85] = new Instruction("ADD A, IYl", 1, i => ADD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD86] = new Instruction("ADD A, (IY + d)", 2, i => ADD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD8C] = new Instruction("ADC A, IYh", 1, i => ADC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD8D] = new Instruction("ADC A, IYl", 1, i => ADC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD8E] = new Instruction("ADC A, (IY + d)", 2, i => ADC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD94] = new Instruction("SUB A, IYh", 1, i => SUB_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD95] = new Instruction("SUB A, IYl", 1, i => SUB_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD96] = new Instruction("SUB A, (IY + d)", 2, i => SUB_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD9C] = new Instruction("SBC A, IYh", 1, i => SBC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD9D] = new Instruction("SBC A, IYl", 1, i => SBC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD9E] = new Instruction("SBC A, (IY + d)", 2, i => SBC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDA4] = new Instruction("AND A, IYh", 1, i => AND_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDA5] = new Instruction("AND A, IYl", 1, i => AND_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDA6] = new Instruction("AND A, (IY + d)", 2, i => AND_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDAC] = new Instruction("XOR A, IYh", 1, i => XOR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDAD] = new Instruction("XOR A, IYl", 1, i => XOR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDAE] = new Instruction("XOR A, (IY + d)", 2, i => XOR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDB4] = new Instruction("OR A, IYh", 1, i => OR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDB5] = new Instruction("OR A, IYl", 1, i => OR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDB6] = new Instruction("OR A, (IY + d)", 2, i => OR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDBC] = new Instruction("CP A, IYh", 1, i => CP_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDBD] = new Instruction("CP A, IYl", 1, i => CP_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDBE] = new Instruction("CP A, (IY + d)", 2, i => CP_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDDD] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0xFDE1] = new Instruction("POP IY", 1, i => POP_RR(i, Register.IY), 10);

        instructions[0xFDE3] = new Instruction("EX (SP), IY", 1, i => EX_addr_SP_RR(i, Register.IY), 19);

        instructions[0xFDE5] = new Instruction("PUSH IY", 1, i => PUSH_RR(i, Register.IY), 11);

        instructions[0xFDE9] = new Instruction("JP (IY)", 1, i => JP_addr_RR(i, Register.IY), 4);

        instructions[0xFDF9] = new Instruction("LD SP, IY", 1, i => LD_SP_RR(i, Register.IY), 6);

        instructions[0xFDFD] = new Instruction("NOP", 1, _ => NOP(), 4);
    }

    private static void InitialiseEDInstructions(Dictionary<int, Instruction> instructions)
    {
        // TODO: instructions[0xED40] = new Instruction("IN B, (C)", 1, , 8);

        // TODO: instructions[0xED41] = new Instruction("OUT (C), B", 1, , 8);

        instructions[0xED42] = new Instruction("SBC HL, BC", 1, i => SBC_RR_RR(i, Register.HL, Register.BC), 11);

        instructions[0xED43] = new Instruction("LD (nn), BC", 3, i => LD_addr_nn_RR(i, Register.BC), 16);

        instructions[0xED44] = new Instruction("NEG", 1, NEG, 4);

        // TODO: instructions[0xED45] = new Instruction("RETN", 1, , 10);


    }

    private static void InitialiseCBInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xCB00] = new Instruction("RLC B", 1, i => RLC_R(i, Register.B), 4);

        instructions[0xCB01] = new Instruction("RLC C", 1, i => RLC_R(i, Register.C), 4);

        instructions[0xCB02] = new Instruction("RLC D", 1, i => RLC_R(i, Register.D), 4);

        instructions[0xCB03] = new Instruction("RLC E", 1, i => RLC_R(i, Register.E), 4);

        instructions[0xCB04] = new Instruction("RLC H", 1, i => RLC_R(i, Register.H), 4);

        instructions[0xCB05] = new Instruction("RLC L", 1, i => RLC_R(i, Register.L), 4);

        instructions[0xCB06] = new Instruction("RLC (HL)", 1, i => RLC_addr_RR(i, Register.HL), 11);

        instructions[0xCB07] = new Instruction("RLC A", 1, i => RLC_R(i, Register.A), 4);

        instructions[0xCB08] = new Instruction("RRC B", 1, i => RRC_R(i, Register.B), 4);

        instructions[0xCB09] = new Instruction("RRC C", 1, i => RRC_R(i, Register.C), 4);

        instructions[0xCB0A] = new Instruction("RRC D", 1, i => RRC_R(i, Register.D), 4);

        instructions[0xCB0B] = new Instruction("RRC E", 1, i => RRC_R(i, Register.E), 4);

        instructions[0xCB0C] = new Instruction("RRC H", 1, i => RRC_R(i, Register.H), 4);

        instructions[0xCB0D] = new Instruction("RRC L", 1, i => RRC_R(i, Register.L), 4);

        instructions[0xCB0E] = new Instruction("RRC (HL)", 1, i => RRC_addr_RR(i, Register.HL), 11);

        instructions[0xCB0F] = new Instruction("RRC A", 1, i => RRC_R(i, Register.A), 4);

        instructions[0xCB10] = new Instruction("RL B", 1, i => RL_R(i, Register.B), 4);

        instructions[0xCB11] = new Instruction("RL C", 1, i => RL_R(i, Register.C), 4);

        instructions[0xCB12] = new Instruction("RL D", 1, i => RL_R(i, Register.D), 4);

        instructions[0xCB13] = new Instruction("RL E", 1, i => RL_R(i, Register.E), 4);

        instructions[0xCB14] = new Instruction("RL H", 1, i => RL_R(i, Register.H), 4);

        instructions[0xCB15] = new Instruction("RL L", 1, i => RL_R(i, Register.L), 4);

        instructions[0xCB16] = new Instruction("RL (HL)", 1, i => RL_addr_RR(i, Register.HL), 11);

        instructions[0xCB17] = new Instruction("RL A", 1, i => RL_R(i, Register.A), 4);
    }

    private static bool NOP()
    {
        // Flags unaffected

        return true;
    }

    private static bool LD_RR_nn(Input input, Register register)
    {
        input.State.Registers.LoadFromRam(register, input.Data[1..3]);

        // Flags unaffected

        return true;
    }

    private static bool LD_addr_RR_R(Input input, Register target, Register source)
    {
        input.Ram[input.State.Registers.ReadPair(target)] = input.State.Registers[source];

        // Flags unaffected

        return true;
    }

    private static bool INC_RR(Input input, Register register)
    {
        unchecked
        {
            input.State.Registers.WritePair(register, (ushort) (input.State.Registers.ReadPair(register) + 1));
        }

        // Flags unaffected

        return true;
    }

    private static bool INC_R(Input input, Register register)
    {
        unchecked
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

        return true;
    }

    private static bool DEC_R(Input input, Register register)
    {
        unchecked
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

        return true;
    }

    private static bool LD_R_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];

        // Flags unaffected

        return true;
    }

    private static bool RLCA(Input input)
    {
        unchecked
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

        return true;
    }

    private static bool LD_addr_nn_R(Input input, Register register)
    {
        input.Ram[(input.Data[2] << 8) | input.Data[1]] = input.State.Registers[register];

        // Flags unaffected

        return true;
    }

    private static bool LD_R_addr_nn(Input input, Register register)
    {
        input.State.Registers[register] = input.Ram[(input.Data[2] << 8) | input.Data[1]];

        // Flags unaffected

        return true;
    }

    private static bool EX_RR_RaRa(Input input, Register register1, Register register2)
    {
        var alternate1 = Enum.Parse<Register>($"{register1}1");

        var alternate2 = Enum.Parse<Register>($"{register2}1");

        (input.State.Registers[register1], input.State.Registers[alternate1]) = (input.State.Registers[alternate1], input.State.Registers[register1]);

        (input.State.Registers[register2], input.State.Registers[alternate2]) = (input.State.Registers[alternate2], input.State.Registers[register2]);

        // Flags unaffected

        return true;
    }

    private static bool ADD_RR_RR(Input input, Register target, Register operand)
    {
        unchecked
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

        return true;
    }

    public static bool LD_R_addr_RR(Input input, Register target, Register source)
    {
        input.State.Registers[target] = input.Ram[input.State.Registers.ReadPair(source)];

        // Flags unaffected

        return true;
    }

    private static bool DEC_RR(Input input, Register register)
    {
        var result = input.State.Registers.ReadPair(register);

        result--;

        input.State.Registers.WritePair(register, result);

        // Flags unaffected

        return true;
    }

    private static bool RRCA(Input input)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[Register.A] & 0x01;

            var result = (byte) (input.State.Registers[Register.A] >> 1);

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

        return true;
    }

    private static bool DJNZ_e(Input input)
    {
        unchecked
        {
            // TODO: If B != 0, 5 more cycles... how to do this?
            DEC_R(input, Register.B);

            if (! input.State.Flags.Zero)
            {
                input.State.ProgramCounter += (sbyte) input.Data[1];
            }

            // Flags unaffected
        }

        return true;
    }

    private static bool RLA(Input input)
    {
        unchecked
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

        return true;
    }

    public static bool JR_e(Input input)
    {
        unchecked
        {
            input.State.ProgramCounter += (sbyte) input.Data[1];

            input.State.ProgramCounter = (ushort) input.State.ProgramCounter;
        }

        return true;
    }

    private static bool RRA(Input input)
    {
        unchecked
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

        return true;
    }

    private static bool JR_NZ_e(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool LD_addr_nn_RR(Input input, Register register)
    {
        unchecked
        {
            var address = input.Data[2] << 8 | input.Data[1];

            var data = input.State.Registers.ReadPair(register);

            input.Ram[address] = (byte) (data & 0x00FF);
            input.Ram[address + 1] = (byte) ((data & 0xFF00) >> 8);

            // Flags unaffected
        }

        return true;
    }

    // TODO: Lol, good luck adding a unit test for this one!
    private static bool DAA(Input input)
    {
        var adjust = 0;

        if (input.State.Flags.HalfCarry || (input.State.Registers[Register.A] & 0x0F) > 0x09)
        {
            adjust++;
        }

        if (input.State.Flags.Carry || input.State.Registers[Register.A] > 0x99)
        {
            adjust += 2;

            input.State.Flags.Carry = true;
        }

        if (input.State.Flags.AddSubtract && ! input.State.Flags.HalfCarry)
        {
            input.State.Flags.HalfCarry = false;
        }
        else
        {
            if (input.State.Flags.AddSubtract && input.State.Flags.HalfCarry)
            {
                input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < 0x06;
            }
            else
            {
                input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) >= 0x0A;
            }
        }

        switch (adjust)
        {
            case 1:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0xFA : 0x06);

                break;
            case 2:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0xA0 : 0x60);

                break;
            case 3:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0x9A : 0x66);

                break;
        }

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = true;
        // TODO: ParityOverflow
        input.State.Flags.X1 = (input.State.Registers[Register.A] & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (input.State.Registers[Register.A] & 0x20) > 0;
        input.State.Flags.Zero = input.State.Registers[Register.A] == 0;
        input.State.Flags.Sign = (input.State.Registers[Register.A] & 0x80) > 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool JR_Z_e(Input input)
    {
        if (input.State.Flags.Zero)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool LD_RR_addr_nn(Input input, Register register)
    {
        unchecked
        {
            var address = input.Data[2] << 8 | input.Data[1];

            input.State.Registers.WritePair(register, (ushort) (input.Ram[address + 1] << 8 | input.Ram[address]));

            // Flags unaffected
        }

        return true;
    }

    private static bool CPL(Input input)
    {
        unchecked
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

        return true;
    }

    private static bool JR_NC_e(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool LD_SP_nn(Input input)
    {
        input.State.StackPointer = input.Data[2] << 8 | input.Data[1];

        // Flags unaffected

        return true;
    }

    private static bool INC_SP(Input input)
    {
        input.State.StackPointer++;

        // Flags unaffected

        return true;
    }

    private static bool INC_addr_RR(Input input, Register register)
    {
        unchecked
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

        return true;
    }

    private static bool DEC_addr_RR(Input input, Register register)
    {
        unchecked
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

        return true;
    }

    private static bool LD_addr_RR_n(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.Data[1];

        // Flags unaffected

        return true;
    }

    private static bool SCF(Input input)
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

        return true;
    }

    private static bool JR_C_e(Input input)
    {
        if (input.State.Flags.Carry)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool ADD_RR_SP(Input input, Register register)
    {
        unchecked
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

        return true;
    }

    private static bool DEC_SP(Input input)
    {
        input.State.StackPointer--;

        // Flags unaffected

        return true;
    }

    private static bool CCF(Input input)
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

        return true;
    }

    private static bool LD_R_R(Input input, Register destination, Register source)
    {
        input.State.Registers[destination] = input.State.Registers[source];

        // Flags unaffected

        return true;
    }

    private static bool HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected

        return true;
    }

    public static bool ADD_R_R(Input input, Register destination, Register source)
    {
        unchecked
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

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADD_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
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

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADC_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADC_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SUB_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SUB_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SBC_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SBC_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool AND_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool AND_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool XOR_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool XOR_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool OR_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool OR_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool CP_R_R(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.State.Registers[right];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool CP_R_addr_RR(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.Ram[input.State.Registers.ReadPair(right)];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_NZ(Input input)
    {
        // TODO: If condition true, 6 more cycles required.
        if (! input.State.Flags.Zero)
        {
            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool POP_RR(Input input, Register register)
    {
        var data = (ushort) input.Ram[input.State.StackPointer];

        input.State.StackPointer++;

        data |= (ushort) (input.Ram[input.State.StackPointer] << 8);

        input.State.StackPointer++;

        input.State.Registers.WritePair(register, data);

        // Flags unaffected

        return true;
    }

    private static bool JP_NZ_nn(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_nn(Input input)
    {
        // TODO: Don't like this - 3 thing... maybe return true/false to indicate whether PC should be adjusted by caller...
        input.State.ProgramCounter = (input.Data[2] << 8 | input.Data[1]) - 3;

        // Flags unaffected

        return true;
    }

    private static bool CALL_NZ_nn(Input input)
    {
        // TODO: If condition true, 7 more cycles required.
        if (! input.State.Flags.Zero)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_nn(Input input)
    {
        unchecked
        {
            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) (((input.State.ProgramCounter + 3) & 0xFF00) >> 8);

            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) ((input.State.ProgramCounter + 3) & 0x00FF);

            // TODO: Remove -3 and return false
            input.State.ProgramCounter = (input.Data[2] << 8 | input.Data[1]) - 3;

            // Flags unaffected
        }

        return true;
    }

    private static bool PUSH_RR(Input input, Register register)
    {
        unchecked
        {
            input.State.StackPointer--;

            var data = input.State.Registers.ReadPair(register);

            input.Ram[input.State.StackPointer] = (byte) ((data & 0xFF00) >> 8);

            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) (data & 0x00FF);

            // Flags unaffected
        }

        return true;
    }

    private static bool ADD_R_n(Input input, Register register)
    {
        unchecked
        {
            var original = input.State.Registers[register];

            var result = input.State.Registers[register] + input.Data[1];

            input.State.Registers[Register.A] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (original & 0x0F) < (result & 0x10);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RST(Input input, byte pageZeroAddress)
    {
        var pc = input.State.ProgramCounter + 1;

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) ((pc & 0xFF00) >> 8);

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) (pc & 0x00FF);

        input.State.ProgramCounter = pageZeroAddress;

        // Flags unaffected

        return false;
    }

    private static bool RET_Z(Input input)
    {
        if (input.State.Flags.Zero)
        {
            // TODO: Same old... more cycles if condition met.

            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET(Input input)
    {
        var spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0xFF00) | spContent;

        input.State.StackPointer++;

        spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0x00FF) | spContent << 8;

        input.State.StackPointer++;

        input.State.ProgramCounter--;

        // Flags unaffected

        return true;
    }

    private static bool JP_Z_nn(Input input)
    {
        if (input.State.Flags.Zero)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_Z_nn(Input input)
    {
        // TODO: If condition true, 7 more cycles required.
        if (input.State.Flags.Zero)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    public static bool ADC_R_n(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Data[1];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_NC(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool OUT_addr_n_R(Input input, Register register)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return true;
    }

    private static bool CALL_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool SUB_R_n(Input input, Register register)
    {
        unchecked
        {
            var valueD = input.State.Registers[register];

            var valueS = input.Data[1];

            var result = valueD - valueS;

            input.State.Registers[register] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_C(Input input)
    {
        if (input.State.Flags.Carry)
        {
            // TODO: Same old... more cycles if condition met.

            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool EXX(Input input)
    {
        var bc = input.State.Registers.ReadPair(Register.BC);

        var de = input.State.Registers.ReadPair(Register.DE);

        var hl = input.State.Registers.ReadPair(Register.HL);

        input.State.Registers.WritePair(Register.BC, input.State.Registers.ReadPair(Register.BC1));

        input.State.Registers.WritePair(Register.DE, input.State.Registers.ReadPair(Register.DE1));

        input.State.Registers.WritePair(Register.HL, input.State.Registers.ReadPair(Register.HL1));

        input.State.Registers.WritePair(Register.BC1, bc);

        input.State.Registers.WritePair(Register.DE1, de);

        input.State.Registers.WritePair(Register.HL1, hl);

        // Flags unaffected

        return true;
    }

    private static bool JP_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool IN_R_addr_N(Input input)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return true;
    }

    private static bool CALL_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool SBC_R_n(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Data[1];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_PO(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool EX_addr_SP_RR(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WriteLow(register, input.Ram[input.State.StackPointer + 1]);

        input.State.Registers.WriteHigh(register, input.Ram[input.State.StackPointer]);

        input.Ram[input.State.StackPointer] = (byte) (value & 0x00FF);

        input.Ram[input.State.StackPointer + 1] = (byte) ((value & 0xFF00) >> 8);

        // Flags unaffected

        return true;
    }

    private static bool CALL_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool AND_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_PE(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            return RET(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool JP_addr_RR(Input input, Register register)
    {
        input.State.ProgramCounter = input.State.Registers.ReadPair(register);
        
        // Flags unaffected

        return false;
    }

    private static bool JP_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            return JP_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool EX_RR_RR(Input input, Register left, Register right)
    {
        var swap = input.State.Registers.ReadPair(left);

        input.State.Registers.WritePair(left, input.State.Registers.ReadPair(right));

        input.State.Registers.WritePair(right, swap);
        
        // Flags unaffected

        return true;
    }

    private static bool CALL_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool XOR_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_NS(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool DI(Input input)
    {
        // TODO: Disable maskable interrupt.
        return true;
    }

    private static bool CALL_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool OR_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RET_S(Input input)
    {
        if (input.State.Flags.Sign)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool LD_RR_RR(Input input)
    {
        input.State.StackPointer = input.State.Registers.ReadPair(Register.HL);

        // Flags unaffected

        return true;
    }

    private static bool JP_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool EI(Input input)
    {
        // TODO: Enable maskable interrupt.
        return true;
    }

    private static bool CALL_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CP_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] - input.Data[1];

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool INC_RRh(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) (byte) ((value & 0xFF00) + 1);

            result += (byte) (value & 0x00FF);
            
            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;
        }

        return true;
    }

    private static bool DEC_RRh(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) ((value & 0xFF00) - 1);

            result += (byte) (value & 0x00FF);
            
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

        return true;
    }

    private static bool LD_RRh_n(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WritePair(register, (ushort) ((input.Data[1] << 8) | (value & 0x00FF)));
        
        // Flags unaffected

        return true;
    }

    private static bool INC_RRl(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) (byte) ((value & 0x00FF) + 1);

            result += (byte) (value & 0xFF00);
            
            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;
        }

        return true;
    }

    private static bool DEC_RRl(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) (byte) ((value & 0x00FF) - 1);

            result += (byte) (value & 0xFF00);
            
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

        return true;
    }

    private static bool LD_RRl_n(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WritePair(register, (ushort) (input.Data[1] | (value & 0xFF00)));
        
        // Flags unaffected

        return true;
    }

    private static bool INC_addr_RR_plus_d(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address]++;
        
        // Flags unaffected

        return true;
    }

    private static bool DEC_addr_RR_plus_d(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address]--;
        
        // Flags unaffected

        return true;
    }

    private static bool LD_addr_RR_plus_d_n(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address] = input.Data[2];

        // Flags unaffected

        return true;
    }

    private static bool LD_R_RRh(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(source);

        input.State.Registers[destination] = (byte) ((value & 0xFF00) >> 8);
        
        // Flags unaffected

        return true;
    }

    private static bool LD_R_RRl(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(source);

        input.State.Registers[destination] = (byte) (value & 0x00FF);
        
        // Flags unaffected

        return true;
    }

    private static bool LD_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        var address = (int) input.State.Registers.ReadPair(source);

        address += (sbyte) input.Data[1];

        input.State.Registers[destination] = input.Ram[address];

        // Flags unaffected
        return true;
    }

    private static bool LD_addr_RR_plus_d_R(Input input, Register destination, Register source)
    {
        var address = (int) input.State.Registers.ReadPair(destination);

        address += (sbyte) input.Data[1];

        input.Ram[address] = input.State.Registers[source];

        // Flags unaffected

        return true;
    }

    private static bool LD_RRh_R(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(destination);

        value = (ushort) ((value & 0x00FF) | (input.State.Registers[source] << 8));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool LD_RRh_RRh(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0x00FF) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool LD_RRh_RRl(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0x00FF) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool LD_RRl_R(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(destination);

        value = (ushort) ((value & 0xFF00) | input.State.Registers[source]);

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool LD_RRl_RRh(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0xFF00) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool LD_RRl_RRl(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);
        
        var value = (ushort) ((left & 0xFF00) | (right & 0x00FF));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    private static bool ADD_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

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

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADD_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

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

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADD_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var address = (int) input.State.Registers.ReadPair(source);

            address += (sbyte) input.Data[1];

            var valueS = input.Ram[address];

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

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADC_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADC_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool ADC_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var address = (int) input.State.Registers.ReadPair(source);

            address += (sbyte) input.Data[1];

            var valueS = input.Ram[address];
            
            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SUB_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SUB_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SUB_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SBC_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SBC_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SBC_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool AND_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool AND_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool AND_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool XOR_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ ((input.State.Registers.ReadPair(source) & 0xFF00) >> 8);

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool XOR_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool XOR_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool OR_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | ((input.State.Registers.ReadPair(source) & 0xFF00) >> 8);

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool OR_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool OR_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool CP_R_RRh(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = (input.State.Registers.ReadPair(right) & 0xFF00) >> 8;

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool CP_R_RRl(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.State.Registers.ReadPair(right) & 0x00FF;

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool CP_R_addr_RR_plus_d(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.Ram[input.State.Registers.ReadPair(right) + (sbyte) input.Data[1]];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool LD_SP_RR(Input input, Register register)
    {
        input.State.StackPointer = input.State.Registers.ReadPair(register);

        // Flags unaffected

        return true;
    }

    private static bool SBC_RR_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.Registers.ReadPair(source);

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers.WritePair(destination, (ushort) result);

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool NEG(Input input)
    {
        unchecked
        {
            var value = input.State.Registers[Register.A];

            var result = (byte) -(sbyte) value;

            input.State.Registers[Register.A] = result;

            // Flags
            input.State.Flags.Carry = value != 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = value == 0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + ((~value + 1) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RLC_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (byte) ((input.State.Registers[register] & 0x80) >> 7);

            var result = (byte) (((input.State.Registers[register] << 1) & 0xFE) | topBit);

            input.State.Registers[register] = result;

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

        return true;
    }

    private static bool RLC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (byte) (data >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

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

        return true;
    }

    private static bool RRC_R(Input input, Register register)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[register] & 0x01;

            var result = (byte) (input.State.Registers[register] >> 1);

            if (bottomBit == 1)
            {
                result |= 0x80;
            }

            input.State.Registers[register] = result;

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

        return true;
    }

    private static bool RRC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var result = (byte) (data >> 1);

            if (bottomBit == 1)
            {
                result |= 0x80;
            }

            input.Ram[input.State.Registers.ReadPair(register)] = result;

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

        return true;
    }

    private static bool RL_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (input.State.Registers[register] & 0x80) >> 7;

            var result = (byte) (input.State.Registers[register] << 1);

            result |= (byte) (input.State.Flags.Carry ? 1 : 0);

            input.State.Registers[register] = result;

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

        return true;
    }

    private static bool RL_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            result |= (byte) (input.State.Flags.Carry ? 1 : 0);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

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

        return true;
    }
}