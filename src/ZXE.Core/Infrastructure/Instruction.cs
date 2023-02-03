using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class Instruction
{
    public string Mnemonic { get; }

    public byte Length { get; }

    public Action<byte[], State, Ram> Action { get; }

    public Instruction(string mnemonic, byte length, Action<byte[], State, Ram> action)
    {
        Mnemonic = mnemonic;

        Length = length;

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