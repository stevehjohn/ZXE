using ZXE.Core.Exceptions;
using ZXE.Core.Extensions;
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

        if (instruction.Action(new Input(data, _state, ram, ports)))
        {
            _state.ProgramCounter += instruction.Length;
        }

        if (! (instruction.Mnemonic.StartsWith("SOPSET") && instruction.Mnemonic.Length == 11))
        {
            UpdateR(instruction.Length);
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

    private void UpdateR(int amount)
    {
        var value = (byte) (_state.Registers[Register.R] & 0x7F);

        var topBit = _state.Registers[Register.R] & 0x80;

        value = (byte) (value + 1); // + amount); // TODO: This seems to work... Y tho?

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
            instructionArray[instruction.Key] = instruction.Value;
        }

        return instructionArray;
    }

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

        instructions[0xED40] = new Instruction("IN B, (BC)", 1, i => IN_R_addr_RR(i, Register.B, Register.BC), 8, null, 0xED40);

        instructions[0xED41] = new Instruction("OUT (BC), B", 1, i => OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED41);

        instructions[0xED42] = new Instruction("SBC HL, BC", 1, i => SBC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED42);

        instructions[0xED43] = new Instruction("LD (nn), BC", 3, i => LD_addr_nn_RR(i, Register.BC), 16, null, 0xED43);

        instructions[0xED44] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED44);

        // TODO: instructions[0xED45] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED45);

        instructions[0xED46] = new Instruction("IM 0", 1, i => IM_m(i, InterruptMode.Mode0), 4, null, 0xED46);

        instructions[0xED47] = new Instruction("LD I, A", 1, i => LD_R_R(i, Register.I, Register.A), 5, null, 0xED47);

        instructions[0xED48] = new Instruction("IN C, (BC)", 1, i => IN_R_addr_RR(i, Register.C, Register.BC), 8, null, 0xED48);

        instructions[0xED49] = new Instruction("OUT (BC), B", 1, i => OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED49);

        instructions[0xED4A] = new Instruction("ADC HL, BC", 1, i => ADC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED4A);

        instructions[0xED4B] = new Instruction("LD BC, (nn)", 3, i => LD_RR_addr_nn(i, Register.BC), 16, null, 0xED4B);

        instructions[0xED4C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED4C);

        // TODO: Which is right? instructions[0xED4C] = new Instruction("MLT BC", 1, i => MLT_RR(i, Register.BC), 13, null, 0xED4C);

        // TODO: instructions[0xED4D] = new Instruction("RETI", 1, i => (i, Register.BC), 10, null, 0xED4D);

        instructions[0xED4F] = new Instruction("LD R, A", 1, i => LD_R_R(i, Register.R, Register.A), 5, null, 0xED4F);
        
        instructions[0xED50] = new Instruction("IN D, (BC)", 1, i => IN_R_addr_RR(i, Register.D, Register.BC), 8, null, 0xED50);

        instructions[0xED51] = new Instruction("OUT (BC), D", 1, i => OUT_addr_RR_R(i, Register.BC, Register.D), 8, null, 0xED51);

        instructions[0xED52] = new Instruction("SBC HL, DE", 1, i => SBC_RR_RR(i, Register.HL, Register.DE), 11, null, 0xED52);

        instructions[0xED53] = new Instruction("LD (nn), DE", 3, i => LD_addr_nn_RR(i, Register.DE), 16, null, 0xED53);

        // TODO: Exists? instructions[0xED54] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED45);

        // TODO: instructions[0xED55] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED55);

        instructions[0xED56] = new Instruction("IM 1", 1, i => IM_m(i, InterruptMode.Mode1), 4, null, 0xED56);

        instructions[0xED57] = new Instruction("LD A, I", 1, i => LD_R_R(i, Register.A, Register.I), 5, null, 0xED57);

        instructions[0xED58] = new Instruction("IN E, (BC)", 1, i => IN_R_addr_RR(i, Register.E, Register.BC), 8, null, 0xED58);

        instructions[0xED59] = new Instruction("OUT (BC), E", 1, i => OUT_addr_RR_R(i, Register.BC, Register.E), 8, null, 0xED59);

        instructions[0xED5A] = new Instruction("ADC HL, DE", 1, i => ADC_RR_RR(i, Register.HL, Register.DE), 11, null, 0xED5A);

        instructions[0xED5B] = new Instruction("LD DE, (nn)", 3, i => LD_RR_addr_nn(i, Register.DE), 16, null, 0xED5B);

        instructions[0xED5C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED5C);

        instructions[0xED5E] = new Instruction("IM 2", 1, i => IM_m(i, InterruptMode.Mode2), 5, null, 0xED5E);

        instructions[0xED5F] = new Instruction("LD A, R", 1, i => LD_R_R(i, Register.A, Register.R), 5, null, 0xED5F);

        instructions[0xED60] = new Instruction("IN H, (BC)", 1, i => IN_R_addr_RR(i, Register.H, Register.BC), 8, null, 0xED60);

        instructions[0xED61] = new Instruction("OUT (BC), H", 1, i => OUT_addr_RR_R(i, Register.BC, Register.H), 8, null, 0xED61);

        instructions[0xED62] = new Instruction("SBC HL, HL", 1, i => SBC_RR_RR(i, Register.HL, Register.HL), 11, null, 0xED62);

        instructions[0xED63] = new Instruction("LD (nn), HL", 3, i => LD_addr_nn_RR(i, Register.HL), 16, null, 0xED63);

        // TODO: Exists? instructions[0xED64] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED64);

        // TODO: instructions[0xED65] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED65);

        // TODO: instructions[0xED67] = new Instruction("RRD", 1, i => (i, Register.A, Register.I), 5, null, 0xED67);

        instructions[0xED68] = new Instruction("IN L, (BC)", 1, i => IN_R_addr_RR(i, Register.L, Register.BC), 8, null, 0xED68);

        instructions[0xED69] = new Instruction("OUT (BC), L", 1, i => OUT_addr_RR_R(i, Register.BC, Register.L), 8, null, 0xED69);

        instructions[0xED6A] = new Instruction("ADC HL, HL", 1, i => ADC_RR_RR(i, Register.HL, Register.HL), 11, null, 0xED6A);

        instructions[0xED6B] = new Instruction("LD HL, (nn)", 3, i => LD_RR_addr_nn(i, Register.HL), 16, null, 0xED6B);

        // TODO: instructions[0xED6C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED6C);

        // TODO: instructions[0xED6F] = new Instruction("RLD", 1, i => (i, Register.A, Register.R), 5, null, 0xED6F);

        instructions[0xED70] = new Instruction("IN (BC)", 1, i => IN_addr_RR(i, Register.BC), 8, null, 0xED70);

        instructions[0xED71] = new Instruction("OUT (BC), 0", 1, i => OUT_addr_R_n(i, Register.BC, 0), 8, null, 0xED71);

        instructions[0xED72] = new Instruction("SBC HL, SP", 1, i => SBC_RR_SP(i, Register.HL), 11, null, 0xED72);

        instructions[0xED73] = new Instruction("LD (nn), SP", 1, i => LD_addr_nn_SP(i), 16, null, 0xED73);

        // TODO: Exists? instructions[0xED74] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED74);

        instructions[0xED76] = new Instruction("IM 1", 1, i => IM_m(i, InterruptMode.Mode1), 4, null, 0xED76);

        instructions[0xED78] = new Instruction("IN A, (BC)", 1, i => IN_R_addr_RR(i, Register.A, Register.BC), 8, null, 0xED78);

        instructions[0xED79] = new Instruction("OUT (BC), A", 1, i => OUT_addr_RR_R(i, Register.BC, Register.A), 8, null, 0xED79);

        instructions[0xED7A] = new Instruction("ADC HL, SP", 1, i => ADC_RR_SP(i, Register.HL), 11, null, 0xED7A);

        instructions[0xED7B] = new Instruction("LD HL, (nn)", 3, i => LD_RR_addr_nn(i, Register.HL), 16, null, 0xED7B);

        // TODO: instructions[0xED7C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED7C);

        instructions[0xED7E] = new Instruction("IM 2", 3, i => IM_m(i, InterruptMode.Mode2), 4, null, 0xED7E);

        instructions[0xEDA0] = new Instruction("LDI", 1, i => LDI(i), 12, null, 0xEDA0);
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

    private static bool EX_RR_RaRa(Input input, Register register1, Register register2)
    {
        var alternate1 = Enum.Parse<Register>($"{register1}1");

        var alternate2 = Enum.Parse<Register>($"{register2}1");

        (input.State.Registers[register1], input.State.Registers[alternate1]) = (input.State.Registers[alternate1], input.State.Registers[register1]);

        (input.State.Registers[register2], input.State.Registers[alternate2]) = (input.State.Registers[alternate2], input.State.Registers[register2]);

        // Flags unaffected

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

    private static bool HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected

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

    private static bool OUT_addr_n_R(Input input, Register register)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

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

    private static bool EX_RR_RR(Input input, Register left, Register right)
    {
        var swap = input.State.Registers.ReadPair(left);

        input.State.Registers.WritePair(left, input.State.Registers.ReadPair(right));

        input.State.Registers.WritePair(right, swap);
        
        // Flags unaffected

        return true;
    }

    private static bool DI(Input input)
    {
        // TODO: Disable maskable interrupt.
        return true;
    }

    private static bool EI(Input input)
    {
        // TODO: Enable maskable interrupt.
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

    private static bool IM_m(Input input, InterruptMode mode)
    {
        input.State.InterruptMode = mode;

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

    private static bool IN_R_addr_RR(Input input, Register destination, Register source)
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

    //private static bool MLT_RR(Input input, Register register)
    //{
    //    var value = (int) input.State.Registers.ReadLow(register);

    //    value *= input.State.Registers.ReadHigh(register);

    //    input.State.Registers.WritePair(register, (ushort) value);

    //    // Flags unaffected

    //    return true;
    //}

    private static bool IN_addr_RR(Input input, Register source)
    {
        var value = input.Ports.ReadByte(input.State.Registers.ReadPair(source));

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

    private static bool OUT_addr_R_n(Input input, Register register, byte data)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return true;
    }

    private static bool LDI(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        input.Ram[input.State.Registers.ReadPair(Register.DE)] = value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.DE, (ushort) (input.State.Registers.ReadPair(Register.DE) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        value += input.State.Registers[Register.A];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }
}