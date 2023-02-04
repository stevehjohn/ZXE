namespace ZXE.Core.Z80;

public static class Flags
{
    public const byte Carry          = 0x01; // C
    public const byte AddSubtract    = 0x02; // N
    public const byte ParityOverflow = 0x04; // P/V
    public const byte X1             = 0x08; // Supposedly not used
    public const byte HalfCarry      = 0x10; // H
    public const byte X2             = 0x20; // Supposedly not used
    public const byte Zero           = 0x40; // Z
    public const byte Sign           = 0x80; // S
}