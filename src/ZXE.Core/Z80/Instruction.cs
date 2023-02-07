namespace ZXE.Core.Z80;

public class Instruction
{
    public string Mnemonic { get; }

    public string? HelperMnemonic { get; }

    public byte Length { get; }

    public Func<Input, bool> Action { get; }

    public int ClockCycles { get; }

    public Instruction(string mnemonic,  byte length, Func<Input, bool> action, int clockCycles, string? helperMnemonic = null)
    {
        Mnemonic = mnemonic;

        Length = length;

        Action = action;

        ClockCycles = clockCycles;

        HelperMnemonic = helperMnemonic;
    }
}