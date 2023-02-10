﻿namespace ZXE.Core.Z80;

public class Instruction
{
    public string Mnemonic { get; }

    public string? HelperMnemonic { get; }

    public byte Length { get; }

    public Func<Input, bool> Action { get; }

    public int ClockCycles { get; }

    // TODO: Go back over all instructions to add this. Cry while doing so.
    public int? Opcode { get; }

    public Instruction(string mnemonic,  byte length, Func<Input, bool> action, int clockCycles, string? helperMnemonic = null, int? opcode = null)
    {
        Mnemonic = mnemonic;

        Length = length;

        Action = action;

        ClockCycles = clockCycles;

        HelperMnemonic = helperMnemonic;

        Opcode = opcode;
    }
}