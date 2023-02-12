// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
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
}