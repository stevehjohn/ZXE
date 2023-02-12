// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
    private void InitialiseDDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xDD09] = new Instruction("ADD IX, BC", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.BC), 11);

        instructions[0xDD19] = new Instruction("ADD IX, DE", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.DE), 11);

        instructions[0xDD21] = new Instruction("LD IX, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.IX), 10);

        instructions[0xDD22] = new Instruction("LD (nn), IX", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.IX), 16);

        instructions[0xDD23] = new Instruction("INC IX", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.IX), 6);

        instructions[0xDD24] = new Instruction("INC IXh", 1, i => ProcessorArithmeticInstructions.INC_RRh(i, Register.IX), 4);

        instructions[0xDD25] = new Instruction("DEC IXh", 1, i => ProcessorArithmeticInstructions.DEC_RRh(i, Register.IX), 4);

        instructions[0xDD26] = new Instruction("LD IXh, n", 2, i => ProcessorLoadInstructions.LD_RRh_n(i, Register.IX), 7);

        instructions[0xDD29] = new Instruction("ADD IX, IX", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.IX), 11);

        instructions[0xDD2A] = new Instruction("LD IX, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.IX), 16);

        instructions[0xDD2B] = new Instruction("DEC IX", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.IX), 6);

        instructions[0xDD2C] = new Instruction("INC IXl", 1, i => ProcessorArithmeticInstructions.INC_RRl(i, Register.IX), 4);

        instructions[0xDD2D] = new Instruction("DEC IXl", 1, i => ProcessorArithmeticInstructions.DEC_RRl(i, Register.IX), 4);

        instructions[0xDD2E] = new Instruction("LD IXl, n", 2, i => ProcessorLoadInstructions.LD_RRl_n(i, Register.IX), 7);

        instructions[0xDD34] = new Instruction("INC (IX + d)", 2, i => ProcessorArithmeticInstructions.INC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD35] = new Instruction("DEC (IX + d)", 2, i => ProcessorArithmeticInstructions.DEC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD36] = new Instruction("LD (IX + d), n", 3, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_n(i, Register.IX), 19);

        instructions[0xDD39] = new Instruction("ADD IX, SP", 1, i => ProcessorArithmeticInstructions.ADD_RR_SP(i, Register.IX), 11);

        instructions[0xDD44] = new Instruction("LD B, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.B, Register.IX), 4);

        instructions[0xDD45] = new Instruction("LD B, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.B, Register.IX), 4);

        instructions[0xDD46] = new Instruction("LD B, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.B, Register.IX), 15);

        instructions[0xDD4C] = new Instruction("LD C, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.C, Register.IX), 4);

        instructions[0xDD4D] = new Instruction("LD C, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.C, Register.IX), 4);

        instructions[0xDD4E] = new Instruction("LD C, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.C, Register.IX), 15);

        instructions[0xDD54] = new Instruction("LD D, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.D, Register.IX), 4);

        instructions[0xDD55] = new Instruction("LD D, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.D, Register.IX), 4);

        instructions[0xDD56] = new Instruction("LD D, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.D, Register.IX), 15);

        instructions[0xDD5C] = new Instruction("LD E, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.E, Register.IX), 4);

        instructions[0xDD5D] = new Instruction("LD E, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.E, Register.IX), 4);

        instructions[0xDD5E] = new Instruction("LD E, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.E, Register.IX), 15);

        instructions[0xDD60] = new Instruction("LD IXh, B", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IX, Register.B), 4);

        instructions[0xDD61] = new Instruction("LD IXh, C", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IX, Register.C), 4);

        instructions[0xDD62] = new Instruction("LD IXh, D", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IX, Register.D), 4);

        instructions[0xDD63] = new Instruction("LD IXh, E", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IX, Register.E), 4);

        instructions[0xDD64] = new Instruction("LD IXh, IXh", 1, i => ProcessorLoadInstructions.LD_RRh_RRh(i, Register.IX, Register.IX), 4);

        instructions[0xDD65] = new Instruction("LD IXh, IXl", 1, i => ProcessorLoadInstructions.LD_RRh_RRl(i, Register.IX, Register.IX), 4);

        instructions[0xDD66] = new Instruction("LD H, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.H, Register.IX), 15);

        instructions[0xDD67] = new Instruction("LD IXh, A", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IX, Register.A), 4);

        instructions[0xDD68] = new Instruction("LD IXl, B", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IX, Register.B), 4);

        instructions[0xDD69] = new Instruction("LD IXl, C", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IX, Register.C), 4);

        instructions[0xDD6A] = new Instruction("LD IXl, D", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IX, Register.D), 4);

        instructions[0xDD6B] = new Instruction("LD IXl, E", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IX, Register.E), 4);

        instructions[0xDD6C] = new Instruction("LD IXl, IXh", 1, i => ProcessorLoadInstructions.LD_RRl_RRh(i, Register.IX, Register.IX), 4);

        instructions[0xDD6D] = new Instruction("LD IXl, IXl", 1, i => ProcessorLoadInstructions.LD_RRl_RRl(i, Register.IX, Register.IX), 4);

        instructions[0xDD6E] = new Instruction("LD L, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.L, Register.IX), 15);

        instructions[0xDD6F] = new Instruction("LD IXl, A", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IX, Register.A), 4);

        instructions[0xDD70] = new Instruction("LD (IX + d), B", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.B), 4);

        instructions[0xDD71] = new Instruction("LD (IX + d), C", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.C), 4);

        instructions[0xDD72] = new Instruction("LD (IX + d), D", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.D), 4);

        instructions[0xDD73] = new Instruction("LD (IX + d), E", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.E), 4);

        instructions[0xDD74] = new Instruction("LD (IX + d), H", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.H), 4);

        instructions[0xDD75] = new Instruction("LD (IX + d), L", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.L), 4);

        instructions[0xDD77] = new Instruction("LD (IX + d), A", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.A), 15);

        instructions[0xDD7C] = new Instruction("LD A, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD7D] = new Instruction("LD A, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD7E] = new Instruction("LD A, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD84] = new Instruction("ADD A, IXh", 1, i => ProcessorArithmeticInstructions.ADD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD85] = new Instruction("ADD A, IXl", 1, i => ProcessorArithmeticInstructions.ADD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD86] = new Instruction("ADD A, (IX + d)", 2, i => ProcessorArithmeticInstructions.ADD_R_addr_RR_plus_d(i, Register.A, Register.IX), 4);

        instructions[0xDD8C] = new Instruction("ADC A, IXh", 1, i => ProcessorArithmeticInstructions.ADC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD8D] = new Instruction("ADC A, IXl", 1, i => ProcessorArithmeticInstructions.ADC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD8E] = new Instruction("ADC A, (IX + d)", 2, i => ProcessorArithmeticInstructions.ADC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD94] = new Instruction("SUB A, IXh", 1, i => ProcessorArithmeticInstructions.SUB_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD95] = new Instruction("SUB A, IXl", 1, i => ProcessorArithmeticInstructions.SUB_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD96] = new Instruction("SUB A, (IX + d)", 2, i => ProcessorArithmeticInstructions.SUB_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD9C] = new Instruction("SBC A, IXh", 1, i => ProcessorArithmeticInstructions.SBC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD9D] = new Instruction("SBC A, IXl", 1, i => ProcessorArithmeticInstructions.SBC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD9E] = new Instruction("SBC A, (IX + d)", 2, i => ProcessorArithmeticInstructions.SBC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDA4] = new Instruction("AND A, IXh", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDA5] = new Instruction("AND A, IXl", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDA6] = new Instruction("AND A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.AND_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDAC] = new Instruction("XOR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDAD] = new Instruction("XOR A, IXl", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDAE] = new Instruction("XOR A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDB4] = new Instruction("OR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDB5] = new Instruction("OR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDB6] = new Instruction("OR A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.OR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDBC] = new Instruction("CP A, IXh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDBD] = new Instruction("CP A, IXh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDBE] = new Instruction("CP A, (IX + d)", 2, i => ProcessorMiscellaneousInstructions.CP_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDCB] = new Instruction("SOPSET 0xDDCB", 1, _ => SetOpcodePrefix(0xDDCB), 4);

        instructions[0xDDDD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xDDE1] = new Instruction("POP IX", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.IX), 10);

        instructions[0xDDE3] = new Instruction("EX (SP), IX", 1, i => ProcessorMiscellaneousInstructions.EX_addr_SP_RR(i, Register.IX), 19);

        instructions[0xDDE5] = new Instruction("PUSH IX", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.IX), 11);

        instructions[0xDDE9] = new Instruction("JP (IX)", 1, i => ProcessorBranchInstructions.JP_addr_RR(i, Register.IX), 4);

        instructions[0xDDF9] = new Instruction("LD SP, IX", 1, i => ProcessorLoadInstructions.LD_SP_RR(i, Register.IX), 6);

        instructions[0xDDFD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);
    }
}