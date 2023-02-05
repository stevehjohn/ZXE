using System.Text;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Tests.Console.Infrastructure;

public class FormattingTracer : ITracer
{
    private readonly List<string> _trace = new();

    public void Trace(string mnemonic, State state, Ram ram)
    {
        _trace.Add(FormatMnemonic(mnemonic));
    }

    public List<string> GetTrace()
    {
        return _trace;
    }

    public static string FormatMnemonic(string mnemonic)
    {
        var parts = mnemonic.Split(' ', StringSplitOptions.TrimEntries).Select(p => p.Replace(",", string.Empty)).ToArray();

        var builder = new StringBuilder();

        var padding = 15;

        builder.Append($"&White;{parts[0]}");

        padding -= parts[0].Length;

        if (parts.Length > 1)
        {
            builder.Append($" {ColourOperand(parts[1])}");

            padding -= parts[1].Length + 1;
        }

        if (parts.Length > 2)
        {
            builder.Append($"&White;, {ColourOperand(parts[2])}");

            padding -= parts[2].Length + 2;
        }

        return $"{builder}{new string(' ', padding)} X";
    }

    private static string ColourOperand(string operand)
    {
        var character = operand[0];

        if (character == '(')
        {
            character = operand[1];
        }

        if (char.IsUpper(character))
        {
            return $"&Magenta;{operand}";
        }

        return $"&Green;{operand}";
    }
}