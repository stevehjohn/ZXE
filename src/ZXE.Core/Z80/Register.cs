// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public enum Register
{
    A   = 0x0000,
    F   = 0x0001,
    B   = 0x0002,
    C   = 0x0003,
    D   = 0x0004,
    E   = 0x0005,
    H   = 0x0006,
    L   = 0x0007,
    A1  = 0x0008,
    F1  = 0x0009,
    B1  = 0x000A,
    C1  = 0x000B,
    D1  = 0x000C,
    E1  = 0x000D,
    H1  = 0x000E,
    L1  = 0x000F,
    I   = 0x0010,
    R   = 0x0011,
    IX  = 0x1213,
    IY  = 0x1415,
    AF =  0x0001,
    BC  = 0x0203,
    DE  = 0x0405,
    HL  = 0x0607,
    BC1 = 0x0A0B,
    DE1 = 0x0C0D,
    HL1 = 0x0E0F
}