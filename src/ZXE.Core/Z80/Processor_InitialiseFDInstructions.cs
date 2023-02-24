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

        instructions[0xFD26] = new Instruction("LD IYh, n", 2, i => ProcessorLoadInstructions.LD_RRh_n(i, Register.IY), 4);

        instructions[0xFD27] = new Instruction("DAA", 1, ProcessorMiscellaneousInstructions.DAA, 4);

        instructions[0xFD28] = new Instruction("JR Z, e", 2, ProcessorBranchInstructions.JR_Z_e, 7);

        instructions[0xFD29] = new Instruction("ADD IY, IY", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.IY, Register.IY), 11);

        instructions[0xFD2A] = new Instruction("LD IY, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.IY), 11);

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

        instructions[0xFD4C] = new Instruction("LD C, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.C, Register.IY), 4);

        instructions[0xFD4D] = new Instruction("LD C, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.C, Register.IY), 4);

        instructions[0xFD4E] = new Instruction("LD C, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.C, Register.IY), 15);

        instructions[0xFD54] = new Instruction("LD D, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.D, Register.IY), 4);

        instructions[0xFD55] = new Instruction("LD D, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.D, Register.IY), 4);

        instructions[0xFD56] = new Instruction("LD D, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.D, Register.IY), 15);

        instructions[0xFD5C] = new Instruction("LD E, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.E, Register.IY), 4);

        instructions[0xFD5D] = new Instruction("LD E, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.E, Register.IY), 4);

        instructions[0xFD5E] = new Instruction("LD E, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.E, Register.IY), 15);

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

        instructions[0xFD6E] = new Instruction("LD L, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.L, Register.IY), 4);

        instructions[0xFD6F] = new Instruction("LD IYl, A", 1, i => ProcessorLoadInstructions.LD_RRl_R(i, Register.IY, Register.A), 4);

        instructions[0xFD70] = new Instruction("LD (IY + d), B", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.B), 15);

        instructions[0xFD71] = new Instruction("LD (IY + d), C", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.C), 15);

        instructions[0xFD72] = new Instruction("LD (IY + d), D", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.D), 15);

        instructions[0xFD73] = new Instruction("LD (IY + d), E", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.E), 15);

        instructions[0xFD74] = new Instruction("LD (IY + d), H", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.H), 15);

        instructions[0xFD75] = new Instruction("LD (IY + d), L", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.L), 15);

        instructions[0xFD77] = new Instruction("LD (IY + d), A", 2, i => ProcessorLoadInstructions.LD_addr_RR_plus_d_R(i, Register.IY, Register.A), 15);

        instructions[0xFD7C] = new Instruction("LD A, IYh", 1, i => ProcessorLoadInstructions.LD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD7D] = new Instruction("LD A, IYl", 1, i => ProcessorLoadInstructions.LD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD7E] = new Instruction("LD A, (IY + d)", 2, i => ProcessorLoadInstructions.LD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD84] = new Instruction("ADD A, IYh", 1, i => ProcessorArithmeticInstructions.ADD_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD85] = new Instruction("ADD A, IYl", 1, i => ProcessorArithmeticInstructions.ADD_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD86] = new Instruction("ADD A, (IY + d)", 2, i => ProcessorArithmeticInstructions.ADD_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD8C] = new Instruction("ADC A, IYh", 1, i => ProcessorArithmeticInstructions.ADC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD8D] = new Instruction("ADC A, IYl", 1, i => ProcessorArithmeticInstructions.ADC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD8E] = new Instruction("ADC A, (IY + d)", 2, i => ProcessorArithmeticInstructions.ADC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD94] = new Instruction("SUB A, IYh", 1, i => ProcessorArithmeticInstructions.SUB_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD95] = new Instruction("SUB A, IYl", 1, i => ProcessorArithmeticInstructions.SUB_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD96] = new Instruction("SUB A, (IY + d)", 2, i => ProcessorArithmeticInstructions.SUB_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFD9C] = new Instruction("SBC A, IYh", 1, i => ProcessorArithmeticInstructions.SBC_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFD9D] = new Instruction("SBC A, IYl", 1, i => ProcessorArithmeticInstructions.SBC_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFD9E] = new Instruction("SBC A, (IY + d)", 2, i => ProcessorArithmeticInstructions.SBC_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDA4] = new Instruction("AND A, IYh", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDA5] = new Instruction("AND A, IYl", 1, i => ProcessorBitwiseLogicInstructions.AND_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDA6] = new Instruction("AND A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.AND_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDAC] = new Instruction("XOR A, IYh", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDAD] = new Instruction("XOR A, IYl", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDAE] = new Instruction("XOR A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDB4] = new Instruction("OR A, IYh", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDB5] = new Instruction("OR A, IYl", 1, i => ProcessorBitwiseLogicInstructions.OR_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDB6] = new Instruction("OR A, (IY + d)", 2, i => ProcessorBitwiseLogicInstructions.OR_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);

        instructions[0xFDBC] = new Instruction("CP A, IYh", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRh(i, Register.A, Register.IY), 4);

        instructions[0xFDBD] = new Instruction("CP A, IYl", 1, i => ProcessorMiscellaneousInstructions.CP_R_RRl(i, Register.A, Register.IY), 4);

        instructions[0xFDBE] = new Instruction("CP A, (IY + d)", 2, i => ProcessorMiscellaneousInstructions.CP_R_addr_RR_plus_d(i, Register.A, Register.IY), 15);
        
        instructions[0xFDCB] = new Instruction("SOPSET 0xFDCB", 1, _ => SetOpcodePrefix(0xFDCB), 4);

        instructions[0xFDDD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);

        instructions[0xFDE1] = new Instruction("POP IY", 1, i => ProcessorMiscellaneousInstructions.POP_RR(i, Register.IY), 10);

        instructions[0xFDE3] = new Instruction("EX (SP), IY", 1, i => ProcessorMiscellaneousInstructions.EX_addr_SP_RR(i, Register.IY), 19);

        instructions[0xFDE5] = new Instruction("PUSH IY", 1, i => ProcessorMiscellaneousInstructions.PUSH_RR(i, Register.IY), 11);

        instructions[0xFDE9] = new Instruction("JP (IY)", 1, i => ProcessorBranchInstructions.JP_addr_RR(i, Register.IY), 4);

        instructions[0xFDF9] = new Instruction("LD SP, IY", 1, i => ProcessorLoadInstructions.LD_SP_RR(i, Register.IY), 6);

        instructions[0xFDFD] = new Instruction("NOP", 1, _ => ProcessorMiscellaneousInstructions.NOP(), 4);
    }
}