// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public partial class Processor
{
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
}