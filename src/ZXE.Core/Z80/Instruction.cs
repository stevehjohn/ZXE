namespace ZXE.Core.Z80;

public class Instruction
{
    public string Mnemonic { get; }

    public byte Length { get; }

    public Action<Input> Action { get; }

    public int ClockCycles { get; }

    public Instruction(string mnemonic, byte length, Action<Input> action, int clockCycles)
    {
        Mnemonic = mnemonic;

        Length = length;

        Action = action;

        ClockCycles = clockCycles;
    }

    public string LogAction(State state)
    {
        // TODO: Replace variables in mnemonic with values from the CPU state
        return Mnemonic;
    }

    public override string ToString()
    {
        return Mnemonic;
    }
}