namespace ZXE.Core.Z80;

public class Instruction
{
    public string Mnemonic { get; }

    public string? HelperMnemonic { get; }

    public byte Length { get; }

    public Action<Input> Action { get; }

    public int ClockCycles { get; }

    public Instruction(string mnemonic,  byte length, Action<Input> action, int clockCycles, string? helperMnemonic = null)
    {
        Mnemonic = mnemonic;

        Length = length;

        Action = action;

        ClockCycles = clockCycles;

        HelperMnemonic = helperMnemonic;
    }
}