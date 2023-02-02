using ZXE.Core.System;

namespace ZXE.Core.Infrastructure;

public class Instruction
{
    public string Mnemonic { get; }

    public byte Length { get; }

    public Action<byte[], CpuState> Action { get; }

    public Instruction(string mnemonic, byte length, Action<byte[], CpuState> action)
    {
        Mnemonic = mnemonic;

        Action = action;
    }

    public string LogAction(CpuState cpuState)
    {
        // TODO: Replace variables in mnemonic with values from the CPU state
        return Mnemonic;
    }

    public override string ToString()
    {
        return Mnemonic;
    }
}