// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
    private void InitialiseDDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xDD00] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4, null, 0xDD00);

        instructions[0xDD01] = new Instruction("LD BC, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.BC), 10, null, 0xDD01);

        instructions[0xDD02] = new Instruction("LD (BC), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.BC, Register.A), 7, null, 0xDD02);

        instructions[0xDD03] = new Instruction("INC BC", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.BC), 6, null, 0xDD03);

        instructions[0xDD04] = new Instruction("INC B", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.B), 4, null, 0xDD04);

        instructions[0xDD05] = new Instruction("DEC B", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.B), 4, null, 0xDD05);

        instructions[0xDD06] = new Instruction("LD B, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.B), 7, null, 0xDD06);

        instructions[0xDD07] = new Instruction("RLCA", 1, ProcessorBitwiseRotationInstructions.RLCA, 4, "RLCA A", 0xDD07);

        instructions[0xDD08] = new Instruction("EX AF, AF'", 1, i => ProcessorMiscellaneousInstructions.EX_RR_R1R1(i, Register.A, Register.F), 4, null, 0xDD08);

        instructions[0xDD09] = new Instruction("ADD IX, BC", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.BC), 11, null, 0xDD09);

        instructions[0xDD0A] = new Instruction("LD A, (BC)'", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.BC), 7, null, 0xDD0A);

        instructions[0xDD0B] = new Instruction("DEC BC", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.BC), 6, null, 0xDD0B);

        instructions[0xDD0C] = new Instruction("INC C", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.C), 4, null, 0xDD0C);

        instructions[0xDD0D] = new Instruction("DEC C", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.C), 4, null, 0xDD0D);

        instructions[0xDD0E] = new Instruction("LD C, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.C), 7, null, 0xDD0E);

        instructions[0xDD0F] = new Instruction("RRCA", 1, ProcessorBitwiseRotationInstructions.RRCA, 4, "RRCA A", 0xDD0F);

        instructions[0xDD10] = new Instruction("DJNZ e", 2, ProcessorBranchInstructions.DJNZ_e, 8, "DJNZ B, e", 0xDD10);

        instructions[0xDD11] = new Instruction("LD DE, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.DE), 10, null, 0xDD11);

        instructions[0xDD12] = new Instruction("LD (DE), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.DE, Register.A), 7, null, 0xDD12);

        instructions[0xDD13] = new Instruction("INC DE", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.DE), 6, null, 0xDD13);

        instructions[0xDD14] = new Instruction("INC D", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.D), 4, null, 0xDD14);

        instructions[0xDD15] = new Instruction("DEC D", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.D), 4, null, 0xDD15);

        instructions[0xDD16] = new Instruction("LD D, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.D), 7, null, 0xDD16);

        instructions[0xDD17] = new Instruction("RLA", 1, ProcessorBitwiseRotationInstructions.RLA, 4, "RLA A", 0xDD17);

        instructions[0xDD18] = new Instruction("JR e", 2, ProcessorBranchInstructions.JR_e, 12, null, 0xDD18);

        instructions[0xDD19] = new Instruction("ADD IX, DE", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.DE), 11, null, 0xDD19);

        instructions[0xDD1A] = new Instruction("LD A, (DE)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.DE), 7, null, 0xDD1A);

        instructions[0xDD1B] = new Instruction("DEC DE", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.DE), 6, null, 0xDD1B);

        instructions[0xDD1C] = new Instruction("INC E", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.E), 4, null, 0xDD1C);

        instructions[0xDD1D] = new Instruction("DEC E", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.E), 4, null, 0xDD1D);

        instructions[0xDD1E] = new Instruction("LD E, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.E), 7, null, 0xDD1E);

        instructions[0xDD1F] = new Instruction("RRA", 1, ProcessorBitwiseRotationInstructions.RRA, 4, "RRA A", 0xDD1F);

        instructions[0xDD20] = new Instruction("JR NZ, e", 2, ProcessorBranchInstructions.JR_NZ_e, 7, null, 0xDD20);

        instructions[0xDD21] = new Instruction("LD IX, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.IX), 10);

        instructions[0xDD22] = new Instruction("LD (nn), IX", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.IX), 16);

        instructions[0xDD23] = new Instruction("INC IX", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.IX), 6);

        instructions[0xDD24] = new Instruction("INC IXh", 1, i => ProcessorArithmeticInstructions.INC_RRh(i, Register.IX), 4);

        instructions[0xDD25] = new Instruction("DEC IXh", 1, i => ProcessorArithmeticInstructions.DEC_RRh(i, Register.IX), 4);

        instructions[0xDD26] = new Instruction("LD IXh, n", 2, i => ProcessorLoadInstructions.LD_RRh_n(i, Register.IX), 7);

        instructions[0xDD27] = new Instruction("DAA", 1, ProcessorMiscellaneousInstructions.DAA, 4, null, 0xDD27);

        instructions[0xDD28] = new Instruction("JR Z, e", 2, ProcessorBranchInstructions.JR_Z_e, 7, null, 0xDD28);

        instructions[0xDD29] = new Instruction("ADD IX, IX", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IX, Register.IX), 11);

        instructions[0xDD2A] = new Instruction("LD IX, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.IX), 16);

        instructions[0xDD2B] = new Instruction("DEC IX", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.IX), 6);

        instructions[0xDD2C] = new Instruction("INC IXl", 1, i => ProcessorArithmeticInstructions.INC_RRl(i, Register.IX), 4);

        instructions[0xDD2D] = new Instruction("DEC IXl", 1, i => ProcessorArithmeticInstructions.DEC_RRl(i, Register.IX), 4);

        instructions[0xDD2E] = new Instruction("LD IXl, n", 2, i => ProcessorLoadInstructions.LD_RRl_n(i, Register.IX), 7);

        instructions[0xDD2F] = new Instruction("CPL", 1, ProcessorMiscellaneousInstructions.CPL, 4, null, 0xDD2F);

        instructions[0xDD30] = new Instruction("JR NC, e", 2, ProcessorBranchInstructions.JR_NC_e, 7, null, 0xDD30);

        instructions[0xDD31] = new Instruction("LD SP, nn", 3, ProcessorLoadInstructions.LD_SP_nn, 10, null, 0xDD31);

        instructions[0xDD32] = new Instruction("LD (nn), A", 3, i => ProcessorLoadInstructions.LD_addr_nn_R(i, Register.A), 13, null, 0xDD32);

        instructions[0xDD33] = new Instruction("INC SP", 1, ProcessorArithmeticInstructions.INC_SP, 6, null, 0xDD33);

        instructions[0xDD34] = new Instruction("INC (IX + d)", 2, i => ProcessorArithmeticInstructions.INC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD35] = new Instruction("DEC (IX + d)", 2, i => ProcessorArithmeticInstructions.DEC_addr_RR_plus_d(i, Register.IX), 19);

        instructions[0xDD36] = new Instruction("LD (IX + d), n", 3, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_n(i, Register.IX), 19);

        instructions[0xDD37] = new Instruction("SCF", 1, ProcessorMiscellaneousInstructions.SCF, 4, null, 0xDD37);

        instructions[0xDD38] = new Instruction("JR C, e", 2, ProcessorBranchInstructions.JR_C_e, 7, null, 0xDD38);

        instructions[0xDD39] = new Instruction("ADD IX, SP", 1, i => ProcessorArithmeticInstructions.ADD_RR_SP(i, Register.IX), 11);

        instructions[0xDD3A] = new Instruction("LD A, (nn)", 3, i => ProcessorLoadInstructions.LD_R_addr_nn(i, Register.A), 13);

        instructions[0xDD3B] = new Instruction("DEC SP", 1, ProcessorArithmeticInstructions.DEC_SP, 6);

        instructions[0xDD3C] = new Instruction("INC A", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.A), 4);

        instructions[0xDD3D] = new Instruction("DEC A", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.A), 4);

        instructions[0xDD3E] = new Instruction("LD A, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.A), 7);

        instructions[0xDD3F] = new Instruction("CCF", 1, ProcessorMiscellaneousInstructions.CCF, 4);

        instructions[0xDD40] = new Instruction("LD B, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.B), 4, null, 0xDD40);

        instructions[0xDD41] = new Instruction("LD B, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.C), 4, null, 0xDD41);

        instructions[0xDD42] = new Instruction("LD B, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.D), 4, null, 0xDD42);

        instructions[0xDD43] = new Instruction("LD B, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.E), 4, null, 0xDD43);

        instructions[0xDD44] = new Instruction("LD B, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.B, Register.IX), 4);

        instructions[0xDD45] = new Instruction("LD B, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.B, Register.IX), 4);

        instructions[0xDD46] = new Instruction("LD B, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.B, Register.IX), 15);

        instructions[0xDD47] = new Instruction("LD B, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.A), 4, null, 0xDD47);

        instructions[0xDD48] = new Instruction("LD C, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.B), 4, null, 0xDD48);

        instructions[0xDD49] = new Instruction("LD C, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.C), 4, null, 0xDD49);

        instructions[0xDD4A] = new Instruction("LD C, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.D), 4, null, 0xDD4A);

        instructions[0xDD4B] = new Instruction("LD C, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.E), 4, null, 0xDD4B);

        instructions[0xDD4C] = new Instruction("LD C, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.C, Register.IX), 4);

        instructions[0xDD4D] = new Instruction("LD C, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.C, Register.IX), 4);

        instructions[0xDD4E] = new Instruction("LD C, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.C, Register.IX), 15);

        instructions[0xDD4F] = new Instruction("LD C, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.A), 4, null, 0xDD4F);

        instructions[0xDD50] = new Instruction("LD D, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.B), 4, null, 0xDD50);

        instructions[0xDD51] = new Instruction("LD D, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.C), 4, null, 0xDD51);

        instructions[0xDD52] = new Instruction("LD D, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.D), 4, null, 0xDD52);

        instructions[0xDD53] = new Instruction("LD D, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.E), 4, null, 0xDD53);

        instructions[0xDD54] = new Instruction("LD D, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.D, Register.IX), 4);

        instructions[0xDD55] = new Instruction("LD D, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.D, Register.IX), 4);

        instructions[0xDD56] = new Instruction("LD D, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.D, Register.IX), 15);

        instructions[0xDD57] = new Instruction("LD D, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.A), 4, null, 0xDD57);

        instructions[0xDD58] = new Instruction("LD E, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.B), 4, null, 0xDD58);

        instructions[0xDD59] = new Instruction("LD E, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.C), 4, null, 0xDD59);

        instructions[0xDD5A] = new Instruction("LD E, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.D), 4, null, 0xDD5A);

        instructions[0xDD5B] = new Instruction("LD E, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.E), 4, null, 0xDD5B);

        instructions[0xDD5C] = new Instruction("LD E, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.E, Register.IX), 4);

        instructions[0xDD5D] = new Instruction("LD E, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.E, Register.IX), 4);

        instructions[0xDD5E] = new Instruction("LD E, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.E, Register.IX), 15);

        instructions[0xDD5F] = new Instruction("LD E, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.A), 4, null, 0xDD5F);

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

        instructions[0xDD70] = new Instruction("LD (IX + d), B", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.B), 15);

        instructions[0xDD71] = new Instruction("LD (IX + d), C", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.C), 15);

        instructions[0xDD72] = new Instruction("LD (IX + d), D", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.D), 15);

        instructions[0xDD73] = new Instruction("LD (IX + d), E", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.E), 15);

        instructions[0xDD74] = new Instruction("LD (IX + d), H", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.H), 15);

        instructions[0xDD75] = new Instruction("LD (IX + d), L", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.L), 15);

        instructions[0xDD76] = new Instruction("HALT", 1, ProcessorMiscellaneousInstructions.HALT, 4, null, 0xDD76);

        instructions[0xDD77] = new Instruction("LD (IX + d), A", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IX, Register.A), 15);

        instructions[0xDD78] = new Instruction("LD A, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.B), 4, null, 0xDD78);

        instructions[0xDD79] = new Instruction("LD A, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.C), 4, null, 0xDD79);

        instructions[0xDD7A] = new Instruction("LD A, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.D), 4, null, 0xDD7A);

        instructions[0xDD7B] = new Instruction("LD A, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.E), 4, null, 0xDD7B);

        instructions[0xDD7C] = new Instruction("LD A, IXh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD7D] = new Instruction("LD A, IXl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD7E] = new Instruction("LD A, (IX + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD7F] = new Instruction("LD A, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.A), 4);

        instructions[0xDD80] = new Instruction("ADD A, B", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.B), 4, null, 0xDD80);

        instructions[0xDD81] = new Instruction("ADD A, C", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.C), 4, null, 0xDD81);

        instructions[0xDD82] = new Instruction("ADD A, D", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.D), 4, null, 0xDD82);

        instructions[0xDD83] = new Instruction("ADD A, E", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.E), 4, null, 0xDD83);

        instructions[0xDD84] = new Instruction("ADD A, IXh", 1, i => ProcessorArithmeticInstructions.ADD_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD85] = new Instruction("ADD A, IXl", 1, i => ProcessorArithmeticInstructions.ADD_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD86] = new Instruction("ADD A, (IX + d)", 2, i => ProcessorArithmeticInstructions.ADD_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD87] = new Instruction("ADD A, A", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.A), 4, null, 0xDD87);

        instructions[0xDD88] = new Instruction("ADC A, B", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.B), 4, null, 0xDD89);

        instructions[0xDD89] = new Instruction("ADC A, C", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.C), 4, null, 0xDD89);

        instructions[0xDD8A] = new Instruction("ADC A, D", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.D), 4, null, 0xDD8A);

        instructions[0xDD8B] = new Instruction("ADC A, E", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.E), 4, null, 0xDD8B);

        instructions[0xDD8C] = new Instruction("ADC A, IXh", 1, i => ProcessorArithmeticInstructions.ADC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD8D] = new Instruction("ADC A, IXl", 1, i => ProcessorArithmeticInstructions.ADC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD8E] = new Instruction("ADC A, (IX + d)", 2, i => ProcessorArithmeticInstructions.ADC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD8F] = new Instruction("ADC A, A", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.A), 4, null, 0xDD8F);

        instructions[0xDD90] = new Instruction("SUB A, B", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.B), 4, null, 0xDD90);

        instructions[0xDD91] = new Instruction("SUB A, C", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.C), 4, null, 0xDD91);

        instructions[0xDD92] = new Instruction("SUB A, D", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.D), 4, null, 0xDD92);

        instructions[0xDD93] = new Instruction("SUB A, E", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.E), 4, null, 0xDD93);

        instructions[0xDD94] = new Instruction("SUB A, IXh", 1, i => ProcessorArithmeticInstructions.SUB_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD95] = new Instruction("SUB A, IXl", 1, i => ProcessorArithmeticInstructions.SUB_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD96] = new Instruction("SUB A, (IX + d)", 2, i => ProcessorArithmeticInstructions.SUB_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD97] = new Instruction("SUB A, A", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.A), 4, null, 0xDD97);

        instructions[0xDD98] = new Instruction("SBC A, B", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.B), 4, null, 0xDD98);

        instructions[0xDD99] = new Instruction("SBC A, C", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.C), 4, null, 0xDD99);

        instructions[0xDD9A] = new Instruction("SBC A, D", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.D), 4, null, 0xDD9A);

        instructions[0xDD9B] = new Instruction("SBC A, E", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.E), 4, null, 0xDD9B);

        instructions[0xDD9C] = new Instruction("SBC A, IXh", 1, i => ProcessorArithmeticInstructions.SBC_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDD9D] = new Instruction("SBC A, IXl", 1, i => ProcessorArithmeticInstructions.SBC_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDD9E] = new Instruction("SBC A, (IX + d)", 2, i => ProcessorArithmeticInstructions.SBC_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDD9F] = new Instruction("SBC A, A", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.A), 4);

        instructions[0xDDA0] = new Instruction("AND A, B", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.B), 4);

        instructions[0xDDA1] = new Instruction("AND A, C", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.C), 4);

        instructions[0xDDA2] = new Instruction("AND A, D", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.D), 4);

        instructions[0xDDA3] = new Instruction("AND A, E", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.E), 4);

        instructions[0xDDA4] = new Instruction("AND A, IXh", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDA5] = new Instruction("AND A, IXl", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDA6] = new Instruction("AND A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.AND_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDA7] = new Instruction("AND A, A", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.A), 4);

        instructions[0xDDA8] = new Instruction("XOR A, B", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.B), 4);

        instructions[0xDDA9] = new Instruction("XOR A, C", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.C), 4);

        instructions[0xDDAA] = new Instruction("XOR A, D", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.D), 4);

        instructions[0xDDAB] = new Instruction("XOR A, E", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.E), 4);

        instructions[0xDDAC] = new Instruction("XOR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDAD] = new Instruction("XOR A, IXl", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDAE] = new Instruction("XOR A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDAF] = new Instruction("XOR A, A", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.A), 4);

        instructions[0xDDB0] = new Instruction("OR A, B", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.B), 4);

        instructions[0xDDB1] = new Instruction("OR A, C", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.C), 4);

        instructions[0xDDB2] = new Instruction("OR A, D", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.D), 4);

        instructions[0xDDB3] = new Instruction("OR A, E", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.E), 4);

        instructions[0xDDB4] = new Instruction("OR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDB5] = new Instruction("OR A, IXh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDB6] = new Instruction("OR A, (IX + d)", 2, i => ProcessorBitwiseLogicInstructions.OR_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDB7] = new Instruction("OR A, A", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.A), 4);

        instructions[0xDDB8] = new Instruction("CP A, B", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.B), 4);

        instructions[0xDDB9] = new Instruction("CP A, C", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.C), 4);

        instructions[0xDDBA] = new Instruction("CP A, D", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.D), 4);

        instructions[0xDDBB] = new Instruction("CP A, E", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.E), 4);

        instructions[0xDDBC] = new Instruction("CP A, IXh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRh(i, Register.A, Register.IX), 4);

        instructions[0xDDBD] = new Instruction("CP A, IXh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRl(i, Register.A, Register.IX), 4);

        instructions[0xDDBE] = new Instruction("CP A, (IX + d)", 2, i => ProcessorMiscellaneousInstructions.CP_R_addr_RR_plus_d(i, Register.A, Register.IX), 15);

        instructions[0xDDBF] = new Instruction("CP A, A", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.A), 4);

        instructions[0xDDC0] = new Instruction("RET NZ", 1, ProcessorBranchInstructions.RET_NZ, 5);

        instructions[0xDDC1] = new Instruction("POP BC", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.BC), 10);

        instructions[0xDDC2] = new Instruction("JP NZ, nn", 3, ProcessorBranchInstructions.JP_NZ_nn, 10);

        instructions[0xDDC3] = new Instruction("JP nn", 3, ProcessorBranchInstructions.JP_nn, 10);

        instructions[0xDDC4] = new Instruction("CALL NZ, nn", 3, ProcessorBranchInstructions.CALL_NZ_nn, 10, null);

        instructions[0xDDC5] = new Instruction("PUSH BC", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.BC), 11);

        instructions[0xDDC6] = new Instruction("ADD A, n", 2, i => ProcessorArithmeticInstructions.ADD_R_n(i, Register.A), 7);

        instructions[0xDDC7] = new Instruction("RST 0x00", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x00), 11);

        instructions[0xDDC8] = new Instruction("RET Z", 1, ProcessorBranchInstructions.RET_Z, 5);

        instructions[0xDDC9] = new Instruction("RET", 1, ProcessorBranchInstructions.RET, 10);

        instructions[0xDDCA] = new Instruction("JP Z, nn", 3, ProcessorBranchInstructions.JP_Z_nn, 10);

        instructions[0xDDCB] = new Instruction("SOPSET 0xDDCB", 1, _ => SetOpcodePrefix(0xDDCB), 4);

        instructions[0xDDCC] = new Instruction("CALL Z, nn", 3, ProcessorBranchInstructions.CALL_Z_nn, 10);

        instructions[0xDDCD] = new Instruction("CALL nn", 3, ProcessorBranchInstructions.CALL_nn, 17);

        instructions[0xDDCE] = new Instruction("ADC A, n", 2, i => ProcessorArithmeticInstructions.ADC_R_n(i, Register.A), 7);

        instructions[0xDDCF] = new Instruction("RST 0x08", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x08), 11);

        instructions[0xDDD0] = new Instruction("RET NC", 1, ProcessorBranchInstructions.RET_NC, 5);

        instructions[0xDDD1] = new Instruction("POP DE", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.DE), 10);

        instructions[0xDDD2] = new Instruction("JP NC, nn", 3, ProcessorBranchInstructions.JP_NC_nn, 10);

        instructions[0xDDD3] = new Instruction("OUT (n), A", 2, i => ProcessorPortInstructions.OUT_addr_n_R(i, Register.A), 11);

        instructions[0xDDD4] = new Instruction("CALL NC, nn", 3, ProcessorBranchInstructions.CALL_NC_nn, 10);

        instructions[0xDDD5] = new Instruction("PUSH DE", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.DE), 11);

        instructions[0xDDD6] = new Instruction("SUB A, n", 2, i => ProcessorArithmeticInstructions.SUB_R_n(i, Register.A), 7);

        instructions[0xDDD7] = new Instruction("RST 0x10", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x10), 11);

        instructions[0xDDD8] = new Instruction("RET C", 1, ProcessorBranchInstructions.RET_C, 5);

        instructions[0xDDD9] = new Instruction("EXX", 1, ProcessorMiscellaneousInstructions.EXX, 4);

        instructions[0xDDDA] = new Instruction("JP C, nn", 3, ProcessorBranchInstructions.JP_C_nn, 10);

        instructions[0xDDDB] = new Instruction("IN A, (n)", 2, i => ProcessorPortInstructions.IN_R_p(i, Register.A), 11);

        instructions[0xDDDC] = new Instruction("CALL C, nn", 3, ProcessorBranchInstructions.CALL_C_nn, 10);

        instructions[0xDDDD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xDDDE] = new Instruction("SBC A, n", 2, i => ProcessorArithmeticInstructions.SBC_R_n(i, Register.A), 7);

        instructions[0xDDDF] = new Instruction("RST 0x18", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x18), 11);

        instructions[0xDDE0] = new Instruction("RET PO", 1, ProcessorBranchInstructions.RET_PO, 5);

        instructions[0xDDE1] = new Instruction("POP IX", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.IX), 10);

        instructions[0xDDE2] = new Instruction("JP PO, nn", 3, ProcessorBranchInstructions.JP_PO_nn, 10);

        instructions[0xDDE3] = new Instruction("EX (SP), IX", 1, i => ProcessorMiscellaneousInstructions.EX_addr_SP_RR(i, Register.IX), 19);

        instructions[0xDDE4] = new Instruction("CALL PO, nn", 3, ProcessorBranchInstructions.CALL_PO_nn, 10);

        instructions[0xDDE5] = new Instruction("PUSH IX", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.IX), 11);

        instructions[0xDDE6] = new Instruction("AND A, n", 2, i => ProcessorBitwiseLogicInstructions.AND_R_n(i, Register.A), 7);

        instructions[0xDDE7] = new Instruction("RST 0x20", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x20), 11);

        instructions[0xDDE8] = new Instruction("RET PE", 1, ProcessorBranchInstructions.RET_PE, 5);

        instructions[0xDDE9] = new Instruction("JP (IX)", 1, i => ProcessorBranchInstructions.JP_addr_RR(i, Register.IX), 4);

        instructions[0xDDEA] = new Instruction("JP PE, nn", 3, ProcessorBranchInstructions.JP_PE_nn, 10);

        instructions[0xDDEB] = new Instruction("EX DE, HL", 1, i => ProcessorMiscellaneousInstructions.EX_RR_RR(i, Register.DE, Register.HL), 4);

        instructions[0xDDEC] = new Instruction("CALL PE, nn", 3, ProcessorBranchInstructions.CALL_PE_nn, 10);

        instructions[0xDDED] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xDDEE] = new Instruction("XOR A, n", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_n(i, Register.A), 7);

        instructions[0xDDEF] = new Instruction("RST 0x28", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x28), 11);

        instructions[0xDDF0] = new Instruction("RET NS", 1, ProcessorBranchInstructions.RET_NS, 5);

        instructions[0xDDF1] = new Instruction("POP AF", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.AF), 10);

        instructions[0xDDF2] = new Instruction("JP NS, nn", 3, ProcessorBranchInstructions.JP_NS_nn, 10);

        instructions[0xDDF3] = new Instruction("DI", 1, ProcessorMiscellaneousInstructions.DI, 4);

        instructions[0xDDF4] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_NS_nn, 10);

        instructions[0xDDF5] = new Instruction("PUSH AF", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.AF), 11);

        instructions[0xDDF6] = new Instruction("OR A, n", 2, i => ProcessorBitwiseLogicInstructions.OR_R_n(i, Register.A), 7);

        instructions[0xDDF7] = new Instruction("RST 0x30", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x30), 11);

        instructions[0xDDF8] = new Instruction("RET S", 1, ProcessorBranchInstructions.RET_S, 5);

        instructions[0xDDF9] = new Instruction("LD SP, IX", 1, i => ProcessorLoadInstructions.LD_SP_RR(i, Register.IX), 6);

        instructions[0xDDFA] = new Instruction("JP S, nn", 3, ProcessorBranchInstructions.JP_S_nn, 10);

        instructions[0xDDFB] = new Instruction("EI", 1, ProcessorMiscellaneousInstructions.EI, 4);

        instructions[0xDDFC] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_S_nn, 10);

        instructions[0xDDFD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xDDFE] = new Instruction("CP A, n", 2, i => ProcessorMiscellaneousInstructions.CP_R_n(i, Register.A), 7);

        instructions[0xDDFF] = new Instruction("RST 0x38", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x38), 11);
    }
}