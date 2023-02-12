namespace ZXE.Core.Z80;

public partial class Processor
{
    private void InitialiseBaseInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0x00] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.BC, Register.A), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.BC), 6);

        instructions[0x04] = new Instruction("INC B", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.B), 4);

        instructions[0x05] = new Instruction("DEC B", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.B), 4);

        instructions[0x06] = new Instruction("LD B, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, ProcessorBitwiseRotationInstructions.RLCA, 4, "RLCA A");

        instructions[0x08] = new Instruction("EX AF, AF'", 1, i => EX_RR_R1R1(i, Register.A, Register.F), 4);

        instructions[0x09] = new Instruction("ADD HL, BC", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.HL, Register.BC), 11);

        instructions[0x0A] = new Instruction("LD A, (BC)'", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.BC), 7);

        instructions[0x0B] = new Instruction("DEC BC", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.BC), 6);

        instructions[0x0C] = new Instruction("INC C", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.C), 4);

        instructions[0x0D] = new Instruction("DEC C", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.C), 4);

        instructions[0x0E] = new Instruction("LD C, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.C), 7);

        instructions[0x0F] = new Instruction("RRCA", 1, ProcessorBitwiseRotationInstructions.RRCA, 4, "RRCA A");

        instructions[0x10] = new Instruction("DJNZ e", 2, ProcessorBranchInstructions.DJNZ_e, 8, "DJNZ B, e");

        instructions[0x11] = new Instruction("LD DE, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.DE), 10);

        instructions[0x12] = new Instruction("LD (DE), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.DE, Register.A), 7);

        instructions[0x13] = new Instruction("INC DE", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.DE), 6);

        instructions[0x14] = new Instruction("INC D", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.D), 4);

        instructions[0x15] = new Instruction("DEC D", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.D), 4);

        instructions[0x16] = new Instruction("LD D, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.D), 7);

        instructions[0x17] = new Instruction("RLA", 1, ProcessorBitwiseRotationInstructions.RLA, 4, "RLA A");

        instructions[0x18] = new Instruction("JR e", 2, ProcessorBranchInstructions.JR_e, 12);

        instructions[0x19] = new Instruction("ADD HL, DE", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.HL, Register.DE), 11);

        instructions[0x1A] = new Instruction("LD A, (DE)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.DE), 7);

        instructions[0x1B] = new Instruction("DEC DE", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.DE), 6);

        instructions[0x1C] = new Instruction("INC E", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.E), 4);

        instructions[0x1D] = new Instruction("DEC E", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.E), 4);

        instructions[0x1E] = new Instruction("LD E, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.E), 7);

        instructions[0x1F] = new Instruction("RRA", 1, ProcessorBitwiseRotationInstructions.RRA, 4, "RRA A");

        instructions[0x20] = new Instruction("JR NZ, e", 2, ProcessorBranchInstructions.JR_NZ_e, 7);

        instructions[0x21] = new Instruction("LD HL, nn", 3, i => ProcessorLoadInstructions.LD_RR_nn(i, Register.HL), 10);

        instructions[0x22] = new Instruction("LD (nn), HL", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.HL), 16);

        instructions[0x23] = new Instruction("INC HL", 1, i => ProcessorArithmeticInstructions.INC_RR(i, Register.HL), 6);

        instructions[0x24] = new Instruction("INC H", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.H), 4);

        instructions[0x25] = new Instruction("DEC H", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.H), 4);

        instructions[0x26] = new Instruction("LD H, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.H), 7);

        instructions[0x27] = new Instruction("DAA", 1, DAA, 4);

        instructions[0x28] = new Instruction("JR Z, e", 2, ProcessorBranchInstructions.JR_Z_e, 7);

        instructions[0x29] = new Instruction("ADD HL, HL", 1, i => ProcessorArithmeticInstructions.ADD_RR_RR(i, Register.HL, Register.HL), 11);

        instructions[0x2A] = new Instruction("LD HL, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.HL), 16);

        instructions[0x2B] = new Instruction("DEC HL", 1, i => ProcessorArithmeticInstructions.DEC_RR(i, Register.HL), 6);

        instructions[0x2C] = new Instruction("INC L", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.L), 4);

        instructions[0x2D] = new Instruction("DEC L", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.L), 4);

        instructions[0x2E] = new Instruction("LD L, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.L), 7);

        instructions[0x2F] = new Instruction("CPL", 1, CPL, 4);

        instructions[0x30] = new Instruction("JR NC, e", 2, ProcessorBranchInstructions.JR_NC_e, 7);

        instructions[0x31] = new Instruction("LD SP, nn", 3, ProcessorLoadInstructions.LD_SP_nn, 10);

        instructions[0x32] = new Instruction("LD (nn), A", 3, i => ProcessorLoadInstructions.LD_addr_nn_R(i, Register.A), 13);

        instructions[0x33] = new Instruction("INC SP", 1, ProcessorArithmeticInstructions.INC_SP, 6);

        instructions[0x34] = new Instruction("INC (HL)", 1, i => ProcessorArithmeticInstructions.INC_addr_RR(i, Register.HL), 11);

        instructions[0x35] = new Instruction("DEC (HL)", 1, i => ProcessorArithmeticInstructions.DEC_addr_RR(i, Register.HL), 11);

        instructions[0x36] = new Instruction("LD (HL), n", 2, i => ProcessorLoadInstructions.LD_addr_RR_n(i, Register.HL), 10);

        instructions[0x37] = new Instruction("SCF", 1, SCF, 4);

        instructions[0x38] = new Instruction("JR C, e", 2, ProcessorBranchInstructions.JR_C_e, 7);

        instructions[0x39] = new Instruction("ADD HL, SP", 1, i => ProcessorArithmeticInstructions.ADD_RR_SP(i, Register.HL), 11);

        instructions[0x3A] = new Instruction("LD A, (nn)", 3, i => ProcessorLoadInstructions.LD_R_addr_nn(i, Register.A), 13);

        instructions[0x3B] = new Instruction("DEC SP", 1, ProcessorArithmeticInstructions.DEC_SP, 6);

        instructions[0x3C] = new Instruction("INC A", 1, i => ProcessorArithmeticInstructions.INC_R(i, Register.A), 4);

        instructions[0x3D] = new Instruction("DEC A", 1, i => ProcessorArithmeticInstructions.DEC_R(i, Register.A), 4);

        instructions[0x3E] = new Instruction("LD A, n", 2, i => ProcessorLoadInstructions.LD_R_n(i, Register.A), 7);

        instructions[0x3F] = new Instruction("CCF", 1, CCF, 4);

        instructions[0x40] = new Instruction("LD B, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.B), 4);

        instructions[0x41] = new Instruction("LD B, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.C), 4);

        instructions[0x42] = new Instruction("LD B, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.D), 4);

        instructions[0x43] = new Instruction("LD B, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.E), 4);

        instructions[0x44] = new Instruction("LD B, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.H), 4);

        instructions[0x45] = new Instruction("LD B, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.L), 4);

        instructions[0x46] = new Instruction("LD B, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.B, Register.HL), 7);

        instructions[0x47] = new Instruction("LD B, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.B, Register.A), 4);

        instructions[0x48] = new Instruction("LD C, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.B), 4);

        instructions[0x49] = new Instruction("LD C, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.C), 4);

        instructions[0x4A] = new Instruction("LD C, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.D), 4);

        instructions[0x4B] = new Instruction("LD C, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.E), 4);

        instructions[0x4C] = new Instruction("LD C, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.H), 4);

        instructions[0x4D] = new Instruction("LD C, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.L), 4);

        instructions[0x4E] = new Instruction("LD C, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.C, Register.HL), 7);

        instructions[0x4F] = new Instruction("LD C, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.C, Register.A), 4);

        instructions[0x50] = new Instruction("LD D, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.B), 4);

        instructions[0x51] = new Instruction("LD D, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.C), 4);

        instructions[0x52] = new Instruction("LD D, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.D), 4);

        instructions[0x53] = new Instruction("LD D, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.E), 4);

        instructions[0x54] = new Instruction("LD D, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.H), 4);

        instructions[0x55] = new Instruction("LD D, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.L), 4);

        instructions[0x56] = new Instruction("LD D, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.D, Register.HL), 7);

        instructions[0x57] = new Instruction("LD D, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.D, Register.A), 4);

        instructions[0x58] = new Instruction("LD E, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.B), 4);

        instructions[0x59] = new Instruction("LD E, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.C), 4);

        instructions[0x5A] = new Instruction("LD E, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.D), 4);

        instructions[0x5B] = new Instruction("LD E, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.E), 4);

        instructions[0x5C] = new Instruction("LD E, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.H), 4);

        instructions[0x5D] = new Instruction("LD E, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.L), 4);

        instructions[0x5E] = new Instruction("LD E, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.E, Register.HL), 7);

        instructions[0x5F] = new Instruction("LD E, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.E, Register.A), 4);

        instructions[0x60] = new Instruction("LD H, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.B), 4);

        instructions[0x61] = new Instruction("LD H, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.C), 4);

        instructions[0x62] = new Instruction("LD H, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.D), 4);

        instructions[0x63] = new Instruction("LD H, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.E), 4);

        instructions[0x64] = new Instruction("LD H, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.H), 4);

        instructions[0x65] = new Instruction("LD H, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.L), 4);

        instructions[0x66] = new Instruction("LD H, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.H, Register.HL), 7);

        instructions[0x67] = new Instruction("LD H, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.H, Register.A), 4);

        instructions[0x68] = new Instruction("LD L, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.B), 4);

        instructions[0x69] = new Instruction("LD L, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.C), 4);

        instructions[0x6A] = new Instruction("LD L, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.D), 4);

        instructions[0x6B] = new Instruction("LD L, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.E), 4);

        instructions[0x6C] = new Instruction("LD L, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.H), 4);

        instructions[0x6D] = new Instruction("LD L, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.L), 4);

        instructions[0x6E] = new Instruction("LD L, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.L, Register.HL), 7);

        instructions[0x6F] = new Instruction("LD L, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.L, Register.A), 4);

        instructions[0x70] = new Instruction("LD (HL), B", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.B), 7);

        instructions[0x71] = new Instruction("LD (HL), C", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.C), 7);

        instructions[0x72] = new Instruction("LD (HL), D", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.D), 7);

        instructions[0x73] = new Instruction("LD (HL), E", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.E), 7);

        instructions[0x74] = new Instruction("LD (HL), H", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.H), 7);

        instructions[0x75] = new Instruction("LD (HL), L", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.L), 7);

        instructions[0x76] = new Instruction("HALT", 1, HALT, 4);

        instructions[0x77] = new Instruction("LD (HL), A", 1, i => ProcessorLoadInstructions.LD_addr_RR_R(i, Register.HL, Register.A), 7);

        instructions[0x78] = new Instruction("LD A, B", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.B), 4);

        instructions[0x79] = new Instruction("LD A, C", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.C), 4);

        instructions[0x7A] = new Instruction("LD A, D", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.D), 4);

        instructions[0x7B] = new Instruction("LD A, E", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.E), 4);

        instructions[0x7C] = new Instruction("LD A, H", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.H), 4);

        instructions[0x7D] = new Instruction("LD A, L", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.L), 4);

        instructions[0x7E] = new Instruction("LD A, (HL)", 1, i => ProcessorLoadInstructions.LD_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x7F] = new Instruction("LD A, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.A), 4);

        instructions[0x80] = new Instruction("ADD A, B", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.B), 4);

        instructions[0x81] = new Instruction("ADD A, C", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.C), 4);

        instructions[0x82] = new Instruction("ADD A, D", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.D), 4);

        instructions[0x83] = new Instruction("ADD A, E", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.E), 4);

        instructions[0x84] = new Instruction("ADD A, H", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.H), 4);

        instructions[0x85] = new Instruction("ADD A, L", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.L), 4);

        instructions[0x86] = new Instruction("ADD A, (HL)", 1, i => ProcessorArithmeticInstructions.ADD_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x87] = new Instruction("ADD A, A", 1, i => ProcessorArithmeticInstructions.ADD_R_R(i, Register.A, Register.A), 4);

        instructions[0x88] = new Instruction("ADC A, B", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.B), 4);

        instructions[0x89] = new Instruction("ADC A, C", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.C), 4);

        instructions[0x8A] = new Instruction("ADC A, D", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.D), 4);

        instructions[0x8B] = new Instruction("ADC A, E", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.E), 4);

        instructions[0x8C] = new Instruction("ADC A, H", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.H), 4);

        instructions[0x8D] = new Instruction("ADC A, L", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.L), 4);

        instructions[0x8E] = new Instruction("ADC A, (HL)", 1, i => ProcessorArithmeticInstructions.ADC_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x8F] = new Instruction("ADC A, A", 1, i => ProcessorArithmeticInstructions.ADC_R_R(i, Register.A, Register.A), 4);

        instructions[0x90] = new Instruction("SUB A, B", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.B), 4);

        instructions[0x91] = new Instruction("SUB A, C", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.C), 4);

        instructions[0x92] = new Instruction("SUB A, D", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.D), 4);

        instructions[0x93] = new Instruction("SUB A, E", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.E), 4);

        instructions[0x94] = new Instruction("SUB A, H", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.H), 4);

        instructions[0x95] = new Instruction("SUB A, L", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.L), 4);

        instructions[0x96] = new Instruction("SUB A, (HL)", 1, i => ProcessorArithmeticInstructions.SUB_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x97] = new Instruction("SUB A, A", 1, i => ProcessorArithmeticInstructions.SUB_R_R(i, Register.A, Register.A), 4);

        instructions[0x98] = new Instruction("SBC A, B", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.B), 4);

        instructions[0x99] = new Instruction("SBC A, C", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.C), 4);

        instructions[0x9A] = new Instruction("SBC A, D", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.D), 4);

        instructions[0x9B] = new Instruction("SBC A, E", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.E), 4);

        instructions[0x9C] = new Instruction("SBC A, H", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.H), 4);

        instructions[0x9D] = new Instruction("SBC A, L", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.L), 4);

        instructions[0x9E] = new Instruction("SBC A, (HL)", 1, i => ProcessorArithmeticInstructions.SBC_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0x9F] = new Instruction("SBC A, A", 1, i => ProcessorArithmeticInstructions.SBC_R_R(i, Register.A, Register.A), 4);

        instructions[0xA0] = new Instruction("AND A, B", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.B), 4);

        instructions[0xA1] = new Instruction("AND A, C", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.C), 4);

        instructions[0xA2] = new Instruction("AND A, D", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.D), 4);

        instructions[0xA3] = new Instruction("AND A, E", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.E), 4);

        instructions[0xA4] = new Instruction("AND A, H", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.H), 4);

        instructions[0xA5] = new Instruction("AND A, L", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.L), 4);

        instructions[0xA6] = new Instruction("AND A, (HL)", 1, i => ProcessorBitwiseLogicInstructions.AND_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xA7] = new Instruction("AND A, A", 1, i => ProcessorBitwiseLogicInstructions.AND_R_R(i, Register.A, Register.A), 4);

        instructions[0xA8] = new Instruction("XOR A, B", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.B), 4);

        instructions[0xA9] = new Instruction("XOR A, C", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.C), 4);

        instructions[0xAA] = new Instruction("XOR A, D", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.D), 4);

        instructions[0xAB] = new Instruction("XOR A, E", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.E), 4);

        instructions[0xAC] = new Instruction("XOR A, H", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.H), 4);

        instructions[0xAD] = new Instruction("XOR A, L", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.L), 4);

        instructions[0xAE] = new Instruction("XOR A, (HL)", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xAF] = new Instruction("XOR A, A", 1, i => ProcessorBitwiseLogicInstructions.XOR_R_R(i, Register.A, Register.A), 4);

        instructions[0xB0] = new Instruction("OR A, B", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.B), 4);

        instructions[0xB1] = new Instruction("OR A, C", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.C), 4);

        instructions[0xB2] = new Instruction("OR A, D", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.D), 4);

        instructions[0xB3] = new Instruction("OR A, E", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.E), 4);

        instructions[0xB4] = new Instruction("OR A, H", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.H), 4);

        instructions[0xB5] = new Instruction("OR A, L", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.L), 4);

        instructions[0xB6] = new Instruction("OR A, (HL)", 1, i => ProcessorBitwiseLogicInstructions.OR_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xB7] = new Instruction("OR A, A", 1, i => ProcessorBitwiseLogicInstructions.OR_R_R(i, Register.A, Register.A), 4);

        instructions[0xB8] = new Instruction("CP A, B", 1, i => CP_R_R(i, Register.A, Register.B), 4);

        instructions[0xB9] = new Instruction("CP A, C", 1, i => CP_R_R(i, Register.A, Register.C), 4);

        instructions[0xBA] = new Instruction("CP A, D", 1, i => CP_R_R(i, Register.A, Register.D), 4);

        instructions[0xBB] = new Instruction("CP A, E", 1, i => CP_R_R(i, Register.A, Register.E), 4);

        instructions[0xBC] = new Instruction("CP A, H", 1, i => CP_R_R(i, Register.A, Register.H), 4);

        instructions[0xBD] = new Instruction("CP A, L", 1, i => CP_R_R(i, Register.A, Register.L), 4);

        instructions[0xBE] = new Instruction("CP A, (HL)", 1, i => CP_R_addr_RR(i, Register.A, Register.HL), 7);

        instructions[0xBF] = new Instruction("CP A, A", 1, i => CP_R_R(i, Register.A, Register.A), 4);

        instructions[0xC0] = new Instruction("RET NZ", 1, ProcessorBranchInstructions.RET_NZ, 5);

        instructions[0xC1] = new Instruction("POP BC", 1, i => POP_RR(i, Register.BC), 10);

        instructions[0xC2] = new Instruction("JP NZ, nn", 3, ProcessorBranchInstructions.JP_NZ_nn, 10);

        instructions[0xC3] = new Instruction("JP nn", 3, ProcessorBranchInstructions.JP_nn, 10);

        instructions[0xC4] = new Instruction("CALL NZ, nn", 3, ProcessorBranchInstructions.CALL_NZ_nn, 10);

        instructions[0xC5] = new Instruction("PUSH BC", 1, i => PUSH_RR(i, Register.BC), 11);

        instructions[0xC6] = new Instruction("ADD A, n", 2, i => ProcessorArithmeticInstructions.ADD_R_n(i, Register.A), 7);

        instructions[0xC7] = new Instruction("RST 0x00", 1, i => RST(i, 0x00), 11);

        instructions[0xC8] = new Instruction("RET Z", 1, ProcessorBranchInstructions.RET_Z, 5);

        instructions[0xC9] = new Instruction("RET", 1, ProcessorBranchInstructions.RET, 10);

        instructions[0xCA] = new Instruction("JP Z, nn", 3, ProcessorBranchInstructions.JP_Z_nn, 10);
                
        // Switch opcode set to CB
        instructions[0xCB] = new Instruction("SOPSET 0xCB", 1, _ => SetOpcodePrefix(0xCB), 4);

        instructions[0xCC] = new Instruction("CALL Z, nn", 3, ProcessorBranchInstructions.CALL_Z_nn, 10);

        instructions[0xCD] = new Instruction("CALL nn", 3, ProcessorBranchInstructions.CALL_nn, 17);

        instructions[0xCE] = new Instruction("ADC A, n", 2, i => ProcessorArithmeticInstructions.ADC_R_n(i, Register.A), 7);

        instructions[0xCF] = new Instruction("RST 0x08", 1, i => RST(i, 0x08), 11);

        instructions[0xD0] = new Instruction("RET NC", 1, ProcessorBranchInstructions.RET_NC, 5);

        instructions[0xD1] = new Instruction("POP DE", 1, i => POP_RR(i, Register.DE), 10);

        instructions[0xD2] = new Instruction("JP NC, nn", 3, ProcessorBranchInstructions.JP_NC_nn, 10);

        // instructions[0xD3] = new Instruction("OUT (n), A", 2, i => OUT_addr_n_R(i, Register.A), 11);

        instructions[0xD4] = new Instruction("CALL NC, nn", 3, ProcessorBranchInstructions.CALL_NC_nn, 10);

        instructions[0xD5] = new Instruction("PUSH DE", 1, i => PUSH_RR(i, Register.DE), 11);

        instructions[0xD6] = new Instruction("SUB A, n", 2, i => ProcessorArithmeticInstructions.SUB_R_n(i, Register.A), 7);

        instructions[0xD7] = new Instruction("RST 0x10", 1, i => RST(i, 0x10), 11);

        instructions[0xD8] = new Instruction("RET C", 1, ProcessorBranchInstructions.RET_C, 5);

        instructions[0xD9] = new Instruction("EXX", 1, EXX, 4);

        instructions[0xDA] = new Instruction("JP C, nn", 3, ProcessorBranchInstructions.JP_C_nn, 10);

        //instructions[0xDB] = new Instruction("IN A, (n)", 2, IN_R_addr_N, 11);

        instructions[0xDC] = new Instruction("CALL C, nn", 3, ProcessorBranchInstructions.CALL_C_nn, 10);

        // Switch opcode set to DD
        instructions[0xDD] = new Instruction("SOPSET 0xDD", 1, _ => SetOpcodePrefix(0xDD), 4);

        instructions[0xDE] = new Instruction("SBC A, n", 2, i => ProcessorArithmeticInstructions.SBC_R_n(i, Register.A), 7);

        instructions[0xDF] = new Instruction("RST 0x18", 1, i => RST(i, 0x18), 11);

        instructions[0xE0] = new Instruction("RET PO", 1, ProcessorBranchInstructions.RET_PO, 5);

        instructions[0xE1] = new Instruction("POP HL", 1, i => POP_RR(i, Register.HL), 10);

        instructions[0xE2] = new Instruction("JP PO, nn", 3, ProcessorBranchInstructions.JP_PO_nn, 10);

        instructions[0xE3] = new Instruction("EX (SP), HL", 1, i => EX_addr_SP_RR(i, Register.HL), 19);

        instructions[0xE4] = new Instruction("CALL PO, nn", 3, ProcessorBranchInstructions.CALL_PO_nn, 10);

        instructions[0xE5] = new Instruction("PUSH HL", 1, i => PUSH_RR(i, Register.HL), 11);

        instructions[0xE6] = new Instruction("AND A, n", 2, i => ProcessorBitwiseLogicInstructions.AND_R_n(i, Register.A), 7);

        instructions[0xE7] = new Instruction("RST 0x20", 1, i => RST(i, 0x20), 11);

        instructions[0xE8] = new Instruction("RET PE", 1, ProcessorBranchInstructions.RET_PE, 5);

        instructions[0xE9] = new Instruction("JP (HL)", 1, i => ProcessorBranchInstructions.JP_addr_RR(i, Register.HL), 4);

        instructions[0xEA] = new Instruction("JP PE, nn", 3, ProcessorBranchInstructions.JP_PE_nn, 10);

        instructions[0xEB] = new Instruction("EX DE, HL", 1, i => EX_RR_RR(i, Register.DE, Register.HL), 4);

        instructions[0xEC] = new Instruction("CALL PE, nn", 3, ProcessorBranchInstructions.CALL_PE_nn, 10);
        
        // Switch opcode set to ED
        instructions[0xED] = new Instruction("SOPSET 0xED", 1, _ => SetOpcodePrefix(0xED), 4);

        instructions[0xEE] = new Instruction("XOR A, n", 2, i => ProcessorBitwiseLogicInstructions.XOR_R_n(i, Register.A), 7);

        instructions[0xEF] = new Instruction("RST 0x28", 1, i => RST(i, 0x28), 11);

        instructions[0xF0] = new Instruction("RET NS", 1, ProcessorBranchInstructions.RET_NS, 5);

        instructions[0xF1] = new Instruction("POP AF", 1, i => POP_RR(i, Register.AF), 10);

        instructions[0xF2] = new Instruction("JP NS, nn", 3, ProcessorBranchInstructions.JP_NS_nn, 10);

        instructions[0xF3] = new Instruction("DI", 1, DI, 4);

        instructions[0xF4] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_NS_nn, 10);

        instructions[0xF5] = new Instruction("PUSH AF", 1, i => PUSH_RR(i, Register.AF), 11);

        instructions[0xF6] = new Instruction("OR A, n", 2, i => ProcessorBitwiseLogicInstructions.OR_R_n(i, Register.A), 7);

        instructions[0xF7] = new Instruction("RST 0x30", 1, i => RST(i, 0x30), 11);

        instructions[0xF8] = new Instruction("RET S", 1, ProcessorBranchInstructions.RET_S, 5);

        instructions[0xF9] = new Instruction("LD SP, HL", 1, ProcessorLoadInstructions.LD_RR_RR, 6);

        instructions[0xFA] = new Instruction("JP S, nn", 3, ProcessorBranchInstructions.JP_S_nn, 10);

        instructions[0xFB] = new Instruction("EI", 1, EI, 4);

        instructions[0xFC] = new Instruction("CALL S, nn", 3, ProcessorBranchInstructions.CALL_S_nn, 10);
        
        // Switch opcode set to FD
        instructions[0xFD] = new Instruction("SOPSET 0xFD", 1, _ => SetOpcodePrefix(0xFD), 4);

        instructions[0xFE] = new Instruction("CP A, n", 2, i => CP_R_n(i, Register.A), 7);

        instructions[0xFF] = new Instruction("RST 0x38", 1, i => RST(i, 0x38), 11);
    }
}