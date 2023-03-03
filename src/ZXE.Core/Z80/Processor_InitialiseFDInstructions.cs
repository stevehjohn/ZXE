// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
    private void InitialiseFDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xFD00] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4, null, 0xFD00);
        
        instructions[0xFD01] = new Instruction("LD BC, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.BC), 10);

        instructions[0xFD02] = new Instruction("LD (BC), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.BC, Register.A), 7);

        instructions[0xFD03] = new Instruction("INC BC", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.BC), 6);

        instructions[0xFD04] = new Instruction("INC B", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.B), 4);

        instructions[0xFD05] = new Instruction("DEC B", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.B), 4);

        instructions[0xFD06] = new Instruction("LD B, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.B), 7);

        instructions[0xFD07] = new Instruction("RLCA", 1, ProcessorBitwiseRotationInstructions.RLCA, 4, "RLCA A");

        instructions[0xFD08] = new Instruction("EX AF, AF'", 1, i => ProcessorMiscellaneousInstructions.EX_RR_R1R1(i, Register.A, Register.F), 4);

        instructions[0xFD0A] = new Instruction("LD A, (BC)'", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.BC), 7);

        instructions[0xFD0B] = new Instruction("DEC BC", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.BC), 6);

        instructions[0xFD0C] = new Instruction("INC C", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.C), 4);

        instructions[0xFD0D] = new Instruction("DEC C", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.C), 4);

        instructions[0xFD0E] = new Instruction("LD C, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.C), 7);

        instructions[0xFD0F] = new Instruction("RRCA", 1, ProcessorBitwiseRotationInstructions.RRCA, 4, "RRCA A");

        instructions[0xFD09] = new Instruction("ADD IY, BC", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IY, Register.BC), 11);

        instructions[0xFD10] = new Instruction("DJNZ e", 2, ProcessorBranchInstructions.DJNZ_e, 8, "DJNZ B, e");

        instructions[0xFD11] = new Instruction("LD DE, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.DE), 10);

        instructions[0xFD12] = new Instruction("LD (DE), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.DE, Register.A), 7);

        instructions[0xFD13] = new Instruction("INC DE", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.DE), 6);

        instructions[0xFD14] = new Instruction("INC D", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.D), 4);

        instructions[0xFD15] = new Instruction("DEC D", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.D), 4);

        instructions[0xFD16] = new Instruction("LD D, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.D), 7);

        instructions[0xFD17] = new Instruction("RLA", 1, ProcessorBitwiseRotationInstructions.RLA, 4, "RLA A");

        instructions[0xFD18] = new Instruction("JR C, e", 2, ProcessorBranchInstructions.JR_C_e, 7, null, 0xFD18);

        instructions[0xFD19] = new Instruction("ADD IY, DE", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IY, Register.DE), 11);

        instructions[0xFD1A] = new Instruction("LD A, (DE)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.DE), 7);

        instructions[0xFD1B] = new Instruction("DEC DE", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.DE), 6);

        instructions[0xFD1C] = new Instruction("INC E", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.E), 4);

        instructions[0xFD1D] = new Instruction("DEC E", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.E), 4);

        instructions[0xFD1E] = new Instruction("LD E, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.E), 7);

        instructions[0xFD1F] = new Instruction("RRA", 1, ProcessorBitwiseRotationInstructions.RRA, 4, "RRA A");

        instructions[0xFD20] = new Instruction("JR NZ, e", 2, ProcessorBranchInstructions.JR_NZ_e, 7);

        instructions[0xFD21] = new Instruction("LD IY, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.IY), 10);

        instructions[0xFD22] = new Instruction("LD (nn), IY", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.IY), 16);

        instructions[0xFD23] = new Instruction("INC IY", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.IY), 6);

        instructions[0xFD24] = new Instruction("INC IYh", 1, i => ProcessorArithmeticInstructions.INC_RRh(i, Register.IY), 4);

        instructions[0xFD25] = new Instruction("DEC IYh", 1, i => ProcessorArithmeticInstructions.DEC_RRh(i, Register.IY), 4);

        instructions[0xFD26] = new Instruction("LD IYh, n", 2, i => ProcessorLoadInstructions.LD_RRh_n(i, Register.IY), 7);

        instructions[0xFD27] = new Instruction("DAA", 1, ProcessorMiscellaneousInstructions.DAA, 4);

        instructions[0xFD28] = new Instruction("JR Z, e", 2, ProcessorBranchInstructions.JR_Z_e, 7);

        instructions[0xFD29] = new Instruction("ADD IY, IY", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IY, Register.IY), 11);

        instructions[0xFD2A] = new Instruction("LD IY, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.IY), 16);

        instructions[0xFD2B] = new Instruction("DEC IY", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.IY), 6);

        instructions[0xFD2C] = new Instruction("INC IYl", 1, i => ProcessorArithmeticInstructions.INC_RRl(i, Register.IY), 4);

        instructions[0xFD2D] = new Instruction("DEC IYl", 1, i => ProcessorArithmeticInstructions.DEC_RRl(i, Register.IY), 4);

        instructions[0xFD2E] = new Instruction("LD IYl, n", 2, i => ProcessorLoadInstructions.LD_RRl_n(i, Register.IY), 7);

        instructions[0xFD2F] = new Instruction("CPL", 1, ProcessorMiscellaneousInstructions.CPL, 4);

        instructions[0xFD30] = new Instruction("JR NC, e", 2, ProcessorBranchInstructions.JR_NC_e, 7);

        instructions[0xFD31] = new Instruction("LD SP, nn", 3, ProcessorLoadInstructions.LD_SP_nn, 10);

        instructions[0xFD32] = new Instruction("LD (nn), A", 3, i => ProcessorLoadInstructions.LD_addr_nn_R(i, Register.A), 13);

        instructions[0xFD33] = new Instruction("INC SP", 1, ProcessorArithmeticInstructions.INC_SP, 6);

        instructions[0xFD34] = new Instruction("INC (IY + d)", 2, i => ProcessorArithmeticInstructions.INC_addr_RR_plus_d(i, Register.IY), 19);

        instructions[0xFD35] = new Instruction("DEC (IY + d)", 2, i => ProcessorArithmeticInstructions.DEC_addr_RR_plus_d(i, Register.IY), 19);

        instructions[0xFD36] = new Instruction("LD (IY + d), n", 3, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_n(i, Register.IY), 15);

        instructions[0xFD37] = new Instruction("SCF", 1, ProcessorMiscellaneousInstructions.SCF, 4);

        instructions[0xFD38] = new Instruction("JR C, e", 2, ProcessorBranchInstructions.JR_C_e, 7);

        instructions[0xFD39] = new Instruction("ADD IY, SP", 1, i => ProcessorArithmeticInstructions.ADD_RR_SP(i, Register.IY), 11);

        instructions[0xFD3A] = new Instruction("LD A, (nn)", 3, i => ProcessorLoadInstructions.LD_R_addr_nn(i, Register.A), 13);

        instructions[0xFD3B] = new Instruction("DEC SP", 1, ProcessorArithmeticInstructions.DEC_SP, 6);

        instructions[0xFD3C] = new Instruction("INC A", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.A), 4);

        instructions[0xFD3D] = new Instruction("DEC A", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.A), 4);

        instructions[0xFD3E] = new Instruction("LD A, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.A), 7);

        instructions[0xFD3F] = new Instruction("CCF", 1, ProcessorMiscellaneousInstructions.CCF, 4);

        instructions[0xFD40] = new Instruction("LD B, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.B), 4);

        instructions[0xFD41] = new Instruction("LD B, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.C), 4);

        instructions[0xFD42] = new Instruction("LD B, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.D), 4);

        instructions[0xFD43] = new Instruction("LD B, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.E), 4);

        instructions[0xFD44] = new Instruction("LD B, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.B, Register.IY), 4);

        instructions[0xFD45] = new Instruction("LD B, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.B, Register.IY), 4);

        instructions[0xFD46] = new Instruction("LD B, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.B, Register.IY), 15);

        instructions[0xFD47] = new Instruction("LD B, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.A), 4);

        instructions[0xFD48] = new Instruction("LD C, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.B), 4);

        instructions[0xFD49] = new Instruction("LD C, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.C), 4);

        instructions[0xFD4A] = new Instruction("LD C, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.D), 4);

        instructions[0xFD4B] = new Instruction("LD C, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.E), 4);

        instructions[0xFD4C] = new Instruction("LD C, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.C, Register.IY), 4);

        instructions[0xFD4D] = new Instruction("LD C, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.C, Register.IY), 4);

        instructions[0xFD4E] = new Instruction("LD C, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.C, Register.IY), 15);

        instructions[0xFD4F] = new Instruction("LD C, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.A), 4);

        instructions[0xFD50] = new Instruction("LD D, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.B), 4);

        instructions[0xFD51] = new Instruction("LD D, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.C), 4);

        instructions[0xFD52] = new Instruction("LD D, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.D), 4);

        instructions[0xFD53] = new Instruction("LD D, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.E), 4);

        instructions[0xFD54] = new Instruction("LD D, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.D, Register.IY), 4);

        instructions[0xFD55] = new Instruction("LD D, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.D, Register.IY), 4);

        instructions[0xFD56] = new Instruction("LD D, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.D, Register.IY), 15);

        instructions[0xFD57] = new Instruction("LD D, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.A), 4);

        instructions[0xFD58] = new Instruction("LD E, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.B), 4);

        instructions[0xFD59] = new Instruction("LD E, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.C), 4);

        instructions[0xFD5A] = new Instruction("LD E, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.D), 4);

        instructions[0xFD5B] = new Instruction("LD E, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.E), 4);

        instructions[0xFD5C] = new Instruction("LD E, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.E, Register.IY), 4);

        instructions[0xFD5D] = new Instruction("LD E, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.E, Register.IY), 4);

        instructions[0xFD5E] = new Instruction("LD E, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.E, Register.IY), 15);

        instructions[0xFD5F] = new Instruction("LD E, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.A), 4);

        instructions[0xFD60] = new Instruction("LD IYh, B", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IY, Register.B), 4);

        instructions[0xFD61] = new Instruction("LD IYh, C", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IY, Register.C), 4);

        instructions[0xFD62] = new Instruction("LD IYh, D", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IY, Register.D), 4);

        instructions[0xFD63] = new Instruction("LD IYh, E", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IY, Register.E), 4);

        instructions[0xFD64] = new Instruction("LD IYh, IYh", 1, i => ProcessorLoadInstructions.LD_RRh_RRh(i, Register.IY, Register.IY), 4);

        instructions[0xFD65] = new Instruction("LD IYh, IYl", 1, i => ProcessorLoadInstructions.LD_RRh_RRl(i, Register.IY, Register.IY), 4);

        instructions[0xFD66] = new Instruction("LD H, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.H, Register.IY), 15);

        instructions[0xFD67] = new Instruction("LD IYh, A", 1, i => ProcessorLoadInstructions.LD_RRh_R(i, Register.IY, Register.A), 4);

        instructions[0xFD68] = new Instruction("LD IYl, B", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.B), 4);

        instructions[0xFD69] = new Instruction("LD IYl, C", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.C), 4);

        instructions[0xFD6A] = new Instruction("LD IYl, D", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.D), 4);

        instructions[0xFD6B] = new Instruction("LD IYl, E", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.E), 4);

        instructions[0xFD6C] = new Instruction("LD IYl, IYh", 1, i => ProcessorLoadInstructions.LD_RRl_RRh(i, Register.IY, Register.IY), 4);

        instructions[0xFD6D] = new Instruction("LD IYl, IYl", 1, i => ProcessorLoadInstructions.LD_RRl_RRl(i, Register.IY, Register.IY), 4);

        instructions[0xFD6E] = new Instruction("LD L, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.L, Register.IY), 15);

        instructions[0xFD6F] = new Instruction("LD IYl, A", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.A), 4);

        instructions[0xFD70] = new Instruction("LD (IY + d), B", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.B), 15);

        instructions[0xFD71] = new Instruction("LD (IY + d), C", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.C), 15);

        instructions[0xFD72] = new Instruction("LD (IY + d), D", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.D), 15);

        instructions[0xFD73] = new Instruction("LD (IY + d), E", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.E), 15);

        instructions[0xFD74] = new Instruction("LD (IY + d), H", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.H), 15);

        instructions[0xFD75] = new Instruction("LD (IY + d), L", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.L), 15);

        instructions[0xFD76] = new Instruction("HALT", 1, ProcessorMiscellaneousInstructions.HALT, 4);

        instructions[0xFD77] = new Instruction("LD (IY + d), A", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.A), 15);

        instructions[0xFD78] = new Instruction("LD A, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.B), 4);

        instructions[0xFD79] = new Instruction("LD A, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.C), 4);

        instructions[0xFD7A] = new Instruction("LD A, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.D), 4);

        instructions[0xFD7B] = new Instruction("LD A, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.E), 4);

        instructions[0xFD7C] = new Instruction("LD A, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD7D] = new Instruction("LD A, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD7E] = new Instruction("LD A, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD7F] = new Instruction("LD A, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.A), 4);

        instructions[0xFD80] = new Instruction("ADD A, B", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.B), 4);

        instructions[0xFD81] = new Instruction("ADD A, C", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.C), 4);

        instructions[0xFD82] = new Instruction("ADD A, D", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.D), 4);

        instructions[0xFD83] = new Instruction("ADD A, E", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.E), 4);

        instructions[0xFD84] = new Instruction("ADD A, IYh", 1, i => ProcessorArithmeticInstructions.ADD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD85] = new Instruction("ADD A, IYl", 1, i => ProcessorArithmeticInstructions.ADD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD86] = new Instruction("ADD A, (IY + d)", 2, i => ProcessorArithmeticInstructions.ADD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD87] = new Instruction("ADD A, A", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.A), 4);

        instructions[0xFD88] = new Instruction("ADC A, B", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.B), 4);

        instructions[0xFD89] = new Instruction("ADC A, C", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.C), 4);

        instructions[0xFD8A] = new Instruction("ADC A, D", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.D), 4);

        instructions[0xFD8B] = new Instruction("ADC A, E", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.E), 4);

        instructions[0xFD8C] = new Instruction("ADC A, IYh", 1, i => ProcessorArithmeticInstructions.ADC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD8D] = new Instruction("ADC A, IYl", 1, i => ProcessorArithmeticInstructions.ADC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD8E] = new Instruction("ADC A, (IY + d)", 2, i => ProcessorArithmeticInstructions.ADC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD8F] = new Instruction("ADC A, A", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.A), 4);

        instructions[0xFD90] = new Instruction("SUB A, B", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.B), 4);

        instructions[0xFD91] = new Instruction("SUB A, C", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.C), 4);

        instructions[0xFD92] = new Instruction("SUB A, D", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.D), 4);

        instructions[0xFD93] = new Instruction("SUB A, E", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.E), 4);

        instructions[0xFD94] = new Instruction("SUB A, IYh", 1, i => ProcessorArithmeticInstructions.SUB_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD95] = new Instruction("SUB A, IYl", 1, i => ProcessorArithmeticInstructions.SUB_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD96] = new Instruction("SUB A, (IY + d)", 2, i => ProcessorArithmeticInstructions.SUB_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD97] = new Instruction("SUB A, A", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.A), 4);

        instructions[0xFD98] = new Instruction("SBC A, B", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.B), 4);

        instructions[0xFD99] = new Instruction("SBC A, C", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.C), 4);

        instructions[0xFD9A] = new Instruction("SBC A, D", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.D), 4);

        instructions[0xFD9B] = new Instruction("SBC A, E", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.E), 4);

        instructions[0xFD9C] = new Instruction("SBC A, IYh", 1, i => ProcessorArithmeticInstructions.SBC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD9D] = new Instruction("SBC A, IYl", 1, i => ProcessorArithmeticInstructions.SBC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD9E] = new Instruction("SBC A, (IY + d)", 2, i => ProcessorArithmeticInstructions.SBC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD9F] = new Instruction("SBC A, A", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.A), 4);

        instructions[0xFDA0] = new Instruction("AND A, B", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.B), 4);

        instructions[0xFDA1] = new Instruction("AND A, C", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.C), 4);

        instructions[0xFDA2] = new Instruction("AND A, D", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.D), 4);

        instructions[0xFDA3] = new Instruction("AND A, E", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.E), 4);

        instructions[0xFDA4] = new Instruction("AND A, IYh", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDA5] = new Instruction("AND A, IYl", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDA6] = new Instruction("AND A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.AND_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDA7] = new Instruction("AND A, A", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.A), 4);

        instructions[0xFDA8] = new Instruction("XOR A, B", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.B), 4);

        instructions[0xFDA9] = new Instruction("XOR A, C", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.C), 4);

        instructions[0xFDAA] = new Instruction("XOR A, D", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.D), 4);

        instructions[0xFDAB] = new Instruction("XOR A, E", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.E), 4);

        instructions[0xFDAC] = new Instruction("XOR A, IYh", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDAD] = new Instruction("XOR A, IYl", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDAE] = new Instruction("XOR A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDAF] = new Instruction("XOR A, A", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.A), 4);

        instructions[0xFDB0] = new Instruction("OR A, B", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.B), 4);

        instructions[0xFDB1] = new Instruction("OR A, C", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.C), 4);

        instructions[0xFDB2] = new Instruction("OR A, D", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.D), 4);

        instructions[0xFDB3] = new Instruction("OR A, E", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.E), 4);

        instructions[0xFDB4] = new Instruction("OR A, IYh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDB5] = new Instruction("OR A, IYl", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDB6] = new Instruction("OR A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.OR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDB7] = new Instruction("OR A, A", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.A), 4);

        instructions[0xFDB8] = new Instruction("CP A, B", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.B), 4);

        instructions[0xFDB9] = new Instruction("CP A, C", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.C), 4);

        instructions[0xFDBA] = new Instruction("CP A, D", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.D), 4);

        instructions[0xFDBB] = new Instruction("CP A, E", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.E), 4);

        instructions[0xFDBC] = new Instruction("CP A, IYh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDBD] = new Instruction("CP A, IYl", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDBE] = new Instruction("CP A, (IY + d)", 2, i => ProcessorMiscellaneousInstructions.CP_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);
        
        instructions[0xFDBF] = new Instruction("CP A, A", 1, i => ProcessorMiscellaneousInstructions.CP_R_R(i, Register.A, Register.A), 4);

        instructions[0xFDC0] = new Instruction("RET NZ", 1, ProcessorBranchInstructions.RET_NZ, 5);

        instructions[0xFDC1] = new Instruction("POP BC", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.BC), 10);

        instructions[0xFDC2] = new Instruction("JP NZ, nn", 3, ProcessorBranchInstructions.JP_NZ_nn, 10);

        instructions[0xFDC3] = new Instruction("JP nn", 3, ProcessorBranchInstructions.JP_nn, 10);

        instructions[0xFDC4] = new Instruction("CALL NZ, nn", 3, ProcessorBranchInstructions.CALL_NZ_nn, 10, null);

        instructions[0xFDC5] = new Instruction("PUSH BC", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.BC), 11);

        instructions[0xFDC6] = new Instruction("ADD A, n", 2, i => ProcessorArithmeticInstructions.ADD_R_n(i, Register.A), 7);

        instructions[0xFDC7] = new Instruction("RST 0x00", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x00), 11);

        instructions[0xFDC8] = new Instruction("RET Z", 1, ProcessorBranchInstructions.RET_Z, 5);

        instructions[0xFDC9] = new Instruction("RET", 1, ProcessorBranchInstructions.RET, 10);

        instructions[0xFDCA] = new Instruction("JP Z, nn", 3, ProcessorBranchInstructions.JP_Z_nn, 10);
        
        instructions[0xFDCB] = new Instruction("SOPSET 0xFDCB", 1, _ => SetOpcodePrefix(0xFDCB), 4);

        instructions[0xFDCC] = new Instruction("CALL Z, nn", 3, ProcessorBranchInstructions.CALL_Z_nn, 10);

        instructions[0xFDCD] = new Instruction("CALL nn", 3, ProcessorBranchInstructions.CALL_nn, 17);

        instructions[0xFDCE] = new Instruction("ADC A, n", 2, i => ProcessorArithmeticInstructions.ADC_R_n(i, Register.A), 7);

        instructions[0xFDCF] = new Instruction("RST 0x08", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x08), 11);

        instructions[0xFDD0] = new Instruction("RET NC", 1, ProcessorBranchInstructions.RET_NC, 5);

        instructions[0xFDD1] = new Instruction("POP DE", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.DE), 10);

        instructions[0xFDD2] = new Instruction("JP NC, nn", 3, ProcessorBranchInstructions.JP_NC_nn, 10);

        instructions[0xFDD3] = new Instruction("OUT (n), A", 2, i => ProcessorMiscellaneousInstructions.OUT_addr_n_R(i, Register.A), 11);

        instructions[0xFDD4] = new Instruction("CALL NC, nn", 3, ProcessorBranchInstructions.CALL_NC_nn, 10);

        instructions[0xFDD5] = new Instruction("PUSH DE", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.DE), 11);

        instructions[0xFDD6] = new Instruction("SUB A, n", 2, i => ProcessorArithmeticInstructions.SUB_R_n(i, Register.A), 7);

        instructions[0xFDD7] = new Instruction("RST 0x10", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x10), 11);

        instructions[0xFDD8] = new Instruction("RET C", 1, ProcessorBranchInstructions.RET_C, 5);

        instructions[0xFDD9] = new Instruction("EXX", 1, ProcessorMiscellaneousInstructions.EXX, 4);

        instructions[0xFDDA] = new Instruction("JP C, nn", 3, ProcessorBranchInstructions.JP_C_nn, 10);

        instructions[0xFDDB] = new Instruction("IN A, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_R_p(i, Register.A), 11);

        instructions[0xFDDC] = new Instruction("CALL C, nn", 3, ProcessorBranchInstructions.CALL_C_nn, 10);

        instructions[0xFFDE] = new Instruction("SBC A, n", 2, i => ProcessorArithmeticInstructions.SBC_R_n(i, Register.A), 7);

        instructions[0xFFDF] = new Instruction("RST 0x18", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x18), 11);

        instructions[0xFFE0] = new Instruction("RET PO", 1, ProcessorBranchInstructions.RET_PO, 5);
        
        instructions[0xFDDD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xFDDE] = new Instruction("SBC A, n", 2, i => ProcessorArithmeticInstructions.SBC_R_n(i, Register.A), 7);

        instructions[0xFDDF] = new Instruction("RST 0x18", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x18), 11);

        instructions[0xFDE0] = new Instruction("RET PO", 1, ProcessorBranchInstructions.RET_PO, 5);

        instructions[0xFDE1] = new Instruction("POP IY", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.IY), 10);

        instructions[0xFDE2] = new Instruction("JP PO, nn", 3, ProcessorBranchInstructions.JP_PO_nn, 10);

        instructions[0xFDE3] = new Instruction("EX (SP), IY", 1, i => ProcessorMiscellaneousInstructions.EX_addr_SP_RR(i, Register.IY), 19);

        instructions[0xFDE4] = new Instruction("CALL PO, nn", 3, ProcessorBranchInstructions.CALL_PO_nn, 10);

        instructions[0xFDE5] = new Instruction("PUSH IY", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.IY), 11);

        instructions[0xFDE6] = new Instruction("AND A, n", 2, i => ProcessorBitwiseLogicInstructions.AND_R_n(i, Register.A), 7);

        instructions[0xFDE7] = new Instruction("RST 0x20", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x20), 11);

        instructions[0xFDE8] = new Instruction("RET PE", 1, ProcessorBranchInstructions.RET_PE, 5);

        instructions[0xFDE9] = new Instruction("JP (IY)", 1, i => ProcessorBranchInstructions.JP_addr_RR(i, Register.IY), 4);

        instructions[0xFDEA] = new Instruction("JP PE, nn", 3, ProcessorBranchInstructions.JP_PE_nn, 10);

        instructions[0xFDEB] = new Instruction("EX DE, HL", 1, i => ProcessorMiscellaneousInstructions.EX_RR_RR(i, Register.DE, Register.HL), 4);

        instructions[0xFDEC] = new Instruction("CALL PE, nn", 3, ProcessorBranchInstructions.CALL_PE_nn, 10);
        
        instructions[0xFDEE] = new Instruction("XOR A, n", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_n(i, Register.A), 7);

        instructions[0xFDEF] = new Instruction("RST 0x28", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x28), 11);

        instructions[0xFDF0] = new Instruction("RET NS", 1, ProcessorBranchInstructions.RET_NS, 5);

        instructions[0xFDF1] = new Instruction("POP AF", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.AF), 10);

        instructions[0xFDF2] = new Instruction("JP NS, nn", 3, ProcessorBranchInstructions.JP_NS_nn, 10);

        instructions[0xFDF3] = new Instruction("DI", 1, ProcessorMiscellaneousInstructions.DI, 4);

        instructions[0xFDF4] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_NS_nn, 10);

        instructions[0xFDF5] = new Instruction("PUSH AF", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.AF), 11);

        instructions[0xFDF6] = new Instruction("OR A, n", 2, i => ProcessorBitwiseLogicInstructions.OR_R_n(i, Register.A), 7);

        instructions[0xFDF7] = new Instruction("RST 0x30", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x30), 11);

        instructions[0xFDF8] = new Instruction("RET S", 1, ProcessorBranchInstructions.RET_S, 5);

        instructions[0xFDF9] = new Instruction("LD SP, IY", 1, i => ProcessorLoadInstructions.LD_SP_RR(i, Register.IY), 6);

        instructions[0xFDFA] = new Instruction("JP S, nn", 3, ProcessorBranchInstructions.JP_S_nn, 10);

        instructions[0xFDFB] = new Instruction("EI", 1, ProcessorMiscellaneousInstructions.EI, 4);

        instructions[0xFDFC] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_S_nn, 10);

        instructions[0xFDFD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);
        
        instructions[0xFDFE] = new Instruction("CP A, n", 2, i => ProcessorMiscellaneousInstructions.CP_R_n(i, Register.A), 7);

        instructions[0xFDFF] = new Instruction("RST 0x38", 1, i => ProcessorMiscellaneousInstructions.RST(i, 0x38), 11);
    }
}