// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
    private static void InitialiseDDCBInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0xDDCB00] = new Instruction("RLC (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB00);

        instructions[0xDDCB01] = new Instruction("RLC (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB01);

        instructions[0xDDCB02] = new Instruction("RLC (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB02);

        instructions[0xDDCB03] = new Instruction("RLC (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB03);

        instructions[0xDDCB04] = new Instruction("RLC (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB04);

        instructions[0xDDCB05] = new Instruction("RLC (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB05);

        instructions[0xDDCB06] = new Instruction("RLC (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB06);

        instructions[0xDDCB07] = new Instruction("RLC (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.RLC_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB07);

        instructions[0xDDCB08] = new Instruction("RRC (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB08);

        instructions[0xDDCB09] = new Instruction("RRC (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB09);

        instructions[0xDDCB0A] = new Instruction("RRC (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB0A);

        instructions[0xDDCB0B] = new Instruction("RRC (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB0B);

        instructions[0xDDCB0C] = new Instruction("RRC (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB0C);

        instructions[0xDDCB0D] = new Instruction("RRC (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB0D);

        instructions[0xDDCB0E] = new Instruction("RRC (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB0E);

        instructions[0xDDCB0F] = new Instruction("RRC (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.RRC_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB0F);

        instructions[0xDDCB10] = new Instruction("RL (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB10);

        instructions[0xDDCB11] = new Instruction("RL (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB11);

        instructions[0xDDCB12] = new Instruction("RL (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB12);

        instructions[0xDDCB13] = new Instruction("RL (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB13);

        instructions[0xDDCB14] = new Instruction("RL (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB14);

        instructions[0xDDCB15] = new Instruction("RL (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB15);

        instructions[0xDDCB16] = new Instruction("RL (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB16);

        instructions[0xDDCB17] = new Instruction("RL (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.RL_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB17);

        instructions[0xDDCB18] = new Instruction("RR (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB18);

        instructions[0xDDCB19] = new Instruction("RR (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB19);

        instructions[0xDDCB1A] = new Instruction("RR (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB1A);

        instructions[0xDDCB1B] = new Instruction("RR (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB1B);

        instructions[0xDDCB1C] = new Instruction("RR (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB1C);

        instructions[0xDDCB1D] = new Instruction("RR (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB1D);

        instructions[0xDDCB1E] = new Instruction("RR (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB1E);

        instructions[0xDDCB1F] = new Instruction("RR (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.RR_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB1F);

        instructions[0xDDCB20] = new Instruction("SLA (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB20);

        instructions[0xDDCB21] = new Instruction("SLA (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB21);

        instructions[0xDDCB22] = new Instruction("SLA (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB22);

        instructions[0xDDCB23] = new Instruction("SLA (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB23);

        instructions[0xDDCB24] = new Instruction("SLA (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB24);

        instructions[0xDDCB25] = new Instruction("SLA (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB25);

        instructions[0xDDCB26] = new Instruction("SLA (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB26);

        instructions[0xDDCB27] = new Instruction("SLA (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.SLA_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB27);

        instructions[0xDDCB28] = new Instruction("SRA (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB28);

        instructions[0xDDCB29] = new Instruction("SRA (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB29);

        instructions[0xDDCB2A] = new Instruction("SRA (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB2A);

        instructions[0xDDCB2B] = new Instruction("SRA (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB2B);

        instructions[0xDDCB2C] = new Instruction("SRA (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB2C);

        instructions[0xDDCB2D] = new Instruction("SRA (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB2D);

        instructions[0xDDCB2E] = new Instruction("SRA (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB2E);

        instructions[0xDDCB2F] = new Instruction("SRA (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.SRA_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB2F);

        instructions[0xDDCB30] = new Instruction("SLS (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB30);

        instructions[0xDDCB31] = new Instruction("SLS (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB31);

        instructions[0xDDCB32] = new Instruction("SLS (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB32);

        instructions[0xDDCB33] = new Instruction("SLS (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB33);

        instructions[0xDDCB34] = new Instruction("SLS (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB34);

        instructions[0xDDCB35] = new Instruction("SLS (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB35);

        instructions[0xDDCB36] = new Instruction("SLS (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB36);

        instructions[0xDDCB37] = new Instruction("SLS (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.SLS_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB37);

        instructions[0xDDCB38] = new Instruction("SRL (IX + d), B", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.B), 15, null, 0xDDCB38);

        instructions[0xDDCB39] = new Instruction("SRL (IX + d), C", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.C), 15, null, 0xDDCB39);

        instructions[0xDDCB3A] = new Instruction("SRL (IX + d), D", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.D), 15, null, 0xDDCB3A);

        instructions[0xDDCB3B] = new Instruction("SRL (IX + d), E", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.E), 15, null, 0xDDCB3B);

        instructions[0xDDCB3C] = new Instruction("SRL (IX + d), H", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.H), 15, null, 0xDDCB3C);

        instructions[0xDDCB3D] = new Instruction("SRL (IX + d), L", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.L), 15, null, 0xDDCB3D);

        instructions[0xDDCB3E] = new Instruction("SRL (IX + d)", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d(i, Register.IX), 15, null, 0xDDCB3E);

        instructions[0xDDCB3F] = new Instruction("SRL (IX + d), A", 2, i => ProcessorBitwiseRotationInstructions.SRL_addr_RR_plus_d_R(i, Register.IX, Register.A), 15, null, 0xDDCB3F);

        instructions[0xDDCB40] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB40);

        instructions[0xDDCB41] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB41);

        instructions[0xDDCB42] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB42);

        instructions[0xDDCB43] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB43);

        instructions[0xDDCB44] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB44);

        instructions[0xDDCB45] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB45);

        instructions[0xDDCB46] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB46);

        instructions[0xDDCB47] = new Instruction("BIT_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x01, Register.IX), 12, null, 0xDDCB47);

        instructions[0xDDCB48] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB48);

        instructions[0xDDCB49] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB49);

        instructions[0xDDCB4A] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4A);

        instructions[0xDDCB4B] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4B);

        instructions[0xDDCB4C] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4C);

        instructions[0xDDCB4D] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4D);

        instructions[0xDDCB4E] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4E);

        instructions[0xDDCB4F] = new Instruction("BIT_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x02, Register.IX), 12, null, 0xDDCB4F);

        instructions[0xDDCB50] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB50);

        instructions[0xDDCB51] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB51);

        instructions[0xDDCB52] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB52);

        instructions[0xDDCB53] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB53);

        instructions[0xDDCB54] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB54);

        instructions[0xDDCB55] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB55);

        instructions[0xDDCB56] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB56);

        instructions[0xDDCB57] = new Instruction("BIT_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x04, Register.IX), 12, null, 0xDDCB57);

        instructions[0xDDCB58] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB58);

        instructions[0xDDCB59] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB59);

        instructions[0xDDCB5A] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5A);

        instructions[0xDDCB5B] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5B);

        instructions[0xDDCB5C] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5C);

        instructions[0xDDCB5D] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5D);

        instructions[0xDDCB5E] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5E);

        instructions[0xDDCB5F] = new Instruction("BIT_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x08, Register.IX), 12, null, 0xDDCB5F);

        instructions[0xDDCB60] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB60);

        instructions[0xDDCB61] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB61);

        instructions[0xDDCB62] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB62);

        instructions[0xDDCB63] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB63);

        instructions[0xDDCB64] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB64);

        instructions[0xDDCB65] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB65);

        instructions[0xDDCB66] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB66);

        instructions[0xDDCB67] = new Instruction("BIT_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x10, Register.IX), 12, null, 0xDDCB67);

        instructions[0xDDCB68] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB68);

        instructions[0xDDCB69] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB69);

        instructions[0xDDCB6A] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6A);

        instructions[0xDDCB6B] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6B);

        instructions[0xDDCB6C] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6C);

        instructions[0xDDCB6D] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6D);

        instructions[0xDDCB6E] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6E);

        instructions[0xDDCB6F] = new Instruction("BIT_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB6F);

        instructions[0xDDCB70] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB70);

        instructions[0xDDCB71] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB71);

        instructions[0xDDCB72] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB72);

        instructions[0xDDCB73] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB73);

        instructions[0xDDCB74] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB74);

        instructions[0xDDCB75] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB75);

        instructions[0xDDCB76] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB76);

        instructions[0xDDCB77] = new Instruction("BIT_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x40, Register.IX), 12, null, 0xDDCB77);

        instructions[0xDDCB78] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB78);

        instructions[0xDDCB79] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB79);

        instructions[0xDDCB7A] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7A);

        instructions[0xDDCB7B] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7B);

        instructions[0xDDCB7C] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7C);

        instructions[0xDDCB7D] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7D);

        instructions[0xDDCB7E] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7E);

        instructions[0xDDCB7F] = new Instruction("BIT_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.BIT_b_addr_RR_plus_d(i, 0x20, Register.IX), 12, null, 0xDDCB7F);

        instructions[0xDDCB80] = new Instruction("RES_0 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.B), 15, null, 0xDDCB80);

        instructions[0xDDCB81] = new Instruction("RES_0 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.C), 15, null, 0xDDCB81);

        instructions[0xDDCB82] = new Instruction("RES_0 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.D), 15, null, 0xDDCB82);

        instructions[0xDDCB83] = new Instruction("RES_0 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.E), 15, null, 0xDDCB83);

        instructions[0xDDCB84] = new Instruction("RES_0 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.H), 15, null, 0xDDCB84);

        instructions[0xDDCB85] = new Instruction("RES_0 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.L), 15, null, 0xDDCB85);

        instructions[0xDDCB86] = new Instruction("RES_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x01, Register.IX), 15, null, 0xDDCB86);

        instructions[0xDDCB87] = new Instruction("RES_0 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.A), 15, null, 0xDDCB87);

        instructions[0xDDCB88] = new Instruction("RES_1 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.B), 15, null, 0xDDCB88);

        instructions[0xDDCB89] = new Instruction("RES_1 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.C), 15, null, 0xDDCB89);

        instructions[0xDDCB8A] = new Instruction("RES_1 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.D), 15, null, 0xDDCB8A);

        instructions[0xDDCB8B] = new Instruction("RES_1 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.E), 15, null, 0xDDCB8B);

        instructions[0xDDCB8C] = new Instruction("RES_1 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.H), 15, null, 0xDDCB8C);

        instructions[0xDDCB8D] = new Instruction("RES_1 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.L), 15, null, 0xDDCB8D);

        instructions[0xDDCB8E] = new Instruction("RES_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x02, Register.IX), 15, null, 0xDDCB8E);

        instructions[0xDDCB8F] = new Instruction("RES_1 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.A), 15, null, 0xDDCB8F);

        instructions[0xDDCB90] = new Instruction("RES_2 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.B), 15, null, 0xDDCB90);

        instructions[0xDDCB91] = new Instruction("RES_2 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.C), 15, null, 0xDDCB91);

        instructions[0xDDCB92] = new Instruction("RES_2 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.D), 15, null, 0xDDCB92);

        instructions[0xDDCB93] = new Instruction("RES_2 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.E), 15, null, 0xDDCB93);

        instructions[0xDDCB94] = new Instruction("RES_2 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.H), 15, null, 0xDDCB94);

        instructions[0xDDCB95] = new Instruction("RES_2 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.L), 15, null, 0xDDCB95);

        instructions[0xDDCB96] = new Instruction("RES_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x04, Register.IX), 15, null, 0xDDCB96);

        instructions[0xDDCB97] = new Instruction("RES_2 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.A), 15, null, 0xDDCB97);
    
        instructions[0xDDCB98] = new Instruction("RES_3 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.B), 15, null, 0xDDCB98);

        instructions[0xDDCB99] = new Instruction("RES_3 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.C), 15, null, 0xDDCB99);

        instructions[0xDDCB9A] = new Instruction("RES_3 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.D), 15, null, 0xDDCB9A);

        instructions[0xDDCB9B] = new Instruction("RES_3 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.E), 15, null, 0xDDCB9B);

        instructions[0xDDCB9C] = new Instruction("RES_3 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.H), 15, null, 0xDDCB9C);

        instructions[0xDDCB9D] = new Instruction("RES_3 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.L), 15, null, 0xDDCB9D);

        instructions[0xDDCB9E] = new Instruction("RES_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x08, Register.IX), 15, null, 0xDDCB9E);

        instructions[0xDDCB9F] = new Instruction("RES_3 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.A), 15, null, 0xDDCB9F);

        instructions[0xDDCBA0] = new Instruction("RES_4 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.B), 15, null, 0xDDCBA0);

        instructions[0xDDCBA1] = new Instruction("RES_4 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.C), 15, null, 0xDDCBA1);

        instructions[0xDDCBA2] = new Instruction("RES_4 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.D), 15, null, 0xDDCBA2);

        instructions[0xDDCBA3] = new Instruction("RES_4 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.E), 15, null, 0xDDCBA3);

        instructions[0xDDCBA4] = new Instruction("RES_4 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.H), 15, null, 0xDDCBA4);

        instructions[0xDDCBA5] = new Instruction("RES_4 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.L), 15, null, 0xDDCBA5);

        instructions[0xDDCBA6] = new Instruction("RES_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x10, Register.IX), 15, null, 0xDDCBA6);

        instructions[0xDDCBA7] = new Instruction("RES_4 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.A), 15, null, 0xDDCBA7);

        instructions[0xDDCBA8] = new Instruction("RES_5 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.B), 15, null, 0xDDCBA8);

        instructions[0xDDCBA9] = new Instruction("RES_5 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.C), 15, null, 0xDDCBA9);

        instructions[0xDDCBAA] = new Instruction("RES_5 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.D), 15, null, 0xDDCBAA);

        instructions[0xDDCBAB] = new Instruction("RES_5 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.E), 15, null, 0xDDCBAB);

        instructions[0xDDCBAC] = new Instruction("RES_5 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.H), 15, null, 0xDDCBAC);

        instructions[0xDDCBAD] = new Instruction("RES_5 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.L), 15, null, 0xDDCBAD);

        instructions[0xDDCBAE] = new Instruction("RES_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x20, Register.IX), 15, null, 0xDDCBAE);

        instructions[0xDDCBAF] = new Instruction("RES_5 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.A), 15, null, 0xDDCBAF);

        instructions[0xDDCBB0] = new Instruction("RES_6 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.B), 15, null, 0xDDCBB0);

        instructions[0xDDCBB1] = new Instruction("RES_6 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.C), 15, null, 0xDDCBB1);

        instructions[0xDDCBB2] = new Instruction("RES_6 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.D), 15, null, 0xDDCBB2);

        instructions[0xDDCBB3] = new Instruction("RES_6 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.E), 15, null, 0xDDCBB3);

        instructions[0xDDCBB4] = new Instruction("RES_6 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.H), 15, null, 0xDDCBB4);

        instructions[0xDDCBB5] = new Instruction("RES_6 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.L), 15, null, 0xDDCBB5);

        instructions[0xDDCBB6] = new Instruction("RES_6, (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x40, Register.IX), 15, null, 0xDDCBB6);

        instructions[0xDDCBB7] = new Instruction("RES_6 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.A), 15, null, 0xDDCBB7);
    
        instructions[0xDDCBB8] = new Instruction("RES_7 (IX + d), B", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.B), 15, null, 0xDDCBB8);

        instructions[0xDDCBB9] = new Instruction("RES_7 (IX + d), C", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.C), 15, null, 0xDDCBB9);

        instructions[0xDDCBBA] = new Instruction("RES_7 (IX + d), D", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.D), 15, null, 0xDDCBBA);

        instructions[0xDDCBBB] = new Instruction("RES_7 (IX + d), E", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.E), 15, null, 0xDDCBBB);

        instructions[0xDDCBBC] = new Instruction("RES_7 (IX + d), H", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.H), 15, null, 0xDDCBBC);

        instructions[0xDDCBBD] = new Instruction("RES_7 (IX + d), L", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.L), 15, null, 0xDDCBBD);

        instructions[0xDDCBBE] = new Instruction("RES_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d(i, 0x80, Register.IX), 15, null, 0xDDCBBE);

        instructions[0xDDCBBF] = new Instruction("RES_7 (IX + d), A", 2, i => ProcessorBitOperationInstructions.RES_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.A), 15, null, 0xDDCBBF);

        instructions[0xDDCBC0] = new Instruction("SET_0 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.B), 15, null, 0xDDCBC0);

        instructions[0xDDCBC1] = new Instruction("SET_0 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.C), 15, null, 0xDDCBC1);

        instructions[0xDDCBC2] = new Instruction("SET_0 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.D), 15, null, 0xDDCBC2);

        instructions[0xDDCBC3] = new Instruction("SET_0 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.E), 15, null, 0xDDCBC3);

        instructions[0xDDCBC4] = new Instruction("SET_0 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.H), 15, null, 0xDDCBC4);

        instructions[0xDDCBC5] = new Instruction("SET_0 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.L), 15, null, 0xDDCBC5);

        instructions[0xDDCBC6] = new Instruction("SET_0 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x01, Register.IX), 15, null, 0xDDCBC6);

        instructions[0xDDCBC7] = new Instruction("SET_0 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x01, Register.IX, Register.A), 15, null, 0xDDCBC7);

        instructions[0xDDCBC8] = new Instruction("SET_1 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.B), 15, null, 0xDDCBC8);

        instructions[0xDDCBC9] = new Instruction("SET_1 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.C), 15, null, 0xDDCBC9);

        instructions[0xDDCBCA] = new Instruction("SET_1 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.D), 15, null, 0xDDCBCA);

        instructions[0xDDCBCB] = new Instruction("SET_1 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.E), 15, null, 0xDDCBCB);

        instructions[0xDDCBCC] = new Instruction("SET_1 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.H), 15, null, 0xDDCBCC);

        instructions[0xDDCBCD] = new Instruction("SET_1 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.L), 15, null, 0xDDCBCD);

        instructions[0xDDCBCE] = new Instruction("SET_1 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x02, Register.IX), 15, null, 0xDDCBCE);

        instructions[0xDDCBCF] = new Instruction("SET_1 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x02, Register.IX, Register.A), 15, null, 0xDDCBCF);
    
        instructions[0xDDCBD0] = new Instruction("SET_2 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.B), 15, null, 0xDDCBD0);

        instructions[0xDDCBD1] = new Instruction("SET_2 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.C), 15, null, 0xDDCBD1);

        instructions[0xDDCBD2] = new Instruction("SET_2 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.D), 15, null, 0xDDCBD2);

        instructions[0xDDCBD3] = new Instruction("SET_2 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.E), 15, null, 0xDDCBD3);

        instructions[0xDDCBD4] = new Instruction("SET_2 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.H), 15, null, 0xDDCBD4);

        instructions[0xDDCBD5] = new Instruction("SET_2 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.L), 15, null, 0xDDCBD5);

        instructions[0xDDCBD6] = new Instruction("SET_2 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x04, Register.IX), 15, null, 0xDDCBD6);

        instructions[0xDDCBD7] = new Instruction("SET_2 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x04, Register.IX, Register.A), 15, null, 0xDDCBD7);

        instructions[0xDDCBD8] = new Instruction("SET_3 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.B), 15, null, 0xDDCBD8);

        instructions[0xDDCBD9] = new Instruction("SET_3 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.C), 15, null, 0xDDCBD9);

        instructions[0xDDCBDA] = new Instruction("SET_3 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.D), 15, null, 0xDDCBDA);

        instructions[0xDDCBDB] = new Instruction("SET_3 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.E), 15, null, 0xDDCBDB);

        instructions[0xDDCBDC] = new Instruction("SET_3 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.H), 15, null, 0xDDCBDC);

        instructions[0xDDCBDD] = new Instruction("SET_3 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.L), 15, null, 0xDDCBDD);

        instructions[0xDDCBDE] = new Instruction("SET_3 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x08, Register.IX), 15, null, 0xDDCBDE);

        instructions[0xDDCBDF] = new Instruction("SET_3 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x08, Register.IX, Register.A), 15, null, 0xDDCBDF);    

        instructions[0xDDCBE0] = new Instruction("SET_4 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.B), 15, null, 0xDDCBE0);

        instructions[0xDDCBE1] = new Instruction("SET_4 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.C), 15, null, 0xDDCBE1);

        instructions[0xDDCBE2] = new Instruction("SET_4 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.D), 15, null, 0xDDCBE2);

        instructions[0xDDCBE3] = new Instruction("SET_4 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.E), 15, null, 0xDDCBE3);

        instructions[0xDDCBE4] = new Instruction("SET_4 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.H), 15, null, 0xDDCBE4);

        instructions[0xDDCBE5] = new Instruction("SET_4 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.L), 15, null, 0xDDCBE5);

        instructions[0xDDCBE6] = new Instruction("SET_4 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x10, Register.IX), 15, null, 0xDDCBE6);

        instructions[0xDDCBE7] = new Instruction("SET_4 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x10, Register.IX, Register.A), 15, null, 0xDDCBE7);

        instructions[0xDDCBE8] = new Instruction("SET_5 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.B), 15, null, 0xDDCBE8);

        instructions[0xDDCBE9] = new Instruction("SET_5 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.C), 15, null, 0xDDCBE9);

        instructions[0xDDCBEA] = new Instruction("SET_5 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.D), 15, null, 0xDDCBEA);

        instructions[0xDDCBEB] = new Instruction("SET_5 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.E), 15, null, 0xDDCBEB);

        instructions[0xDDCBEC] = new Instruction("SET_5 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.H), 15, null, 0xDDCBEC);

        instructions[0xDDCBED] = new Instruction("SET_5 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.L), 15, null, 0xDDCBED);

        instructions[0xDDCBEE] = new Instruction("SET_5 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x20, Register.IX), 15, null, 0xDDCBEE);

        instructions[0xDDCBEF] = new Instruction("SET_5 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x20, Register.IX, Register.A), 15, null, 0xDDCBEF);     

        instructions[0xDDCBF0] = new Instruction("SET_6 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.B), 15, null, 0xDDCBF0);

        instructions[0xDDCBF1] = new Instruction("SET_6 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.C), 15, null, 0xDDCBF1);

        instructions[0xDDCBF2] = new Instruction("SET_6 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.D), 15, null, 0xDDCBF2);

        instructions[0xDDCBF3] = new Instruction("SET_6 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.E), 15, null, 0xDDCBF3);

        instructions[0xDDCBF4] = new Instruction("SET_6 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.H), 15, null, 0xDDCBF4);

        instructions[0xDDCBF5] = new Instruction("SET_6 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.L), 15, null, 0xDDCBF5);

        instructions[0xDDCBF6] = new Instruction("SET_6 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x40, Register.IX), 15, null, 0xDDCBF6);

        instructions[0xDDCBF7] = new Instruction("SET_6 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x40, Register.IX, Register.A), 15, null, 0xDDCBF7);

        instructions[0xDDCBF8] = new Instruction("SET_7 (IX + d), B", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.B), 15, null, 0xDDCBF8);

        instructions[0xDDCBF9] = new Instruction("SET_7 (IX + d), C", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.C), 15, null, 0xDDCBF9);

        instructions[0xDDCBFA] = new Instruction("SET_7 (IX + d), D", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.D), 15, null, 0xDDCBFA);

        instructions[0xDDCBFB] = new Instruction("SET_7 (IX + d), E", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.E), 15, null, 0xDDCBFB);

        instructions[0xDDCBFC] = new Instruction("SET_7 (IX + d), H", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.H), 15, null, 0xDDCBFC);

        instructions[0xDDCBFD] = new Instruction("SET_7 (IX + d), L", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.L), 15, null, 0xDDCBFD);

        instructions[0xDDCBFE] = new Instruction("SET_7 (IX + d)", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d(i, 0x80, Register.IX), 15, null, 0xDDCBFE);

        instructions[0xDDCBFF] = new Instruction("SET_7 (IX + d), A", 2, i => ProcessorBitOperationInstructions.SET_b_addr_RR_plus_d_R(i, 0x80, Register.IX, Register.A), 15, null, 0xDDCBFF);        
    }
}