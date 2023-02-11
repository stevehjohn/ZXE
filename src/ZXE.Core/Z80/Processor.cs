using Microsoft.Win32;
using ZXE.Core.Exceptions;
using ZXE.Core.Extensions;
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
        instructions[0xCB] = new Instruction("SOPSET 0xCB", 1, _ => SetOpcodePrefix(0xCB), 4);

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
        instructions[0xDD] = new Instruction("SOPSET 0xDD", 1, _ => SetOpcodePrefix(0xDD), 4);

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
        instructions[0xED] = new Instruction("SOPSET 0xED", 1, _ => SetOpcodePrefix(0xED), 4);

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
        instructions[0xFD] = new Instruction("SOPSET 0xFD", 1, _ => SetOpcodePrefix(0xFD), 4);

        instructions[0xFE] = new Instruction("CP A, n", 2, i => CP_R_n(i, Register.A), 7);

        instructions[0xFF] = new Instruction("RST 0x38", 1, i => RST(i, 0x38), 11);
    }

    private void InitialiseDDInstructions(Dictionary<int, Instruction> instructions)
    {
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

        instructions[0xDDCB] = new Instruction("SOPSET 0xDDCB", 1, _ => SetOpcodePrefix(0xDDCB), 4);

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

    // TODO: Here be dragons... these are the more complicated (or emulator complicating) instructions...
    private static void InitialiseEDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xED00] = new Instruction("IN_0 B, (n)", 2, i => IN_b_R_addr_n(i, Register.B), 8, null, 0x0ED00);

        instructions[0xED01] = new Instruction("OUT_0 (n), B", 2, i => OUT_b_addr_n_R(i, Register.B), 8, null, 0xED01);

        instructions[0xED04] = new Instruction("TST B", 1, i => TST_R(i, Register.B), 6, null, 0xED04);

        instructions[0xED08] = new Instruction("IN_0 C, (n)", 2, i => IN_b_R_addr_n(i, Register.C), 8, null, 0x0ED08);

        instructions[0xED09] = new Instruction("OUT_0 (n), C", 2, i => OUT_b_addr_n_R(i, Register.C), 8, null, 0xED09);

        instructions[0xED0C] = new Instruction("TST C", 1, i => TST_R(i, Register.C), 6, null, 0xED0C);
        
        instructions[0xED10] = new Instruction("IN_0 D, (n)", 2, i => IN_b_R_addr_n(i, Register.D), 8, null, 0x0ED10);

        instructions[0xED11] = new Instruction("OUT_0 (n), D", 2, i => OUT_b_addr_n_R(i, Register.D), 8, null, 0xED11);

        instructions[0xED14] = new Instruction("TST D", 1, i => TST_R(i, Register.B), 6, null, 0xED14);

        instructions[0xED18] = new Instruction("IN_0 E, (n)", 2, i => IN_b_R_addr_n(i, Register.E), 8, null, 0x0ED18);

        instructions[0xED19] = new Instruction("OUT_0 (n), E", 2, i => OUT_b_addr_n_R(i, Register.E), 8, null, 0xED19);

        instructions[0xED1C] = new Instruction("TST E", 1, i => TST_R(i, Register.E), 6, null, 0xED1C);

        instructions[0xED20] = new Instruction("IN_0 H, (n)", 2, i => IN_b_R_addr_n(i, Register.H), 8, null, 0x0ED20);

        instructions[0xED21] = new Instruction("OUT_0 (n), H", 2, i => OUT_b_addr_n_R(i, Register.H), 8, null, 0xED21);

        instructions[0xED24] = new Instruction("TST H", 1, i => TST_R(i, Register.H), 6, null, 0xED24);

        instructions[0xED28] = new Instruction("IN_0 L, (n)", 2, i => IN_b_R_addr_n(i, Register.L), 8, null, 0x0ED28);

        instructions[0xED29] = new Instruction("OUT_0 (n), L", 2, i => OUT_b_addr_n_R(i, Register.L), 8, null, 0xED29);

        instructions[0xED2C] = new Instruction("TST L", 1, i => TST_R(i, Register.L), 6, null, 0xED2C);

        instructions[0xED34] = new Instruction("TST (HL)", 1, i => TST_addr_R(i, Register.HL), 6, null, 0xED34);

        instructions[0xED38] = new Instruction("IN_0 A, (n)", 2, i => IN_b_R_addr_n(i, Register.A), 8, null, 0x0ED38);

        instructions[0xED39] = new Instruction("OUT_0 (n), A", 2, i => OUT_b_addr_n_R(i, Register.A), 8, null, 0xED39);

        instructions[0xED3C] = new Instruction("TST A", 1, i => TST_R(i, Register.A), 6, null, 0xED3C);

        instructions[0xED40] = new Instruction("IN B, (BC)", 1, i => IN_R_addr_R(i, Register.B, Register.BC), 8, null, 0xED40);

        instructions[0xED41] = new Instruction("OUT (BC), B", 1, i => OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED41);

        instructions[0xED42] = new Instruction("SBC HL, BC", 1, i => SBC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED42);

        instructions[0xED43] = new Instruction("LD (nn), BC", 3, i => LD_addr_nn_RR(i, Register.BC), 16, null, 0xED43);

        instructions[0xED44] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED44);

        // TODO: instructions[0xED45] = new Instruction("RETN", 1, i => IM_m(i, InterruptMode.Mode0), 4, null, 0xED45);

        instructions[0xED46] = new Instruction("IM 0", 1, i => IM_m(i, InterruptMode.Mode0), 4, null, 0xED46);

        instructions[0xED47] = new Instruction("LD I, A", 1, i => LD_R_R(i, Register.I, Register.A), 5, null, 0xED47);

        instructions[0xED48] = new Instruction("IN C, (BC)", 1, i => IN_R_addr_R(i, Register.C, Register.BC), 8, null, 0xED48);

        instructions[0xED49] = new Instruction("OUT (BC), B", 1, i => OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED49);

        instructions[0xED4A] = new Instruction("ADC HL, BC", 1, i => ADC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED4A);
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

        instructions[0xCB18] = new Instruction("RR B", 1, i => RR_R(i, Register.B), 4);

        instructions[0xCB19] = new Instruction("RR C", 1, i => RR_R(i, Register.C), 4);

        instructions[0xCB1A] = new Instruction("RR D", 1, i => RR_R(i, Register.D), 4);

        instructions[0xCB1B] = new Instruction("RR E", 1, i => RR_R(i, Register.E), 4);

        instructions[0xCB1C] = new Instruction("RR H", 1, i => RR_R(i, Register.H), 4);

        instructions[0xCB1D] = new Instruction("RR L", 1, i => RR_R(i, Register.L), 4);

        instructions[0xCB1E] = new Instruction("RR (HL)", 1, i => RR_addr_RR(i, Register.HL), 11);

        instructions[0xCB1F] = new Instruction("RR A", 1, i => RR_R(i, Register.A), 4);

        instructions[0xCB20] = new Instruction("SLA B", 1, i => SLA_R(i, Register.B), 4);

        instructions[0xCB21] = new Instruction("SLA C", 1, i => SLA_R(i, Register.C), 4);

        instructions[0xCB22] = new Instruction("SLA D", 1, i => SLA_R(i, Register.D), 4);

        instructions[0xCB23] = new Instruction("SLA E", 1, i => SLA_R(i, Register.E), 4);

        instructions[0xCB24] = new Instruction("SLA H", 1, i => SLA_R(i, Register.H), 4);

        instructions[0xCB25] = new Instruction("SLA L", 1, i => SLA_R(i, Register.L), 4);

        instructions[0xCB26] = new Instruction("SLA (HL)", 1, i => SLA_addr_RR(i, Register.HL), 11);

        instructions[0xCB27] = new Instruction("SLA A", 1, i => SLA_R(i, Register.A), 4);

        instructions[0xCB28] = new Instruction("SRA B", 1, i => SRA_R(i, Register.B), 4);

        instructions[0xCB29] = new Instruction("SRA C", 1, i => SRA_R(i, Register.C), 4);

        instructions[0xCB2A] = new Instruction("SRA D", 1, i => SRA_R(i, Register.D), 4);

        instructions[0xCB2B] = new Instruction("SRA E", 1, i => SRA_R(i, Register.E), 4);

        instructions[0xCB2C] = new Instruction("SRA H", 1, i => SRA_R(i, Register.H), 4);

        instructions[0xCB2D] = new Instruction("SRA L", 1, i => SRA_R(i, Register.L), 4);

        instructions[0xCB2E] = new Instruction("SRA (HL)", 1, i => SRA_addr_RR(i, Register.HL), 11);

        instructions[0xCB2F] = new Instruction("SRA A", 1, i => SRA_R(i, Register.A), 4);

        instructions[0xCB30] = new Instruction("SLS B", 1, i => SLS_R(i, Register.B), 4);

        instructions[0xCB31] = new Instruction("SLS C", 1, i => SLS_R(i, Register.C), 4);

        instructions[0xCB32] = new Instruction("SLS D", 1, i => SLS_R(i, Register.D), 4);

        instructions[0xCB33] = new Instruction("SLS E", 1, i => SLS_R(i, Register.E), 4);

        instructions[0xCB34] = new Instruction("SLS H", 1, i => SLS_R(i, Register.H), 4);

        instructions[0xCB35] = new Instruction("SLS L", 1, i => SLS_R(i, Register.L), 4);

        instructions[0xCB36] = new Instruction("SLS (HL)", 1, i => SLS_addr_RR(i, Register.HL), 11);

        instructions[0xCB37] = new Instruction("SLS A", 1, i => SLS_R(i, Register.A), 4);

        instructions[0xCB38] = new Instruction("SRL B", 1, i => SRL_R(i, Register.B), 4);

        instructions[0xCB39] = new Instruction("SRL C", 1, i => SRL_R(i, Register.C), 4);

        instructions[0xCB3A] = new Instruction("SRL D", 1, i => SRL_R(i, Register.D), 4);

        instructions[0xCB3B] = new Instruction("SRL E", 1, i => SRL_R(i, Register.E), 4);

        instructions[0xCB3C] = new Instruction("SRL H", 1, i => SRL_R(i, Register.H), 4);

        instructions[0xCB3D] = new Instruction("SRL L", 1, i => SRL_R(i, Register.L), 4);

        instructions[0xCB3E] = new Instruction("SRL (HL)", 1, i => SRL_addr_RR(i, Register.HL), 11);

        instructions[0xCB3F] = new Instruction("SRL A", 1, i => SRL_R(i, Register.A), 4);

        instructions[0xCB40] = new Instruction("BIT 0, B", 1, i => BIT_b_R(i, 0x01, Register.B), 4);

        instructions[0xCB41] = new Instruction("BIT 0, C", 1, i => BIT_b_R(i, 0x01, Register.C), 4);

        instructions[0xCB42] = new Instruction("BIT 0, D", 1, i => BIT_b_R(i, 0x01, Register.D), 4);

        instructions[0xCB43] = new Instruction("BIT 0, E", 1, i => BIT_b_R(i, 0x01, Register.E), 4);

        instructions[0xCB44] = new Instruction("BIT 0, H", 1, i => BIT_b_R(i, 0x01, Register.H), 4);

        instructions[0xCB45] = new Instruction("BIT 0, L", 1, i => BIT_b_R(i, 0x01, Register.L), 4);

        instructions[0xCB46] = new Instruction("BIT 0, (HL)", 1, i => BIT_b_addr_RR(i, 0x01, Register.HL), 8);
        
        instructions[0xCB47] = new Instruction("BIT 0, A", 1, i => BIT_b_R(i, 0x01, Register.A), 4);

        instructions[0xCB48] = new Instruction("BIT 1, B", 1, i => BIT_b_R(i, 0x02, Register.B), 4);

        instructions[0xCB49] = new Instruction("BIT 1, C", 1, i => BIT_b_R(i, 0x02, Register.C), 4);

        instructions[0xCB4A] = new Instruction("BIT 1, D", 1, i => BIT_b_R(i, 0x02, Register.D), 4);

        instructions[0xCB4B] = new Instruction("BIT 1, E", 1, i => BIT_b_R(i, 0x02, Register.E), 4);

        instructions[0xCB4C] = new Instruction("BIT 1, H", 1, i => BIT_b_R(i, 0x02, Register.H), 4);

        instructions[0xCB4D] = new Instruction("BIT 1, L", 1, i => BIT_b_R(i, 0x02, Register.L), 4);

        instructions[0xCB4E] = new Instruction("BIT 1, (HL)", 1, i => BIT_b_addr_RR(i, 0x02, Register.HL), 8);
        
        instructions[0xCB4F] = new Instruction("BIT 1, A", 1, i => BIT_b_R(i, 0x02, Register.A), 4);

        instructions[0xCB50] = new Instruction("BIT 2, B", 1, i => BIT_b_R(i, 0x04, Register.B), 4);

        instructions[0xCB51] = new Instruction("BIT 2, C", 1, i => BIT_b_R(i, 0x04, Register.C), 4);

        instructions[0xCB52] = new Instruction("BIT 2, D", 1, i => BIT_b_R(i, 0x04, Register.D), 4);

        instructions[0xCB53] = new Instruction("BIT 2, E", 1, i => BIT_b_R(i, 0x04, Register.E), 4);

        instructions[0xCB54] = new Instruction("BIT 2, H", 1, i => BIT_b_R(i, 0x04, Register.H), 4);

        instructions[0xCB55] = new Instruction("BIT 2, L", 1, i => BIT_b_R(i, 0x04, Register.L), 4);

        instructions[0xCB56] = new Instruction("BIT 2, (HL)", 1, i => BIT_b_addr_RR(i, 0x04, Register.HL), 8);
        
        instructions[0xCB57] = new Instruction("BIT 2, A", 1, i => BIT_b_R(i, 0x04, Register.A), 4);

        instructions[0xCB58] = new Instruction("BIT 3, B", 1, i => BIT_b_R(i, 0x08, Register.B), 4);

        instructions[0xCB59] = new Instruction("BIT 3, C", 1, i => BIT_b_R(i, 0x08, Register.C), 4);

        instructions[0xCB5A] = new Instruction("BIT 3, D", 1, i => BIT_b_R(i, 0x08, Register.D), 4);

        instructions[0xCB5B] = new Instruction("BIT 3, E", 1, i => BIT_b_R(i, 0x08, Register.E), 4);

        instructions[0xCB5C] = new Instruction("BIT 3, H", 1, i => BIT_b_R(i, 0x08, Register.H), 4);

        instructions[0xCB5D] = new Instruction("BIT 3, L", 1, i => BIT_b_R(i, 0x08, Register.L), 4);

        instructions[0xCB5E] = new Instruction("BIT 3, (HL)", 1, i => BIT_b_addr_RR(i, 0x08, Register.HL), 8);
        
        instructions[0xCB5F] = new Instruction("BIT 3, A", 1, i => BIT_b_R(i, 0x08, Register.A), 4);

        instructions[0xCB60] = new Instruction("BIT 4, B", 1, i => BIT_b_R(i, 0x10, Register.B), 4);

        instructions[0xCB61] = new Instruction("BIT 4, C", 1, i => BIT_b_R(i, 0x10, Register.C), 4);

        instructions[0xCB62] = new Instruction("BIT 4, D", 1, i => BIT_b_R(i, 0x10, Register.D), 4);

        instructions[0xCB63] = new Instruction("BIT 4, E", 1, i => BIT_b_R(i, 0x10, Register.E), 4);

        instructions[0xCB64] = new Instruction("BIT 4, H", 1, i => BIT_b_R(i, 0x10, Register.H), 4);

        instructions[0xCB65] = new Instruction("BIT 4, L", 1, i => BIT_b_R(i, 0x10, Register.L), 4);

        instructions[0xCB66] = new Instruction("BIT 4, (HL)", 1, i => BIT_b_addr_RR(i, 0x10, Register.HL), 8);
        
        instructions[0xCB67] = new Instruction("BIT 4, A", 1, i => BIT_b_R(i, 0x10, Register.A), 4);
        
        instructions[0xCB68] = new Instruction("BIT 5, B", 1, i => BIT_b_R(i, 0x20, Register.B), 4);

        instructions[0xCB69] = new Instruction("BIT 5, C", 1, i => BIT_b_R(i, 0x20, Register.C), 4);

        instructions[0xCB6A] = new Instruction("BIT 5, D", 1, i => BIT_b_R(i, 0x20, Register.D), 4);

        instructions[0xCB6B] = new Instruction("BIT 5, E", 1, i => BIT_b_R(i, 0x20, Register.E), 4);

        instructions[0xCB6C] = new Instruction("BIT 5, H", 1, i => BIT_b_R(i, 0x20, Register.H), 4);

        instructions[0xCB6D] = new Instruction("BIT 5, L", 1, i => BIT_b_R(i, 0x20, Register.L), 4);

        instructions[0xCB6E] = new Instruction("BIT 5, (HL)", 1, i => BIT_b_addr_RR(i, 0x20, Register.HL), 8);
        
        instructions[0xCB6F] = new Instruction("BIT 5, A", 1, i => BIT_b_R(i, 0x20, Register.A), 4);
        
        instructions[0xCB70] = new Instruction("BIT 6, B", 1, i => BIT_b_R(i, 0x40, Register.B), 4);

        instructions[0xCB71] = new Instruction("BIT 6, C", 1, i => BIT_b_R(i, 0x40, Register.C), 4);

        instructions[0xCB72] = new Instruction("BIT 6, D", 1, i => BIT_b_R(i, 0x40, Register.D), 4);

        instructions[0xCB73] = new Instruction("BIT 6, E", 1, i => BIT_b_R(i, 0x40, Register.E), 4);

        instructions[0xCB74] = new Instruction("BIT 6, H", 1, i => BIT_b_R(i, 0x40, Register.H), 4);

        instructions[0xCB75] = new Instruction("BIT 6, L", 1, i => BIT_b_R(i, 0x40, Register.L), 4);

        instructions[0xCB76] = new Instruction("BIT 6, (HL)", 1, i => BIT_b_addr_RR(i, 0x40, Register.HL), 8);
        
        instructions[0xCB77] = new Instruction("BIT 6, A", 1, i => BIT_b_R(i, 0x40, Register.A), 4);
        
        instructions[0xCB78] = new Instruction("BIT 7, B", 1, i => BIT_b_R(i, 0x80, Register.B), 4);

        instructions[0xCB79] = new Instruction("BIT 7, C", 1, i => BIT_b_R(i, 0x80, Register.C), 4);

        instructions[0xCB7A] = new Instruction("BIT 7, D", 1, i => BIT_b_R(i, 0x80, Register.D), 4);

        instructions[0xCB7B] = new Instruction("BIT 7, E", 1, i => BIT_b_R(i, 0x80, Register.E), 4);

        instructions[0xCB7C] = new Instruction("BIT 7, H", 1, i => BIT_b_R(i, 0x80, Register.H), 4);

        instructions[0xCB7D] = new Instruction("BIT 7, L", 1, i => BIT_b_R(i, 0x80, Register.L), 4);

        instructions[0xCB7E] = new Instruction("BIT 7, (HL)", 1, i => BIT_b_addr_RR(i, 0x80, Register.HL), 8);
        
        instructions[0xCB7F] = new Instruction("BIT 7, A", 1, i => BIT_b_R(i, 0x80, Register.A), 4);
        
        instructions[0xCB80] = new Instruction("RES 0, B", 1, i => RES_b_R(i, 0x01, Register.B), 4, "RES B");

        instructions[0xCB81] = new Instruction("RES 0, C", 1, i => RES_b_R(i, 0x01, Register.C), 4, "RES C");

        instructions[0xCB82] = new Instruction("RES 0, D", 1, i => RES_b_R(i, 0x01, Register.D), 4, "RES D");

        instructions[0xCB83] = new Instruction("RES 0, E", 1, i => RES_b_R(i, 0x01, Register.E), 4, "RES E");

        instructions[0xCB84] = new Instruction("RES 0, H", 1, i => RES_b_R(i, 0x01, Register.H), 4, "RES H");

        instructions[0xCB85] = new Instruction("RES 0, L", 1, i => RES_b_R(i, 0x01, Register.L), 4, "RES L");

        instructions[0xCB86] = new Instruction("RES 0, (HL)", 1, i => RES_b_addr_RR(i, 0x01, Register.HL), 11, "RES (HL)");

        instructions[0xCB87] = new Instruction("RES 0, A", 1, i => RES_b_R(i, 0x01, Register.A), 4, "RES A");
        
        instructions[0xCB88] = new Instruction("RES 1, B", 1, i => RES_b_R(i, 0x02, Register.B), 4, "RES B");

        instructions[0xCB89] = new Instruction("RES 1, C", 1, i => RES_b_R(i, 0x02, Register.C), 4, "RES C");

        instructions[0xCB8A] = new Instruction("RES 1, D", 1, i => RES_b_R(i, 0x02, Register.D), 4, "RES D");

        instructions[0xCB8B] = new Instruction("RES 1, E", 1, i => RES_b_R(i, 0x02, Register.E), 4, "RES E");

        instructions[0xCB8C] = new Instruction("RES 1, H", 1, i => RES_b_R(i, 0x02, Register.H), 4, "RES H");

        instructions[0xCB8D] = new Instruction("RES 1, L", 1, i => RES_b_R(i, 0x02, Register.L), 4, "RES L");

        instructions[0xCB8E] = new Instruction("RES 1, (HL)", 1, i => RES_b_addr_RR(i, 0x02, Register.HL), 11, "RES (HL)");

        instructions[0xCB8F] = new Instruction("RES 1, A", 1, i => RES_b_R(i, 0x02, Register.A), 4, "RES A");

        instructions[0xCB90] = new Instruction("RES 2, B", 1, i => RES_b_R(i, 0x04, Register.B), 4, "RES B");

        instructions[0xCB91] = new Instruction("RES 2, C", 1, i => RES_b_R(i, 0x04, Register.C), 4, "RES C");

        instructions[0xCB92] = new Instruction("RES 2, D", 1, i => RES_b_R(i, 0x04, Register.D), 4, "RES D");

        instructions[0xCB93] = new Instruction("RES 2, E", 1, i => RES_b_R(i, 0x04, Register.E), 4, "RES E");

        instructions[0xCB94] = new Instruction("RES 2, H", 1, i => RES_b_R(i, 0x04, Register.H), 4, "RES H");

        instructions[0xCB95] = new Instruction("RES 2, L", 1, i => RES_b_R(i, 0x04, Register.L), 4, "RES L");

        instructions[0xCB96] = new Instruction("RES 2, (HL)", 1, i => RES_b_addr_RR(i, 0x04, Register.HL), 11, "RES (HL)");

        instructions[0xCB97] = new Instruction("RES 2, A", 1, i => RES_b_R(i, 0x04, Register.A), 4, "RES A");

        instructions[0xCB98] = new Instruction("RES 3, B", 1, i => RES_b_R(i, 0x08, Register.B), 4, "RES B");

        instructions[0xCB99] = new Instruction("RES 3, C", 1, i => RES_b_R(i, 0x08, Register.C), 4, "RES C");

        instructions[0xCB9A] = new Instruction("RES 3, D", 1, i => RES_b_R(i, 0x08, Register.D), 4, "RES D");

        instructions[0xCB9B] = new Instruction("RES 3, E", 1, i => RES_b_R(i, 0x08, Register.E), 4, "RES E");

        instructions[0xCB9C] = new Instruction("RES 3, H", 1, i => RES_b_R(i, 0x08, Register.H), 4, "RES H");

        instructions[0xCB9D] = new Instruction("RES 3, L", 1, i => RES_b_R(i, 0x08, Register.L), 4, "RES L");

        instructions[0xCB9E] = new Instruction("RES 3, (HL)", 1, i => RES_b_addr_RR(i, 0x08, Register.HL), 11, "RES (HL)");

        instructions[0xCB9F] = new Instruction("RES 3, A", 1, i => RES_b_R(i, 0x08, Register.A), 4, "RES A");

        instructions[0xCBA0] = new Instruction("RES 4, B", 1, i => RES_b_R(i, 0x10, Register.B), 4, "RES B");

        instructions[0xCBA1] = new Instruction("RES 4, C", 1, i => RES_b_R(i, 0x10, Register.C), 4, "RES C");

        instructions[0xCBA2] = new Instruction("RES 4, D", 1, i => RES_b_R(i, 0x10, Register.D), 4, "RES D");

        instructions[0xCBA3] = new Instruction("RES 4, E", 1, i => RES_b_R(i, 0x10, Register.E), 4, "RES E");

        instructions[0xCBA4] = new Instruction("RES 4, H", 1, i => RES_b_R(i, 0x10, Register.H), 4, "RES H");

        instructions[0xCBA5] = new Instruction("RES 4, L", 1, i => RES_b_R(i, 0x10, Register.L), 4, "RES L");

        instructions[0xCBA6] = new Instruction("RES 4, (HL)", 1, i => RES_b_addr_RR(i, 0x10, Register.HL), 11, "RES (HL)");

        instructions[0xCBA7] = new Instruction("RES 4, A", 1, i => RES_b_R(i, 0x10, Register.A), 4, "RES A");

        instructions[0xCBA8] = new Instruction("RES 5, B", 1, i => RES_b_R(i, 0x20, Register.B), 4, "RES B");

        instructions[0xCBA9] = new Instruction("RES 5, C", 1, i => RES_b_R(i, 0x20, Register.C), 4, "RES C");

        instructions[0xCBAA] = new Instruction("RES 5, D", 1, i => RES_b_R(i, 0x20, Register.D), 4, "RES D");

        instructions[0xCBAB] = new Instruction("RES 5, E", 1, i => RES_b_R(i, 0x20, Register.E), 4, "RES E");

        instructions[0xCBAC] = new Instruction("RES 5, H", 1, i => RES_b_R(i, 0x20, Register.H), 4, "RES H");

        instructions[0xCBAD] = new Instruction("RES 5, L", 1, i => RES_b_R(i, 0x20, Register.L), 4, "RES L");

        instructions[0xCBAE] = new Instruction("RES 5, (HL)", 1, i => RES_b_addr_RR(i, 0x20, Register.HL), 11, "RES (HL)");

        instructions[0xCBAF] = new Instruction("RES 5, A", 1, i => RES_b_R(i, 0x20, Register.A), 4, "RES A");

        instructions[0xCBB0] = new Instruction("RES 6, B", 1, i => RES_b_R(i, 0x40, Register.B), 4, "RES B");

        instructions[0xCBB1] = new Instruction("RES 6, C", 1, i => RES_b_R(i, 0x40, Register.C), 4, "RES C");

        instructions[0xCBB2] = new Instruction("RES 6, D", 1, i => RES_b_R(i, 0x40, Register.D), 4, "RES D");

        instructions[0xCBB3] = new Instruction("RES 6, E", 1, i => RES_b_R(i, 0x40, Register.E), 4, "RES E");

        instructions[0xCBB4] = new Instruction("RES 6, H", 1, i => RES_b_R(i, 0x40, Register.H), 4, "RES H");

        instructions[0xCBB5] = new Instruction("RES 6, L", 1, i => RES_b_R(i, 0x40, Register.L), 4, "RES L");

        instructions[0xCBB6] = new Instruction("RES 6, (HL)", 1, i => RES_b_addr_RR(i, 0x40, Register.HL), 11, "RES (HL)");

        instructions[0xCBB7] = new Instruction("RES 6, A", 1, i => RES_b_R(i, 0x40, Register.A), 4, "RES A");

        instructions[0xCBB8] = new Instruction("RES 7, B", 1, i => RES_b_R(i, 0x80, Register.B), 4, "RES B");

        instructions[0xCBB9] = new Instruction("RES 7, C", 1, i => RES_b_R(i, 0x80, Register.C), 4, "RES C");

        instructions[0xCBBA] = new Instruction("RES 7, D", 1, i => RES_b_R(i, 0x80, Register.D), 4, "RES D");

        instructions[0xCBBB] = new Instruction("RES 7, E", 1, i => RES_b_R(i, 0x80, Register.E), 4, "RES E");

        instructions[0xCBBC] = new Instruction("RES 7, H", 1, i => RES_b_R(i, 0x80, Register.H), 4, "RES H");

        instructions[0xCBBD] = new Instruction("RES 7, L", 1, i => RES_b_R(i, 0x80, Register.L), 4, "RES L");

        instructions[0xCBBE] = new Instruction("RES 7, (HL)", 1, i => RES_b_addr_RR(i, 0x80, Register.HL), 11, "RES (HL)");

        instructions[0xCBBF] = new Instruction("RES 7, A", 1, i => RES_b_R(i, 0x80, Register.A), 4, "RES A");

        instructions[0xCBC0] = new Instruction("SET 0, B", 1, i => SET_b_R(i, 0x01, Register.B), 4, "SET B");

        instructions[0xCBC1] = new Instruction("SET 0, C", 1, i => SET_b_R(i, 0x01, Register.C), 4, "SET C");

        instructions[0xCBC2] = new Instruction("SET 0, D", 1, i => SET_b_R(i, 0x01, Register.D), 4, "SET D");

        instructions[0xCBC3] = new Instruction("SET 0, E", 1, i => SET_b_R(i, 0x01, Register.E), 4, "SET E");

        instructions[0xCBC4] = new Instruction("SET 0, H", 1, i => SET_b_R(i, 0x01, Register.H), 4, "SET H");

        instructions[0xCBC5] = new Instruction("SET 0, L", 1, i => SET_b_R(i, 0x01, Register.L), 4, "SET L");

        instructions[0xCBC6] = new Instruction("SET 0, (HL)", 1, i => SET_b_addr_RR(i, 0x01, Register.HL), 11, "SET (HL)");

        instructions[0xCBC7] = new Instruction("SET 0, A", 1, i => SET_b_R(i, 0x01, Register.A), 4, "SET A");
        
        instructions[0xCBC8] = new Instruction("SET 1, B", 1, i => SET_b_R(i, 0x02, Register.B), 4, "SET B");

        instructions[0xCBC9] = new Instruction("SET 1, C", 1, i => SET_b_R(i, 0x02, Register.C), 4, "SET C");

        instructions[0xCBCA] = new Instruction("SET 1, D", 1, i => SET_b_R(i, 0x02, Register.D), 4, "SET D");

        instructions[0xCBCB] = new Instruction("SET 1, E", 1, i => SET_b_R(i, 0x02, Register.E), 4, "SET E");

        instructions[0xCBCC] = new Instruction("SET 1, H", 1, i => SET_b_R(i, 0x02, Register.H), 4, "SET H");

        instructions[0xCBCD] = new Instruction("SET 1, L", 1, i => SET_b_R(i, 0x02, Register.L), 4, "SET L");

        instructions[0xCBCE] = new Instruction("SET 1, (HL)", 1, i => SET_b_addr_RR(i, 0x02, Register.HL), 11, "SET (HL)");

        instructions[0xCBCF] = new Instruction("SET 1, A", 1, i => SET_b_R(i, 0x02, Register.A), 4, "SET A");

        instructions[0xCBD0] = new Instruction("SET 2, B", 1, i => SET_b_R(i, 0x04, Register.B), 4, "SET B");

        instructions[0xCBD1] = new Instruction("SET 2, C", 1, i => SET_b_R(i, 0x04, Register.C), 4, "SET C");

        instructions[0xCBD2] = new Instruction("SET 2, D", 1, i => SET_b_R(i, 0x04, Register.D), 4, "SET D");

        instructions[0xCBD3] = new Instruction("SET 2, E", 1, i => SET_b_R(i, 0x04, Register.E), 4, "SET E");

        instructions[0xCBD4] = new Instruction("SET 2, H", 1, i => SET_b_R(i, 0x04, Register.H), 4, "SET H");

        instructions[0xCBD5] = new Instruction("SET 2, L", 1, i => SET_b_R(i, 0x04, Register.L), 4, "SET L");

        instructions[0xCBD6] = new Instruction("SET 2, (HL)", 1, i => SET_b_addr_RR(i, 0x04, Register.HL), 11, "SET (HL)");

        instructions[0xCBD7] = new Instruction("SET 2, A", 1, i => SET_b_R(i, 0x04, Register.A), 4, "SET A");

        instructions[0xCBD8] = new Instruction("SET 3, B", 1, i => SET_b_R(i, 0x08, Register.B), 4, "SET B");

        instructions[0xCBD9] = new Instruction("SET 3, C", 1, i => SET_b_R(i, 0x08, Register.C), 4, "SET C");

        instructions[0xCBDA] = new Instruction("SET 3, D", 1, i => SET_b_R(i, 0x08, Register.D), 4, "SET D");

        instructions[0xCBDB] = new Instruction("SET 3, E", 1, i => SET_b_R(i, 0x08, Register.E), 4, "SET E");

        instructions[0xCBDC] = new Instruction("SET 3, H", 1, i => SET_b_R(i, 0x08, Register.H), 4, "SET H");

        instructions[0xCBDD] = new Instruction("SET 3, L", 1, i => SET_b_R(i, 0x08, Register.L), 4, "SET L");

        instructions[0xCBDE] = new Instruction("SET 3, (HL)", 1, i => SET_b_addr_RR(i, 0x08, Register.HL), 11, "SET (HL)");

        instructions[0xCBDF] = new Instruction("SET 3, A", 1, i => SET_b_R(i, 0x08, Register.A), 4, "SET A");

        instructions[0xCBE0] = new Instruction("SET 4, B", 1, i => SET_b_R(i, 0x10, Register.B), 4, "SET B");

        instructions[0xCBE1] = new Instruction("SET 4, C", 1, i => SET_b_R(i, 0x10, Register.C), 4, "SET C");

        instructions[0xCBE2] = new Instruction("SET 4, D", 1, i => SET_b_R(i, 0x10, Register.D), 4, "SET D");

        instructions[0xCBE3] = new Instruction("SET 4, E", 1, i => SET_b_R(i, 0x10, Register.E), 4, "SET E");

        instructions[0xCBE4] = new Instruction("SET 4, H", 1, i => SET_b_R(i, 0x10, Register.H), 4, "SET H");

        instructions[0xCBE5] = new Instruction("SET 4, L", 1, i => SET_b_R(i, 0x10, Register.L), 4, "SET L");

        instructions[0xCBE6] = new Instruction("SET 4, (HL)", 1, i => SET_b_addr_RR(i, 0x10, Register.HL), 11, "SET (HL)");

        instructions[0xCBE7] = new Instruction("SET 4, A", 1, i => SET_b_R(i, 0x10, Register.A), 4, "SET A");

        instructions[0xCBE8] = new Instruction("SET 5, B", 1, i => SET_b_R(i, 0x20, Register.B), 4, "SET B");

        instructions[0xCBE9] = new Instruction("SET 5, C", 1, i => SET_b_R(i, 0x20, Register.C), 4, "SET C");

        instructions[0xCBEA] = new Instruction("SET 5, D", 1, i => SET_b_R(i, 0x20, Register.D), 4, "SET D");

        instructions[0xCBEB] = new Instruction("SET 5, E", 1, i => SET_b_R(i, 0x20, Register.E), 4, "SET E");

        instructions[0xCBEC] = new Instruction("SET 5, H", 1, i => SET_b_R(i, 0x20, Register.H), 4, "SET H");

        instructions[0xCBED] = new Instruction("SET 5, L", 1, i => SET_b_R(i, 0x20, Register.L), 4, "SET L");

        instructions[0xCBEE] = new Instruction("SET 5, (HL)", 1, i => SET_b_addr_RR(i, 0x20, Register.HL), 11, "SET (HL)");

        instructions[0xCBEF] = new Instruction("SET 5, A", 1, i => SET_b_R(i, 0x20, Register.A), 4, "SET A");

        instructions[0xCBF0] = new Instruction("SET 6, B", 1, i => SET_b_R(i, 0x40, Register.B), 4, "SET B");

        instructions[0xCBF1] = new Instruction("SET 6, C", 1, i => SET_b_R(i, 0x40, Register.C), 4, "SET C");

        instructions[0xCBF2] = new Instruction("SET 6, D", 1, i => SET_b_R(i, 0x40, Register.D), 4, "SET D");

        instructions[0xCBF3] = new Instruction("SET 6, E", 1, i => SET_b_R(i, 0x40, Register.E), 4, "SET E");

        instructions[0xCBF4] = new Instruction("SET 6, H", 1, i => SET_b_R(i, 0x40, Register.H), 4, "SET H");

        instructions[0xCBF5] = new Instruction("SET 6, L", 1, i => SET_b_R(i, 0x40, Register.L), 4, "SET L");

        instructions[0xCBF6] = new Instruction("SET 6, (HL)", 1, i => SET_b_addr_RR(i, 0x40, Register.HL), 11, "SET (HL)");

        instructions[0xCBF7] = new Instruction("SET 6, A", 1, i => SET_b_R(i, 0x40, Register.A), 4, "SET A");

        instructions[0xCBF8] = new Instruction("SET 7, B", 1, i => SET_b_R(i, 0x80, Register.B), 4, "SET B");

        instructions[0xCBF9] = new Instruction("SET 7, C", 1, i => SET_b_R(i, 0x80, Register.C), 4, "SET C");

        instructions[0xCBFA] = new Instruction("SET 7, D", 1, i => SET_b_R(i, 0x80, Register.D), 4, "SET D");

        instructions[0xCBFB] = new Instruction("SET 7, E", 1, i => SET_b_R(i, 0x80, Register.E), 4, "SET E");

        instructions[0xCBFC] = new Instruction("SET 7, H", 1, i => SET_b_R(i, 0x80, Register.H), 4, "SET H");

        instructions[0xCBFD] = new Instruction("SET 7, L", 1, i => SET_b_R(i, 0x80, Register.L), 4, "SET L");

        instructions[0xCBFE] = new Instruction("SET 7, (HL)", 1, i => SET_b_addr_RR(i, 0x80, Register.HL), 11, "SET (HL)");

        instructions[0xCBFF] = new Instruction("SET 7, A", 1, i => SET_b_R(i, 0x80, Register.A), 4, "SET A");
    }

    private static void InitialiseDDCBInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xDDCB00] = new Instruction("RLC (IX + d), B", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB00);

        instructions[0xDDCB01] = new Instruction("RLC (IX + d), C", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB01);

        instructions[0xDDCB02] = new Instruction("RLC (IX + d), D", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB02);

        instructions[0xDDCB03] = new Instruction("RLC (IX + d), E", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB03);

        instructions[0xDDCB04] = new Instruction("RLC (IX + d), H", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB04);

        instructions[0xDDCB05] = new Instruction("RLC (IX + d), L", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB05);

        instructions[0xDDCB06] = new Instruction("RLC (IX + d)", 2, i => RLC_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB06);

        instructions[0xDDCB07] = new Instruction("RLC (IX + d), A", 2, i => RLC_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB07);

        instructions[0xDDCB08] = new Instruction("RRC (IX + d), B", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB08);

        instructions[0xDDCB09] = new Instruction("RRC (IX + d), C", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB09);

        instructions[0xDDCB0A] = new Instruction("RRC (IX + d), D", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB0A);

        instructions[0xDDCB0B] = new Instruction("RRC (IX + d), E", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB0B);

        instructions[0xDDCB0C] = new Instruction("RRC (IX + d), H", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB0C);

        instructions[0xDDCB0D] = new Instruction("RRC (IX + d), L", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB0D);

        instructions[0xDDCB0E] = new Instruction("RRC (IX + d)", 2, i => RRC_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB0E);

        instructions[0xDDCB0F] = new Instruction("RRC (IX + d), A", 2, i => RRC_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB0F);

        instructions[0xDDCB10] = new Instruction("RL (IX + d), B", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB10);

        instructions[0xDDCB11] = new Instruction("RL (IX + d), C", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB11);

        instructions[0xDDCB12] = new Instruction("RL (IX + d), D", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB12);

        instructions[0xDDCB13] = new Instruction("RL (IX + d), E", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB13);

        instructions[0xDDCB14] = new Instruction("RL (IX + d), H", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB14);

        instructions[0xDDCB15] = new Instruction("RL (IX + d), L", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB15);

        instructions[0xDDCB16] = new Instruction("RL (IX + d)", 2, i => RL_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB16);

        instructions[0xDDCB17] = new Instruction("RL (IX + d), A", 2, i => RL_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB17);

        instructions[0xDDCB18] = new Instruction("RR (IX + d), B", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB18);

        instructions[0xDDCB19] = new Instruction("RR (IX + d), C", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB19);

        instructions[0xDDCB1A] = new Instruction("RR (IX + d), D", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB1A);

        instructions[0xDDCB1B] = new Instruction("RR (IX + d), E", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB1B);

        instructions[0xDDCB1C] = new Instruction("RR (IX + d), H", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB1C);

        instructions[0xDDCB1D] = new Instruction("RR (IX + d), L", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB1D);

        instructions[0xDDCB1E] = new Instruction("RR (IX + d)", 2, i => RR_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB1E);

        instructions[0xDDCB1F] = new Instruction("RR (IX + d), A", 2, i => RR_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB1F);

        instructions[0xDDCB20] = new Instruction("SLA (IX + d), B", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB20);

        instructions[0xDDCB21] = new Instruction("SLA (IX + d), C", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB21);

        instructions[0xDDCB22] = new Instruction("SLA (IX + d), D", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB22);

        instructions[0xDDCB23] = new Instruction("SLA (IX + d), E", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB23);

        instructions[0xDDCB24] = new Instruction("SLA (IX + d), H", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB24);

        instructions[0xDDCB25] = new Instruction("SLA (IX + d), L", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB25);

        instructions[0xDDCB26] = new Instruction("SLA (IX + d)", 2, i => SLA_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB26);

        instructions[0xDDCB27] = new Instruction("SLA (IX + d), A", 2, i => SLA_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB27);

        instructions[0xDDCB28] = new Instruction("SRA (IX + d), B", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB28);

        instructions[0xDDCB29] = new Instruction("SRA (IX + d), C", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB29);

        instructions[0xDDCB2A] = new Instruction("SRA (IX + d), D", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB2A);

        instructions[0xDDCB2B] = new Instruction("SRA (IX + d), E", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB2B);

        instructions[0xDDCB2C] = new Instruction("SRA (IX + d), H", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB2C);

        instructions[0xDDCB2D] = new Instruction("SRA (IX + d), L", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB2D);

        instructions[0xDDCB2E] = new Instruction("SRA (IX + d)", 2, i => SRA_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB2E);

        instructions[0xDDCB2F] = new Instruction("SRA (IX + d), A", 2, i => SRA_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB2F);

        instructions[0xDDCB30] = new Instruction("SLS (IX + d), B", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB30);

        instructions[0xDDCB31] = new Instruction("SLS (IX + d), C", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB31);

        instructions[0xDDCB32] = new Instruction("SLS (IX + d), D", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB32);

        instructions[0xDDCB33] = new Instruction("SLS (IX + d), E", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB33);

        instructions[0xDDCB34] = new Instruction("SLS (IX + d), H", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB34);

        instructions[0xDDCB35] = new Instruction("SLS (IX + d), L", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB35);

        instructions[0xDDCB36] = new Instruction("SLS (IX + d)", 2, i => SLS_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB36);

        instructions[0xDDCB37] = new Instruction("SLS (IX + d), A", 2, i => SLS_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB37);

        instructions[0xDDCB38] = new Instruction("SRL (IX + d), B", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB38);

        instructions[0xDDCB39] = new Instruction("SRL (IX + d), C", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB39);

        instructions[0xDDCB3A] = new Instruction("SRL (IX + d), D", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB3A);

        instructions[0xDDCB3B] = new Instruction("SRL (IX + d), E", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB3B);

        instructions[0xDDCB3C] = new Instruction("SRL (IX + d), H", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB3C);

        instructions[0xDDCB3D] = new Instruction("SRL (IX + d), L", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB3D);

        instructions[0xDDCB3E] = new Instruction("SRL (IX + d)", 2, i => SRL_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB3E);

        instructions[0xDDCB3F] = new Instruction("SRL (IX + d), A", 2, i => SRL_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB3F);

        instructions[0xDDCB40] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB40);

        instructions[0xDDCB41] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB41);

        instructions[0xDDCB42] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB42);

        instructions[0xDDCB43] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB43);

        instructions[0xDDCB44] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB44);

        instructions[0xDDCB45] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB45);

        instructions[0xDDCB46] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB46);

        instructions[0xDDCB47] = new Instruction("BIT_0 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB47);

        instructions[0xDDCB48] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB48);

        instructions[0xDDCB49] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB49);

        instructions[0xDDCB4A] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4A);

        instructions[0xDDCB4B] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4B);

        instructions[0xDDCB4C] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4C);

        instructions[0xDDCB4D] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4D);

        instructions[0xDDCB4E] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4E);

        instructions[0xDDCB4F] = new Instruction("BIT_1 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4F);

        instructions[0xDDCB50] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB50);

        instructions[0xDDCB51] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB51);

        instructions[0xDDCB52] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB52);

        instructions[0xDDCB53] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB53);

        instructions[0xDDCB54] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB54);

        instructions[0xDDCB55] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB55);

        instructions[0xDDCB56] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB56);

        instructions[0xDDCB57] = new Instruction("BIT_2 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB57);

        instructions[0xDDCB58] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB58);

        instructions[0xDDCB59] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB59);

        instructions[0xDDCB5A] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5A);

        instructions[0xDDCB5B] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5B);

        instructions[0xDDCB5C] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5C);

        instructions[0xDDCB5D] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5D);

        instructions[0xDDCB5E] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5E);

        instructions[0xDDCB5F] = new Instruction("BIT_3 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5F);

        instructions[0xDDCB60] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB60);

        instructions[0xDDCB61] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB61);

        instructions[0xDDCB62] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB62);

        instructions[0xDDCB63] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB63);

        instructions[0xDDCB64] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB64);

        instructions[0xDDCB65] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB65);

        instructions[0xDDCB66] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB66);

        instructions[0xDDCB67] = new Instruction("BIT_4 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB67);

        instructions[0xDDCB68] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB68);

        instructions[0xDDCB69] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB69);

        instructions[0xDDCB6A] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6A);

        instructions[0xDDCB6B] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6B);

        instructions[0xDDCB6C] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6C);

        instructions[0xDDCB6D] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6D);

        instructions[0xDDCB6E] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6E);

        instructions[0xDDCB6F] = new Instruction("BIT_5 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6F);

        instructions[0xDDCB70] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB70);

        instructions[0xDDCB71] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB71);

        instructions[0xDDCB72] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB72);

        instructions[0xDDCB73] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB73);

        instructions[0xDDCB74] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB74);

        instructions[0xDDCB75] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB75);

        instructions[0xDDCB76] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB76);

        instructions[0xDDCB77] = new Instruction("BIT_6 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB77);

        instructions[0xDDCB78] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB78);

        instructions[0xDDCB79] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB79);

        instructions[0xDDCB7A] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7A);

        instructions[0xDDCB7B] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7B);

        instructions[0xDDCB7C] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7C);

        instructions[0xDDCB7D] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7D);

        instructions[0xDDCB7E] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7E);

        instructions[0xDDCB7F] = new Instruction("BIT_7 (IX + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7F);

        instructions[0xDDCB80] = new Instruction("RES_0 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.B), 15, null, 0xDDCB80);

        instructions[0xDDCB81] = new Instruction("RES_0 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.C), 15, null, 0xDDCB81);

        instructions[0xDDCB82] = new Instruction("RES_0 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.D), 15, null, 0xDDCB82);

        instructions[0xDDCB83] = new Instruction("RES_0 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.E), 15, null, 0xDDCB83);

        instructions[0xDDCB84] = new Instruction("RES_0 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.H), 15, null, 0xDDCB84);

        instructions[0xDDCB85] = new Instruction("RES_0 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.L), 15, null, 0xDDCB85);

        instructions[0xDDCB86] = new Instruction("RES_0 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x01, Register.IX), 15, null, 0xDDCB86);

        instructions[0xDDCB87] = new Instruction("RES_0 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.A), 15, null, 0xDDCB87);

        instructions[0xDDCB88] = new Instruction("RES_1 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.B), 15, null, 0xDDCB88);

        instructions[0xDDCB89] = new Instruction("RES_1 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.C), 15, null, 0xDDCB89);

        instructions[0xDDCB8A] = new Instruction("RES_1 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.D), 15, null, 0xDDCB8A);

        instructions[0xDDCB8B] = new Instruction("RES_1 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.E), 15, null, 0xDDCB8B);

        instructions[0xDDCB8C] = new Instruction("RES_1 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.H), 15, null, 0xDDCB8C);

        instructions[0xDDCB8D] = new Instruction("RES_1 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.L), 15, null, 0xDDCB8D);

        instructions[0xDDCB8E] = new Instruction("RES_1 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x02, Register.IX), 15, null, 0xDDCB8E);

        instructions[0xDDCB8F] = new Instruction("RES_1 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.A), 15, null, 0xDDCB8F);

        instructions[0xDDCB90] = new Instruction("RES_2 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.B), 15, null, 0xDDCB90);

        instructions[0xDDCB91] = new Instruction("RES_2 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.C), 15, null, 0xDDCB91);

        instructions[0xDDCB92] = new Instruction("RES_2 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.D), 15, null, 0xDDCB92);

        instructions[0xDDCB93] = new Instruction("RES_2 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.E), 15, null, 0xDDCB93);

        instructions[0xDDCB94] = new Instruction("RES_2 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.H), 15, null, 0xDDCB94);

        instructions[0xDDCB95] = new Instruction("RES_2 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.L), 15, null, 0xDDCB95);

        instructions[0xDDCB96] = new Instruction("RES_2 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x04, Register.IX), 15, null, 0xDDCB96);

        instructions[0xDDCB97] = new Instruction("RES_2 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.A), 15, null, 0xDDCB97);
    
        instructions[0xDDCB98] = new Instruction("RES_3 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.B), 15, null, 0xDDCB98);

        instructions[0xDDCB99] = new Instruction("RES_3 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.C), 15, null, 0xDDCB99);

        instructions[0xDDCB9A] = new Instruction("RES_3 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.D), 15, null, 0xDDCB9A);

        instructions[0xDDCB9B] = new Instruction("RES_3 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.E), 15, null, 0xDDCB9B);

        instructions[0xDDCB9C] = new Instruction("RES_3 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.H), 15, null, 0xDDCB9C);

        instructions[0xDDCB9D] = new Instruction("RES_3 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.L), 15, null, 0xDDCB9D);

        instructions[0xDDCB9E] = new Instruction("RES_3 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x08, Register.IX), 15, null, 0xDDCB9E);

        instructions[0xDDCB9F] = new Instruction("RES_3 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.A), 15, null, 0xDDCB9F);

        instructions[0xDDCBA0] = new Instruction("RES_4 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.B), 15, null, 0xDDCBA0);

        instructions[0xDDCBA1] = new Instruction("RES_4 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.C), 15, null, 0xDDCBA1);

        instructions[0xDDCBA2] = new Instruction("RES_4 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.D), 15, null, 0xDDCBA2);

        instructions[0xDDCBA3] = new Instruction("RES_4 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.E), 15, null, 0xDDCBA3);

        instructions[0xDDCBA4] = new Instruction("RES_4 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.H), 15, null, 0xDDCBA4);

        instructions[0xDDCBA5] = new Instruction("RES_4 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.L), 15, null, 0xDDCBA5);

        instructions[0xDDCBA6] = new Instruction("RES_4 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x10, Register.IX), 15, null, 0xDDCBA6);

        instructions[0xDDCBA7] = new Instruction("RES_4 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.A), 15, null, 0xDDCBA7);

        instructions[0xDDCBA8] = new Instruction("RES_5 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.B), 15, null, 0xDDCBA8);

        instructions[0xDDCBA9] = new Instruction("RES_5 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.C), 15, null, 0xDDCBA9);

        instructions[0xDDCBAA] = new Instruction("RES_5 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.D), 15, null, 0xDDCBAA);

        instructions[0xDDCBAB] = new Instruction("RES_5 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.E), 15, null, 0xDDCBAB);

        instructions[0xDDCBAC] = new Instruction("RES_5 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.H), 15, null, 0xDDCBAC);

        instructions[0xDDCBAD] = new Instruction("RES_5 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.L), 15, null, 0xDDCBAD);

        instructions[0xDDCBAE] = new Instruction("RES_5 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x20, Register.IX), 15, null, 0xDDCBAE);

        instructions[0xDDCBAF] = new Instruction("RES_5 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.A), 15, null, 0xDDCBAF);

        instructions[0xDDCBB0] = new Instruction("RES_6 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.B), 15, null, 0xDDCBB0);

        instructions[0xDDCBB1] = new Instruction("RES_6 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.C), 15, null, 0xDDCBB1);

        instructions[0xDDCBB2] = new Instruction("RES_6 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.D), 15, null, 0xDDCBB2);

        instructions[0xDDCBB3] = new Instruction("RES_6 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.E), 15, null, 0xDDCBB3);

        instructions[0xDDCBB4] = new Instruction("RES_6 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.H), 15, null, 0xDDCBB4);

        instructions[0xDDCBB5] = new Instruction("RES_6 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.L), 15, null, 0xDDCBB5);

        instructions[0xDDCBB6] = new Instruction("RES_6, (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x40, Register.IX), 15, null, 0xDDCBB6);

        instructions[0xDDCBB7] = new Instruction("RES_6 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.A), 15, null, 0xDDCBB7);
    
        instructions[0xDDCBB8] = new Instruction("RES_7 (IX + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.B), 15, null, 0xDDCBB8);

        instructions[0xDDCBB9] = new Instruction("RES_7 (IX + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.C), 15, null, 0xDDCBB9);

        instructions[0xDDCBBA] = new Instruction("RES_7 (IX + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.D), 15, null, 0xDDCBBA);

        instructions[0xDDCBBB] = new Instruction("RES_7 (IX + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.E), 15, null, 0xDDCBBB);

        instructions[0xDDCBBC] = new Instruction("RES_7 (IX + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.H), 15, null, 0xDDCBBC);

        instructions[0xDDCBBD] = new Instruction("RES_7 (IX + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.L), 15, null, 0xDDCBBD);

        instructions[0xDDCBBE] = new Instruction("RES_7 (IX + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x80, Register.IX), 15, null, 0xDDCBBE);

        instructions[0xDDCBBF] = new Instruction("RES_7 (IX + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.A), 15, null, 0xDDCBBF);

        instructions[0xDDCBC0] = new Instruction("SET_0 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.B), 15, null, 0xDDCBC0);

        instructions[0xDDCBC1] = new Instruction("SET_0 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.C), 15, null, 0xDDCBC1);

        instructions[0xDDCBC2] = new Instruction("SET_0 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.D), 15, null, 0xDDCBC2);

        instructions[0xDDCBC3] = new Instruction("SET_0 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.E), 15, null, 0xDDCBC3);

        instructions[0xDDCBC4] = new Instruction("SET_0 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.H), 15, null, 0xDDCBC4);

        instructions[0xDDCBC5] = new Instruction("SET_0 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.L), 15, null, 0xDDCBC5);

        instructions[0xDDCBC6] = new Instruction("SET_0 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x01, Register.IX), 15, null, 0xDDCBC6);

        instructions[0xDDCBC7] = new Instruction("SET_0 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.A), 15, null, 0xDDCBC7);

        instructions[0xDDCBC8] = new Instruction("SET_1 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.B), 15, null, 0xDDCBC8);

        instructions[0xDDCBC9] = new Instruction("SET_1 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.C), 15, null, 0xDDCBC9);

        instructions[0xDDCBCA] = new Instruction("SET_1 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.D), 15, null, 0xDDCBCA);

        instructions[0xDDCBCB] = new Instruction("SET_1 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.E), 15, null, 0xDDCBCB);

        instructions[0xDDCBCC] = new Instruction("SET_1 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.H), 15, null, 0xDDCBCC);

        instructions[0xDDCBCD] = new Instruction("SET_1 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.L), 15, null, 0xDDCBCD);

        instructions[0xDDCBCE] = new Instruction("SET_1 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x02, Register.IX), 15, null, 0xDDCBCE);

        instructions[0xDDCBCF] = new Instruction("SET_1 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.A), 15, null, 0xDDCBCF);
    
        instructions[0xDDCBD0] = new Instruction("SET_2 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.B), 15, null, 0xDDCBD0);

        instructions[0xDDCBD1] = new Instruction("SET_2 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.C), 15, null, 0xDDCBD1);

        instructions[0xDDCBD2] = new Instruction("SET_2 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.D), 15, null, 0xDDCBD2);

        instructions[0xDDCBD3] = new Instruction("SET_2 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.E), 15, null, 0xDDCBD3);

        instructions[0xDDCBD4] = new Instruction("SET_2 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.H), 15, null, 0xDDCBD4);

        instructions[0xDDCBD5] = new Instruction("SET_2 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.L), 15, null, 0xDDCBD5);

        instructions[0xDDCBD6] = new Instruction("SET_2 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x04, Register.IX), 15, null, 0xDDCBD6);

        instructions[0xDDCBD7] = new Instruction("SET_2 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.A), 15, null, 0xDDCBD7);

        instructions[0xDDCBD8] = new Instruction("SET_3 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.B), 15, null, 0xDDCBD8);

        instructions[0xDDCBD9] = new Instruction("SET_3 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.C), 15, null, 0xDDCBD9);

        instructions[0xDDCBDA] = new Instruction("SET_3 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.D), 15, null, 0xDDCBDA);

        instructions[0xDDCBDB] = new Instruction("SET_3 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.E), 15, null, 0xDDCBDB);

        instructions[0xDDCBDC] = new Instruction("SET_3 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.H), 15, null, 0xDDCBDC);

        instructions[0xDDCBDD] = new Instruction("SET_3 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.L), 15, null, 0xDDCBDD);

        instructions[0xDDCBDE] = new Instruction("SET_3 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x08, Register.IX), 15, null, 0xDDCBDE);

        instructions[0xDDCBDF] = new Instruction("SET_3 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.A), 15, null, 0xDDCBDF);    

        instructions[0xDDCBE0] = new Instruction("SET_4 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.B), 15, null, 0xDDCBE0);

        instructions[0xDDCBE1] = new Instruction("SET_4 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.C), 15, null, 0xDDCBE1);

        instructions[0xDDCBE2] = new Instruction("SET_4 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.D), 15, null, 0xDDCBE2);

        instructions[0xDDCBE3] = new Instruction("SET_4 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.E), 15, null, 0xDDCBE3);

        instructions[0xDDCBE4] = new Instruction("SET_4 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.H), 15, null, 0xDDCBE4);

        instructions[0xDDCBE5] = new Instruction("SET_4 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.L), 15, null, 0xDDCBE5);

        instructions[0xDDCBE6] = new Instruction("SET_4 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x10, Register.IX), 15, null, 0xDDCBE6);

        instructions[0xDDCBE7] = new Instruction("SET_4 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.A), 15, null, 0xDDCBE7);

        instructions[0xDDCBE8] = new Instruction("SET_5 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.B), 15, null, 0xDDCBE8);

        instructions[0xDDCBE9] = new Instruction("SET_5 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.C), 15, null, 0xDDCBE9);

        instructions[0xDDCBEA] = new Instruction("SET_5 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.D), 15, null, 0xDDCBEA);

        instructions[0xDDCBEB] = new Instruction("SET_5 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.E), 15, null, 0xDDCBEB);

        instructions[0xDDCBEC] = new Instruction("SET_5 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.H), 15, null, 0xDDCBEC);

        instructions[0xDDCBED] = new Instruction("SET_5 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.L), 15, null, 0xDDCBED);

        instructions[0xDDCBEE] = new Instruction("SET_5 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x20, Register.IX), 15, null, 0xDDCBEE);

        instructions[0xDDCBEF] = new Instruction("SET_5 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.A), 15, null, 0xDDCBEF);     

        instructions[0xDDCBF0] = new Instruction("SET_6 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.B), 15, null, 0xDDCBF0);

        instructions[0xDDCBF1] = new Instruction("SET_6 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.C), 15, null, 0xDDCBF1);

        instructions[0xDDCBF2] = new Instruction("SET_6 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.D), 15, null, 0xDDCBF2);

        instructions[0xDDCBF3] = new Instruction("SET_6 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.E), 15, null, 0xDDCBF3);

        instructions[0xDDCBF4] = new Instruction("SET_6 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.H), 15, null, 0xDDCBF4);

        instructions[0xDDCBF5] = new Instruction("SET_6 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.L), 15, null, 0xDDCBF5);

        instructions[0xDDCBF6] = new Instruction("SET_6 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x40, Register.IX), 15, null, 0xDDCBF6);

        instructions[0xDDCBF7] = new Instruction("SET_6 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.A), 15, null, 0xDDCBF7);

        instructions[0xDDCBF8] = new Instruction("SET_7 (IX + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.B), 15, null, 0xDDCBF8);

        instructions[0xDDCBF9] = new Instruction("SET_7 (IX + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.C), 15, null, 0xDDCBF9);

        instructions[0xDDCBFA] = new Instruction("SET_7 (IX + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.D), 15, null, 0xDDCBFA);

        instructions[0xDDCBFB] = new Instruction("SET_7 (IX + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.E), 15, null, 0xDDCBFB);

        instructions[0xDDCBFC] = new Instruction("SET_7 (IX + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.H), 15, null, 0xDDCBFC);

        instructions[0xDDCBFD] = new Instruction("SET_7 (IX + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.L), 15, null, 0xDDCBFD);

        instructions[0xDDCBFE] = new Instruction("SET_7 (IX + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x80, Register.IX), 15, null, 0xDDCBFE);

        instructions[0xDDCBFF] = new Instruction("SET_7 (IX + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.A), 15, null, 0xDDCBFF);        
    }
    
    private static void InitialiseFDCBInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xFDCB00] = new Instruction("RLC (IY + d), B", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB00);

        instructions[0xFDCB01] = new Instruction("RLC (IY + d), C", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB01);

        instructions[0xFDCB02] = new Instruction("RLC (IY + d), D", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB02);

        instructions[0xFDCB03] = new Instruction("RLC (IY + d), E", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB03);

        instructions[0xFDCB04] = new Instruction("RLC (IY + d), H", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB04);

        instructions[0xFDCB05] = new Instruction("RLC (IY + d), L", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB05);

        instructions[0xFDCB06] = new Instruction("RLC (IY + d)", 2, i => RLC_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB06);

        instructions[0xFDCB07] = new Instruction("RLC (IY + d), A", 2, i => RLC_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB07);

        instructions[0xFDCB08] = new Instruction("RRC (IY + d), B", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB08);

        instructions[0xFDCB09] = new Instruction("RRC (IY + d), C", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB09);

        instructions[0xFDCB0A] = new Instruction("RRC (IY + d), D", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB0A);

        instructions[0xFDCB0B] = new Instruction("RRC (IY + d), E", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB0B);

        instructions[0xFDCB0C] = new Instruction("RRC (IY + d), H", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB0C);

        instructions[0xFDCB0D] = new Instruction("RRC (IY + d), L", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB0D);

        instructions[0xFDCB0E] = new Instruction("RRC (IY + d)", 2, i => RRC_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB0E);

        instructions[0xFDCB0F] = new Instruction("RRC (IY + d), A", 2, i => RRC_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB0F);

        instructions[0xFDCB10] = new Instruction("RL (IY + d), B", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB10);

        instructions[0xFDCB11] = new Instruction("RL (IY + d), C", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB11);

        instructions[0xFDCB12] = new Instruction("RL (IY + d), D", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB12);

        instructions[0xFDCB13] = new Instruction("RL (IY + d), E", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB13);

        instructions[0xFDCB14] = new Instruction("RL (IY + d), H", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB14);

        instructions[0xFDCB15] = new Instruction("RL (IY + d), L", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB15);

        instructions[0xFDCB16] = new Instruction("RL (IY + d)", 2, i => RL_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB16);

        instructions[0xFDCB17] = new Instruction("RL (IY + d), A", 2, i => RL_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB17);

        instructions[0xFDCB18] = new Instruction("RR (IY + d), B", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB18);

        instructions[0xFDCB19] = new Instruction("RR (IY + d), C", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB19);

        instructions[0xFDCB1A] = new Instruction("RR (IY + d), D", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB1A);

        instructions[0xFDCB1B] = new Instruction("RR (IY + d), E", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB1B);

        instructions[0xFDCB1C] = new Instruction("RR (IY + d), H", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB1C);

        instructions[0xFDCB1D] = new Instruction("RR (IY + d), L", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB1D);

        instructions[0xFDCB1E] = new Instruction("RR (IY + d)", 2, i => RR_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB1E);

        instructions[0xFDCB1F] = new Instruction("RR (IY + d), A", 2, i => RR_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB1F);

        instructions[0xFDCB20] = new Instruction("SLA (IY + d), B", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB20);

        instructions[0xFDCB21] = new Instruction("SLA (IY + d), C", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB21);

        instructions[0xFDCB22] = new Instruction("SLA (IY + d), D", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB22);

        instructions[0xFDCB23] = new Instruction("SLA (IY + d), E", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB23);

        instructions[0xFDCB24] = new Instruction("SLA (IY + d), H", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB24);

        instructions[0xFDCB25] = new Instruction("SLA (IY + d), L", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB25);

        instructions[0xFDCB26] = new Instruction("SLA (IY + d)", 2, i => SLA_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB26);

        instructions[0xFDCB27] = new Instruction("SLA (IY + d), A", 2, i => SLA_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB27);

        instructions[0xFDCB28] = new Instruction("SRA (IY + d), B", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB28);

        instructions[0xFDCB29] = new Instruction("SRA (IY + d), C", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB29);

        instructions[0xFDCB2A] = new Instruction("SRA (IY + d), D", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB2A);

        instructions[0xFDCB2B] = new Instruction("SRA (IY + d), E", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB2B);

        instructions[0xFDCB2C] = new Instruction("SRA (IY + d), H", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB2C);

        instructions[0xFDCB2D] = new Instruction("SRA (IY + d), L", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB2D);

        instructions[0xFDCB2E] = new Instruction("SRA (IY + d)", 2, i => SRA_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB2E);

        instructions[0xFDCB2F] = new Instruction("SRA (IY + d), A", 2, i => SRA_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB2F);

        instructions[0xFDCB30] = new Instruction("SLS (IY + d), B", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB30);

        instructions[0xFDCB31] = new Instruction("SLS (IY + d), C", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB31);

        instructions[0xFDCB32] = new Instruction("SLS (IY + d), D", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB32);

        instructions[0xFDCB33] = new Instruction("SLS (IY + d), E", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB33);

        instructions[0xFDCB34] = new Instruction("SLS (IY + d), H", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB34);

        instructions[0xFDCB35] = new Instruction("SLS (IY + d), L", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB35);

        instructions[0xFDCB36] = new Instruction("SLS (IY + d)", 2, i => SLS_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB36);

        instructions[0xFDCB37] = new Instruction("SLS (IY + d), A", 2, i => SLS_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB37);

        instructions[0xFDCB38] = new Instruction("SRL (IY + d), B", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.B), 15, null, 0xFDCB38);

        instructions[0xFDCB39] = new Instruction("SRL (IY + d), C", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.C), 15, null, 0xFDCB39);

        instructions[0xFDCB3A] = new Instruction("SRL (IY + d), D", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.D), 15, null, 0xFDCB3A);

        instructions[0xFDCB3B] = new Instruction("SRL (IY + d), E", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.E), 15, null, 0xFDCB3B);

        instructions[0xFDCB3C] = new Instruction("SRL (IY + d), H", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.H), 15, null, 0xFDCB3C);

        instructions[0xFDCB3D] = new Instruction("SRL (IY + d), L", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.L), 15, null, 0xFDCB3D);

        instructions[0xFDCB3E] = new Instruction("SRL (IY + d)", 2, i => SRL_addr_RR_plus_d(i, Register.IY), 15, null, 0xFDCB3E);

        instructions[0xFDCB3F] = new Instruction("SRL (IY + d), A", 2, i => SRL_addr_RR_plus_d_R(i, Register.IY, Register.A), 15, null, 0xFDCB3F);

        instructions[0xFDCB40] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB40);

        instructions[0xFDCB41] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB41);

        instructions[0xFDCB42] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB42);

        instructions[0xFDCB43] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB43);

        instructions[0xFDCB44] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB44);

        instructions[0xFDCB45] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB45);

        instructions[0xFDCB46] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB46);

        instructions[0xFDCB47] = new Instruction("BIT_0 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x01, Register.IY), 12, null, 0xFDCB47);

        instructions[0xFDCB48] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB48);

        instructions[0xFDCB49] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB49);

        instructions[0xFDCB4A] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4A);

        instructions[0xFDCB4B] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4B);

        instructions[0xFDCB4C] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4C);

        instructions[0xFDCB4D] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4D);

        instructions[0xFDCB4E] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4E);

        instructions[0xFDCB4F] = new Instruction("BIT_1 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x02, Register.IY), 12, null, 0xFDCB4F);

        instructions[0xFDCB50] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB50);

        instructions[0xFDCB51] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB51);

        instructions[0xFDCB52] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB52);

        instructions[0xFDCB53] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB53);

        instructions[0xFDCB54] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB54);

        instructions[0xFDCB55] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB55);

        instructions[0xFDCB56] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB56);

        instructions[0xFDCB57] = new Instruction("BIT_2 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x04, Register.IY), 12, null, 0xFDCB57);

        instructions[0xFDCB58] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB58);

        instructions[0xFDCB59] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB59);

        instructions[0xFDCB5A] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5A);

        instructions[0xFDCB5B] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5B);

        instructions[0xFDCB5C] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5C);

        instructions[0xFDCB5D] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5D);

        instructions[0xFDCB5E] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5E);

        instructions[0xFDCB5F] = new Instruction("BIT_3 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x08, Register.IY), 12, null, 0xFDCB5F);

        instructions[0xFDCB60] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB60);

        instructions[0xFDCB61] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB61);

        instructions[0xFDCB62] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB62);

        instructions[0xFDCB63] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB63);

        instructions[0xFDCB64] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB64);

        instructions[0xFDCB65] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB65);

        instructions[0xFDCB66] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB66);

        instructions[0xFDCB67] = new Instruction("BIT_4 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x10, Register.IY), 12, null, 0xFDCB67);

        instructions[0xFDCB68] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB68);

        instructions[0xFDCB69] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB69);

        instructions[0xFDCB6A] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6A);

        instructions[0xFDCB6B] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6B);

        instructions[0xFDCB6C] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6C);

        instructions[0xFDCB6D] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6D);

        instructions[0xFDCB6E] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6E);

        instructions[0xFDCB6F] = new Instruction("BIT_5 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB6F);

        instructions[0xFDCB70] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB70);

        instructions[0xFDCB71] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB71);

        instructions[0xFDCB72] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB72);

        instructions[0xFDCB73] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB73);

        instructions[0xFDCB74] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB74);

        instructions[0xFDCB75] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB75);

        instructions[0xFDCB76] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB76);

        instructions[0xFDCB77] = new Instruction("BIT_6 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x40, Register.IY), 12, null, 0xFDCB77);

        instructions[0xFDCB78] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB78);

        instructions[0xFDCB79] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB79);

        instructions[0xFDCB7A] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7A);

        instructions[0xFDCB7B] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7B);

        instructions[0xFDCB7C] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7C);

        instructions[0xFDCB7D] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7D);

        instructions[0xFDCB7E] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7E);

        instructions[0xFDCB7F] = new Instruction("BIT_7 (IY + d)", 2, i => BIT_b_addr_RR_plus_d(i, 0x20, Register.IY), 12, null, 0xFDCB7F);

        instructions[0xFDCB80] = new Instruction("RES_0 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.B), 15, null, 0xFDCB80);

        instructions[0xFDCB81] = new Instruction("RES_0 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.C), 15, null, 0xFDCB81);

        instructions[0xFDCB82] = new Instruction("RES_0 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.D), 15, null, 0xFDCB82);

        instructions[0xFDCB83] = new Instruction("RES_0 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.E), 15, null, 0xFDCB83);

        instructions[0xFDCB84] = new Instruction("RES_0 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.H), 15, null, 0xFDCB84);

        instructions[0xFDCB85] = new Instruction("RES_0 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.L), 15, null, 0xFDCB85);

        instructions[0xFDCB86] = new Instruction("RES_0 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x01, Register.IY), 15, null, 0xFDCB86);

        instructions[0xFDCB87] = new Instruction("RES_0 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.A), 15, null, 0xFDCB87);

        instructions[0xFDCB88] = new Instruction("RES_1 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.B), 15, null, 0xFDCB88);

        instructions[0xFDCB89] = new Instruction("RES_1 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.C), 15, null, 0xFDCB89);

        instructions[0xFDCB8A] = new Instruction("RES_1 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.D), 15, null, 0xFDCB8A);

        instructions[0xFDCB8B] = new Instruction("RES_1 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.E), 15, null, 0xFDCB8B);

        instructions[0xFDCB8C] = new Instruction("RES_1 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.H), 15, null, 0xFDCB8C);

        instructions[0xFDCB8D] = new Instruction("RES_1 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.L), 15, null, 0xFDCB8D);

        instructions[0xFDCB8E] = new Instruction("RES_1 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x02, Register.IY), 15, null, 0xFDCB8E);

        instructions[0xFDCB8F] = new Instruction("RES_1 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.A), 15, null, 0xFDCB8F);

        instructions[0xFDCB90] = new Instruction("RES_2 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.B), 15, null, 0xFDCB90);

        instructions[0xFDCB91] = new Instruction("RES_2 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.C), 15, null, 0xFDCB91);

        instructions[0xFDCB92] = new Instruction("RES_2 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.D), 15, null, 0xFDCB92);

        instructions[0xFDCB93] = new Instruction("RES_2 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.E), 15, null, 0xFDCB93);

        instructions[0xFDCB94] = new Instruction("RES_2 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.H), 15, null, 0xFDCB94);

        instructions[0xFDCB95] = new Instruction("RES_2 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.L), 15, null, 0xFDCB95);

        instructions[0xFDCB96] = new Instruction("RES_2 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x04, Register.IY), 15, null, 0xFDCB96);

        instructions[0xFDCB97] = new Instruction("RES_2 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.A), 15, null, 0xFDCB97);
    
        instructions[0xFDCB98] = new Instruction("RES_3 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.B), 15, null, 0xFDCB98);

        instructions[0xFDCB99] = new Instruction("RES_3 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.C), 15, null, 0xFDCB99);

        instructions[0xFDCB9A] = new Instruction("RES_3 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.D), 15, null, 0xFDCB9A);

        instructions[0xFDCB9B] = new Instruction("RES_3 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.E), 15, null, 0xFDCB9B);

        instructions[0xFDCB9C] = new Instruction("RES_3 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.H), 15, null, 0xFDCB9C);

        instructions[0xFDCB9D] = new Instruction("RES_3 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.L), 15, null, 0xFDCB9D);

        instructions[0xFDCB9E] = new Instruction("RES_3 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x08, Register.IY), 15, null, 0xFDCB9E);

        instructions[0xFDCB9F] = new Instruction("RES_3 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.A), 15, null, 0xFDCB9F);

        instructions[0xFDCBA0] = new Instruction("RES_4 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.B), 15, null, 0xFDCBA0);

        instructions[0xFDCBA1] = new Instruction("RES_4 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.C), 15, null, 0xFDCBA1);

        instructions[0xFDCBA2] = new Instruction("RES_4 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.D), 15, null, 0xFDCBA2);

        instructions[0xFDCBA3] = new Instruction("RES_4 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.E), 15, null, 0xFDCBA3);

        instructions[0xFDCBA4] = new Instruction("RES_4 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.H), 15, null, 0xFDCBA4);

        instructions[0xFDCBA5] = new Instruction("RES_4 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.L), 15, null, 0xFDCBA5);

        instructions[0xFDCBA6] = new Instruction("RES_4 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x10, Register.IY), 15, null, 0xFDCBA6);

        instructions[0xFDCBA7] = new Instruction("RES_4 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.A), 15, null, 0xFDCBA7);

        instructions[0xFDCBA8] = new Instruction("RES_5 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.B), 15, null, 0xFDCBA8);

        instructions[0xFDCBA9] = new Instruction("RES_5 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.C), 15, null, 0xFDCBA9);

        instructions[0xFDCBAA] = new Instruction("RES_5 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.D), 15, null, 0xFDCBAA);

        instructions[0xFDCBAB] = new Instruction("RES_5 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.E), 15, null, 0xFDCBAB);

        instructions[0xFDCBAC] = new Instruction("RES_5 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.H), 15, null, 0xFDCBAC);

        instructions[0xFDCBAD] = new Instruction("RES_5 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.L), 15, null, 0xFDCBAD);

        instructions[0xFDCBAE] = new Instruction("RES_5 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x20, Register.IY), 15, null, 0xFDCBAE);

        instructions[0xFDCBAF] = new Instruction("RES_5 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.A), 15, null, 0xFDCBAF);

        instructions[0xFDCBB0] = new Instruction("RES_6 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.B), 15, null, 0xFDCBB0);

        instructions[0xFDCBB1] = new Instruction("RES_6 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.C), 15, null, 0xFDCBB1);

        instructions[0xFDCBB2] = new Instruction("RES_6 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.D), 15, null, 0xFDCBB2);

        instructions[0xFDCBB3] = new Instruction("RES_6 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.E), 15, null, 0xFDCBB3);

        instructions[0xFDCBB4] = new Instruction("RES_6 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.H), 15, null, 0xFDCBB4);

        instructions[0xFDCBB5] = new Instruction("RES_6 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.L), 15, null, 0xFDCBB5);

        instructions[0xFDCBB6] = new Instruction("RES_6, (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x40, Register.IY), 15, null, 0xFDCBB6);

        instructions[0xFDCBB7] = new Instruction("RES_6 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.A), 15, null, 0xFDCBB7);
    
        instructions[0xFDCBB8] = new Instruction("RES_7 (IY + d), B", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.B), 15, null, 0xFDCBB8);

        instructions[0xFDCBB9] = new Instruction("RES_7 (IY + d), C", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.C), 15, null, 0xFDCBB9);

        instructions[0xFDCBBA] = new Instruction("RES_7 (IY + d), D", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.D), 15, null, 0xFDCBBA);

        instructions[0xFDCBBB] = new Instruction("RES_7 (IY + d), E", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.E), 15, null, 0xFDCBBB);

        instructions[0xFDCBBC] = new Instruction("RES_7 (IY + d), H", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.H), 15, null, 0xFDCBBC);

        instructions[0xFDCBBD] = new Instruction("RES_7 (IY + d), L", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.L), 15, null, 0xFDCBBD);

        instructions[0xFDCBBE] = new Instruction("RES_7 (IY + d)", 2, i => RES_b_addr_RR_plus_d(i, 0x80, Register.IY), 15, null, 0xFDCBBE);

        instructions[0xFDCBBF] = new Instruction("RES_7 (IY + d), A", 2, i => RES_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.A), 15, null, 0xFDCBBF);

        instructions[0xFDCBC0] = new Instruction("SET_0 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.B), 15, null, 0xFDCBC0);

        instructions[0xFDCBC1] = new Instruction("SET_0 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.C), 15, null, 0xFDCBC1);

        instructions[0xFDCBC2] = new Instruction("SET_0 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.D), 15, null, 0xFDCBC2);

        instructions[0xFDCBC3] = new Instruction("SET_0 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.E), 15, null, 0xFDCBC3);

        instructions[0xFDCBC4] = new Instruction("SET_0 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.H), 15, null, 0xFDCBC4);

        instructions[0xFDCBC5] = new Instruction("SET_0 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.L), 15, null, 0xFDCBC5);

        instructions[0xFDCBC6] = new Instruction("SET_0 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x01, Register.IY), 15, null, 0xFDCBC6);

        instructions[0xFDCBC7] = new Instruction("SET_0 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x01, Register.IY, Register.A), 15, null, 0xFDCBC7);

        instructions[0xFDCBC8] = new Instruction("SET_1 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.B), 15, null, 0xFDCBC8);

        instructions[0xFDCBC9] = new Instruction("SET_1 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.C), 15, null, 0xFDCBC9);

        instructions[0xFDCBCA] = new Instruction("SET_1 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.D), 15, null, 0xFDCBCA);

        instructions[0xFDCBCB] = new Instruction("SET_1 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.E), 15, null, 0xFDCBCB);

        instructions[0xFDCBCC] = new Instruction("SET_1 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.H), 15, null, 0xFDCBCC);

        instructions[0xFDCBCD] = new Instruction("SET_1 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.L), 15, null, 0xFDCBCD);

        instructions[0xFDCBCE] = new Instruction("SET_1 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x02, Register.IY), 15, null, 0xFDCBCE);

        instructions[0xFDCBCF] = new Instruction("SET_1 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x02, Register.IY, Register.A), 15, null, 0xFDCBCF);
    
        instructions[0xFDCBD0] = new Instruction("SET_2 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.B), 15, null, 0xFDCBD0);

        instructions[0xFDCBD1] = new Instruction("SET_2 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.C), 15, null, 0xFDCBD1);

        instructions[0xFDCBD2] = new Instruction("SET_2 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.D), 15, null, 0xFDCBD2);

        instructions[0xFDCBD3] = new Instruction("SET_2 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.E), 15, null, 0xFDCBD3);

        instructions[0xFDCBD4] = new Instruction("SET_2 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.H), 15, null, 0xFDCBD4);

        instructions[0xFDCBD5] = new Instruction("SET_2 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.L), 15, null, 0xFDCBD5);

        instructions[0xFDCBD6] = new Instruction("SET_2 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x04, Register.IY), 15, null, 0xFDCBD6);

        instructions[0xFDCBD7] = new Instruction("SET_2 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x04, Register.IY, Register.A), 15, null, 0xFDCBD7);

        instructions[0xFDCBD8] = new Instruction("SET_3 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.B), 15, null, 0xFDCBD8);

        instructions[0xFDCBD9] = new Instruction("SET_3 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.C), 15, null, 0xFDCBD9);

        instructions[0xFDCBDA] = new Instruction("SET_3 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.D), 15, null, 0xFDCBDA);

        instructions[0xFDCBDB] = new Instruction("SET_3 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.E), 15, null, 0xFDCBDB);

        instructions[0xFDCBDC] = new Instruction("SET_3 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.H), 15, null, 0xFDCBDC);

        instructions[0xFDCBDD] = new Instruction("SET_3 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.L), 15, null, 0xFDCBDD);

        instructions[0xFDCBDE] = new Instruction("SET_3 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x08, Register.IY), 15, null, 0xFDCBDE);

        instructions[0xFDCBDF] = new Instruction("SET_3 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x08, Register.IY, Register.A), 15, null, 0xFDCBDF);    

        instructions[0xFDCBE0] = new Instruction("SET_4 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.B), 15, null, 0xFDCBE0);

        instructions[0xFDCBE1] = new Instruction("SET_4 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.C), 15, null, 0xFDCBE1);

        instructions[0xFDCBE2] = new Instruction("SET_4 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.D), 15, null, 0xFDCBE2);

        instructions[0xFDCBE3] = new Instruction("SET_4 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.E), 15, null, 0xFDCBE3);

        instructions[0xFDCBE4] = new Instruction("SET_4 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.H), 15, null, 0xFDCBE4);

        instructions[0xFDCBE5] = new Instruction("SET_4 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.L), 15, null, 0xFDCBE5);

        instructions[0xFDCBE6] = new Instruction("SET_4 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x10, Register.IY), 15, null, 0xFDCBE6);

        instructions[0xFDCBE7] = new Instruction("SET_4 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x10, Register.IY, Register.A), 15, null, 0xFDCBE7);

        instructions[0xFDCBE8] = new Instruction("SET_5 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.B), 15, null, 0xFDCBE8);

        instructions[0xFDCBE9] = new Instruction("SET_5 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.C), 15, null, 0xFDCBE9);

        instructions[0xFDCBEA] = new Instruction("SET_5 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.D), 15, null, 0xFDCBEA);

        instructions[0xFDCBEB] = new Instruction("SET_5 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.E), 15, null, 0xFDCBEB);

        instructions[0xFDCBEC] = new Instruction("SET_5 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.H), 15, null, 0xFDCBEC);

        instructions[0xFDCBED] = new Instruction("SET_5 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.L), 15, null, 0xFDCBED);

        instructions[0xFDCBEE] = new Instruction("SET_5 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x20, Register.IY), 15, null, 0xFDCBEE);

        instructions[0xFDCBEF] = new Instruction("SET_5 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x20, Register.IY, Register.A), 15, null, 0xFDCBEF);     

        instructions[0xFDCBF0] = new Instruction("SET_6 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.B), 15, null, 0xFDCBF0);

        instructions[0xFDCBF1] = new Instruction("SET_6 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.C), 15, null, 0xFDCBF1);

        instructions[0xFDCBF2] = new Instruction("SET_6 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.D), 15, null, 0xFDCBF2);

        instructions[0xFDCBF3] = new Instruction("SET_6 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.E), 15, null, 0xFDCBF3);

        instructions[0xFDCBF4] = new Instruction("SET_6 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.H), 15, null, 0xFDCBF4);

        instructions[0xFDCBF5] = new Instruction("SET_6 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.L), 15, null, 0xFDCBF5);

        instructions[0xFDCBF6] = new Instruction("SET_6 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x40, Register.IY), 15, null, 0xFDCBF6);

        instructions[0xFDCBF7] = new Instruction("SET_6 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x40, Register.IY, Register.A), 15, null, 0xFDCBF7);

        instructions[0xFDCBF8] = new Instruction("SET_7 (IY + d), B", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.B), 15, null, 0xFDCBF8);

        instructions[0xFDCBF9] = new Instruction("SET_7 (IY + d), C", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.C), 15, null, 0xFDCBF9);

        instructions[0xFDCBFA] = new Instruction("SET_7 (IY + d), D", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.D), 15, null, 0xFDCBFA);

        instructions[0xFDCBFB] = new Instruction("SET_7 (IY + d), E", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.E), 15, null, 0xFDCBFB);

        instructions[0xFDCBFC] = new Instruction("SET_7 (IY + d), H", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.H), 15, null, 0xFDCBFC);

        instructions[0xFDCBFD] = new Instruction("SET_7 (IY + d), L", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.L), 15, null, 0xFDCBFD);

        instructions[0xFDCBFE] = new Instruction("SET_7 (IY + d)", 2, i => SET_b_addr_RR_plus_d(i, 0x80, Register.IY), 15, null, 0xFDCBFE);

        instructions[0xFDCBFF] = new Instruction("SET_7 (IY + d), A", 2, i => SET_b_addr_RR_plus_d_R(i, 0x80, Register.IY, Register.A), 15, null, 0xFDCBFF);        
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

    private static bool RR_R(Input input, Register register)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[register] & 0x01;

            var result = (byte) (input.State.Registers[register] >> 1);

            result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

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

    private static bool RR_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var result = (byte) (data >> 1);

            result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

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

    private static bool SLA_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (input.State.Registers[register] & 0x80) >> 7;

            var result = input.State.Registers[register] <<= 1;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLA_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRA_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var bottomBit = data & 0x01;

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data >> 1);

            result |= topBit;

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRA_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data >> 1);

            result |= topBit;

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLS_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data << 1);

            result |= 0x01;

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = topBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLS_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data << 1);

            result |= 0x01;

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRL_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRL_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool BIT_b_R(Input input, byte bit, Register register)
    {
        var data = input.State.Registers[register];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool BIT_b_addr_RR(Input input, byte bit, Register register)
    {
        var data = input.Ram[input.State.Registers.ReadPair(register)];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool RES_b_R(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.State.Registers[register] = (byte) (input.State.Registers[register] & mask);

        // Flags unaffected

        return true;
    }

    private static bool RES_b_addr_RR(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.Ram[input.State.Registers.ReadPair(register)] = (byte) (input.Ram[input.State.Registers.ReadPair(register)] & mask);

        // Flags unaffected

        return true;
    }

    private static bool SET_b_R(Input input, byte bit, Register register)
    {
        input.State.Registers[register] |= bit;

        // Flags unaffected

        return true;
    }

    private static bool SET_b_addr_RR(Input input, byte bit, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] |= bit;

        // Flags unaffected

        return true;
    }

    private static bool IM_m(Input input, InterruptMode mode)
    {
        input.State.InterruptMode = mode;

        // Flags unaffected

        return true;
    }

    private static bool SLA_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RLC_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RLC_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RRC_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (bottomBit << 7));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RRC_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (bottomBit << 7));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RL_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | (byte) (input.State.Flags.Carry ? 0x01 : 0x00));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RL_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) |  (byte) (input.State.Flags.Carry ? 0x01 : 0x00));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RR_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (byte) (input.State.Flags.Carry ? 0x80 : 0x00));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool RR_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (byte) (input.State.Flags.Carry ? 0x80 : 0x00));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLA_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRA_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) (data & 0x80);

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | topBit);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRA_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) (data & 0x80);

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | topBit);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLS_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) ((data << 1) | 0x01);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SLS_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) ((data << 1) | 0x01);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRL_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool SRL_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    private static bool BIT_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool RES_b_addr_RR_plus_d_R(Input input, byte bit, Register source, Register destination)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        input.State.Registers[destination] = result;

        // Flags unaffected

        return true;
    }
    
    private static bool RES_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        // Flags unaffected

        return true;
    }

    private static bool SET_b_addr_RR_plus_d(Input input, byte bit, Register register)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        // Flags unaffected

        return true;
    }

    private static bool SET_b_addr_RR_plus_d_R(Input input, byte bit, Register register, Register destination)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        input.State.Registers[destination] = input.Ram[address];

        // Flags unaffected

        return true;
    }

    private static bool IN_b_R_addr_n(Input input, Register register)
    {
        var result = input.Ports.ReadByte(input.Data[1]);

        input.State.Registers[register] = result;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool OUT_b_addr_n_R(Input input, Register register)
    {
        input.Ports.WriteByte(input.Data[1], input.State.Registers[register]);

        // Flags unaffected

        return true;
    }

    private static bool TST_R(Input input, Register register)
    {
        var result = (byte) (input.State.Registers[Register.A] & input.State.Registers[register]);

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool TST_addr_R(Input input, Register register)
    {
        var result = (byte) (input.State.Registers[Register.A] & input.State.Registers.ReadPair(register));

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool IN_R_addr_R(Input input, Register destination, Register source)
    {
        var value = input.Ports.ReadByte(input.State.Registers.ReadPair(source));

        input.State.Registers[destination] = value;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value.IsEvenParity();
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = value == 0;
        input.State.Flags.Sign = (sbyte) value < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool OUT_addr_RR_R(Input input, Register destination, Register source)
    {
        // Flags unaffected

        return true;
    }

    private static bool NEG_R(Input input, Register register)
    {
        var result = (byte) (0 - input.State.Registers[register]);

        input.State.Registers[register] = result;

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    private static bool ADC_RR_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.Registers.ReadPair(source);

            var carry = (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers.WritePair(destination, (ushort) result);

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
    }}