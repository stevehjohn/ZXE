using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class Instruction
{
    public string Mnemonic { get; }

    public byte Length { get; }

    public Action<byte[], State> Action { get; }

    public Instruction(string mnemonic, byte length, Action<byte[], State> action)
    {
        Mnemonic = mnemonic;

        Action = action;
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