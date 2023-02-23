// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;
    
public partial class Processor
{
    private static void InitialiseEDInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xED00] = new Instruction("IN_0 B, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.B), 8, null, 0x0ED00);

        instructions[0xED01] = new Instruction("OUT_0 (n), B", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.B), 8, null, 0xED01);

        instructions[0xED04] = new Instruction("TST B", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.B), 6, null, 0xED04);

        instructions[0xED08] = new Instruction("IN_0 C, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.C), 8, null, 0x0ED08);

        instructions[0xED09] = new Instruction("OUT_0 (n), C", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.C), 8, null, 0xED09);

        instructions[0xED0C] = new Instruction("TST C", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.C), 6, null, 0xED0C);
        
        instructions[0xED10] = new Instruction("IN_0 D, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.D), 8, null, 0x0ED10);

        instructions[0xED11] = new Instruction("OUT_0 (n), D", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.D), 8, null, 0xED11);

        instructions[0xED14] = new Instruction("TST D", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.B), 6, null, 0xED14);

        instructions[0xED18] = new Instruction("IN_0 E, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.E), 8, null, 0x0ED18);

        instructions[0xED19] = new Instruction("OUT_0 (n), E", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.E), 8, null, 0xED19);

        instructions[0xED1C] = new Instruction("TST E", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.E), 6, null, 0xED1C);

        instructions[0xED20] = new Instruction("IN_0 H, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.H), 8, null, 0x0ED20);

        instructions[0xED21] = new Instruction("OUT_0 (n), H", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.H), 8, null, 0xED21);

        instructions[0xED24] = new Instruction("TST H", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.H), 6, null, 0xED24);

        instructions[0xED28] = new Instruction("IN_0 L, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.L), 8, null, 0x0ED28);

        instructions[0xED29] = new Instruction("OUT_0 (n), L", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.L), 8, null, 0xED29);

        instructions[0xED2C] = new Instruction("TST L", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.L), 6, null, 0xED2C);

        instructions[0xED34] = new Instruction("TST (HL)", 1, i => ProcessorMiscellaneousInstructions.TST_addr_R(i, Register.HL), 6, null, 0xED34);

        instructions[0xED38] = new Instruction("IN_0 A, (n)", 2, i => ProcessorMiscellaneousInstructions.IN_b_R_addr_n(i, Register.A), 8, null, 0x0ED38);

        instructions[0xED39] = new Instruction("OUT_0 (n), A", 2, i => ProcessorMiscellaneousInstructions.OUT_b_addr_n_R(i, Register.A), 8, null, 0xED39);

        instructions[0xED3C] = new Instruction("TST A", 1, i => ProcessorMiscellaneousInstructions.TST_R(i, Register.A), 6, null, 0xED3C);

        instructions[0xED40] = new Instruction("IN B, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.B), 8, null, 0xED40);

        instructions[0xED41] = new Instruction("OUT (BC), B", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED41);

        instructions[0xED42] = new Instruction("SBC HL, BC", 1, i => ProcessorArithmeticInstructions.SBC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED42);

        instructions[0xED43] = new Instruction("LD (nn), BC", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.BC), 16, null, 0xED43);

        instructions[0xED44] = new Instruction("NEG A", 1, i => ProcessorBitOperationInstructions.NEG_R(i, Register.A), 4, null, 0xED44);

        // TODO: instructions[0xED45] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED45);

        instructions[0xED46] = new Instruction("IM 0", 1, i => ProcessorMiscellaneousInstructions.IM_m(i, InterruptMode.Mode0), 4, null, 0xED46);

        instructions[0xED47] = new Instruction("LD I, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.I, Register.A), 5, null, 0xED47);

        instructions[0xED48] = new Instruction("IN C, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.C), 8, null, 0xED48);

        instructions[0xED49] = new Instruction("OUT (BC), B", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.B), 8, null, 0xED49);

        instructions[0xED4A] = new Instruction("ADC HL, BC", 1, i => ProcessorArithmeticInstructions.ADC_RR_RR(i, Register.HL, Register.BC), 11, null, 0xED4A);

        instructions[0xED4B] = new Instruction("LD BC, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.BC), 16, null, 0xED4B);

        instructions[0xED4C] = new Instruction("NEG A", 1, i => ProcessorBitOperationInstructions.NEG_R(i, Register.A), 4, null, 0xED4C);

        // TODO: Which is right? instructions[0xED4C] = new Instruction("MLT BC", 1, i => MLT_RR(i, Register.BC), 13, null, 0xED4C);

        // TODO: instructions[0xED4D] = new Instruction("RETI", 1, i => (i, Register.BC), 10, null, 0xED4D);

        instructions[0xED4F] = new Instruction("LD R, A", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.R, Register.A), 5, null, 0xED4F);
        
        instructions[0xED50] = new Instruction("IN D, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.D), 8, null, 0xED50);

        instructions[0xED51] = new Instruction("OUT (BC), D", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.D), 8, null, 0xED51);

        instructions[0xED52] = new Instruction("SBC HL, DE", 1, i => ProcessorArithmeticInstructions.SBC_RR_RR(i, Register.HL, Register.DE), 11, null, 0xED52);

        instructions[0xED53] = new Instruction("LD (nn), DE", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.DE), 16, null, 0xED53);

        // TODO: Exists? instructions[0xED54] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED45);

        // TODO: instructions[0xED55] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED55);

        instructions[0xED56] = new Instruction("IM 1", 1, i => ProcessorMiscellaneousInstructions.IM_m(i, InterruptMode.Mode1), 4, null, 0xED56);

        instructions[0xED57] = new Instruction("LD A, I", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.I), 5, null, 0xED57);

        instructions[0xED58] = new Instruction("IN E, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.E), 8, null, 0xED58);

        instructions[0xED59] = new Instruction("OUT (BC), E", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.E), 8, null, 0xED59);

        instructions[0xED5A] = new Instruction("ADC HL, DE", 1, i => ProcessorArithmeticInstructions.ADC_RR_RR(i, Register.HL, Register.DE), 11, null, 0xED5A);

        instructions[0xED5B] = new Instruction("LD DE, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.DE), 16, null, 0xED5B);

        instructions[0xED5C] = new Instruction("NEG A", 1, i => ProcessorBitOperationInstructions.NEG_R(i, Register.A), 4, null, 0xED5C);

        instructions[0xED5E] = new Instruction("IM 2", 1, i => ProcessorMiscellaneousInstructions.IM_m(i, InterruptMode.Mode2), 5, null, 0xED5E);

        instructions[0xED5F] = new Instruction("LD A, R", 1, i => ProcessorLoadInstructions.LD_R_R(i, Register.A, Register.R), 5, null, 0xED5F);

        instructions[0xED60] = new Instruction("IN H, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.H), 8, null, 0xED60);

        instructions[0xED61] = new Instruction("OUT (BC), H", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.H), 8, null, 0xED61);

        instructions[0xED62] = new Instruction("SBC HL, HL", 1, i => ProcessorArithmeticInstructions.SBC_RR_RR(i, Register.HL, Register.HL), 11, null, 0xED62);

        instructions[0xED63] = new Instruction("LD (nn), HL", 3, i => ProcessorLoadInstructions.LD_addr_nn_RR(i, Register.HL), 16, null, 0xED63);

        // TODO: Exists? instructions[0xED64] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED64);

        // TODO: instructions[0xED65] = new Instruction("RETN", 1, i => (i, InterruptMode.Mode0), 10, null, 0xED65);

        // TODO: instructions[0xED67] = new Instruction("RRD", 1, i => (i, Register.A, Register.I), 5, null, 0xED67);

        instructions[0xED68] = new Instruction("IN L, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.L), 8, null, 0xED68);

        instructions[0xED69] = new Instruction("OUT (BC), L", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.L), 8, null, 0xED69);

        instructions[0xED6A] = new Instruction("ADC HL, HL", 1, i => ProcessorArithmeticInstructions.ADC_RR_RR(i, Register.HL, Register.HL), 11, null, 0xED6A);

        instructions[0xED6B] = new Instruction("LD HL, (nn)", 3, i => ProcessorLoadInstructions.LD_RR_addr_nn(i, Register.HL), 16, null, 0xED6B);

        // TODO: instructions[0xED6C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED6C);

        // TODO: instructions[0xED6F] = new Instruction("RLD", 1, i => (i, Register.A, Register.R), 5, null, 0xED6F);

        instructions[0xED70] = new Instruction("IN (BC)", 1, i => ProcessorMiscellaneousInstructions.IN_addr_RR(i, Register.BC), 8, null, 0xED70);

        instructions[0xED71] = new Instruction("OUT (BC), 0", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_R_n(i, Register.BC, 0), 8, null, 0xED71);

        instructions[0xED72] = new Instruction("SBC HL, SP", 1, i => ProcessorArithmeticInstructions.SBC_RR_SP(i, Register.HL), 11, null, 0xED72);

        instructions[0xED73] = new Instruction("LD (nn), SP", 3, ProcessorLoadInstructions.LD_addr_nn_SP, 16, null, 0xED73);

        // TODO: Exists? instructions[0xED74] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED74);

        instructions[0xED76] = new Instruction("IM 1", 1, i => ProcessorMiscellaneousInstructions.IM_m(i, InterruptMode.Mode1), 4, null, 0xED76);

        instructions[0xED78] = new Instruction("IN A, (C)", 1, i => ProcessorMiscellaneousInstructions.IN_R_C(i, Register.A), 8, null, 0xED78);

        instructions[0xED79] = new Instruction("OUT (BC), A", 1, i => ProcessorMiscellaneousInstructions.OUT_addr_RR_R(i, Register.BC, Register.A), 8, null, 0xED79);

        instructions[0xED7A] = new Instruction("ADC HL, SP", 1, i => ProcessorArithmeticInstructions.ADC_RR_SP(i, Register.HL), 11, null, 0xED7A);

        instructions[0xED7B] = new Instruction("LD SP, (nn)", 3, ProcessorLoadInstructions.LD_SP_addr_nn, 16, null, 0xED7B);

        // TODO: instructions[0xED7C] = new Instruction("NEG A", 1, i => NEG_R(i, Register.A), 4, null, 0xED7C);

        instructions[0xED7E] = new Instruction("IM 2", 1, i => ProcessorMiscellaneousInstructions.IM_m(i, InterruptMode.Mode2), 4, null, 0xED7E);

        instructions[0xEDA0] = new Instruction("LDI", 1, ProcessorMiscellaneousInstructions.LDI, 12, null, 0xEDA0);

        instructions[0xEDA1] = new Instruction("CPI", 1, ProcessorMiscellaneousInstructions.CPI, 12, null, 0xEDA1);

        instructions[0xEDA2] = new Instruction("INI", 1, ProcessorMiscellaneousInstructions.INI, 12, null, 0xEDA2);

        instructions[0xEDA3] = new Instruction("OUTI", 1, ProcessorMiscellaneousInstructions.OUTI, 12, null, 0xEDA3);

        instructions[0xEDA3] = new Instruction("OUTI", 1, ProcessorMiscellaneousInstructions.OUTI, 12, null, 0xEDA3);

        instructions[0xEDA8] = new Instruction("LDD", 1, ProcessorMiscellaneousInstructions.LDD, 12, null, 0xEDA8);

        instructions[0xEDA9] = new Instruction("CPD", 1, ProcessorMiscellaneousInstructions.CPD, 12, null, 0xEDA9);

        instructions[0xEDAA] = new Instruction("IND", 1, ProcessorMiscellaneousInstructions.IND, 12, null, 0xEDAA);

        instructions[0xEDAB] = new Instruction("OUTD", 1, ProcessorMiscellaneousInstructions.OUTD, 12, null, 0xEDAB);

        instructions[0xEDB0] = new Instruction("LDIR", 1, ProcessorMiscellaneousInstructions.LDIR, 12, null, 0xEDB0);

        instructions[0xEDB1] = new Instruction("CPIR", 1, ProcessorMiscellaneousInstructions.CPIR, 12, "CPIR BC HL", 0xEDB1);

        instructions[0xEDB2] = new Instruction("INIR", 1, ProcessorMiscellaneousInstructions.INIR, 12, null, 0xEDB2);

        instructions[0xEDB3] = new Instruction("OTIR", 1, ProcessorMiscellaneousInstructions.OTIR, 12, "OTIR B", 0xEDB3);

        instructions[0xEDA8] = new Instruction("LDD", 1, ProcessorMiscellaneousInstructions.LDD, 12, null, 0xEDA8);

        instructions[0xEDB8] = new Instruction("LDDR", 1, ProcessorMiscellaneousInstructions.LDDR, 12, null, 0xEDB8);

        instructions[0xEDB9] = new Instruction("CPDR", 1, ProcessorMiscellaneousInstructions.CPDR, 12, null, 0xEDB9);

        instructions[0xEDBA] = new Instruction("INDR", 1, ProcessorMiscellaneousInstructions.INDR, 12, null, 0xEDBA);

        instructions[0xEDBB] = new Instruction("OTDR", 1, ProcessorMiscellaneousInstructions.OTDR, 12, null, 0xEDBB);
    }
}