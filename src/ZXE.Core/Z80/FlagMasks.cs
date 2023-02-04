namespace ZXE.Core.Z80;

public enum FlagMasks
{
    Carry = 0x01,
    AddSubtract = 0x02,
    ParityOverflow = 0x04,
    // Not used
    HalfCarry = 0x10,
    // Not used
    Zero = 0x40,
    Sign = 0x80
}