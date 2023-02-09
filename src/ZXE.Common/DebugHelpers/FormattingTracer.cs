using System.Diagnostics.CodeAnalysis;
using System.Text;
using ZXE.Common.Extensions;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Common.DebugHelpers;

[ExcludeFromCodeCoverage]
public class FormattingTracer : ITracer
{
    private readonly List<string> _trace = new();

    // TODO: Always show PC and SP.

    public void TraceBefore(Instruction instruction, byte[] data, State state, Ram ram)
    {
        _trace.Add($"{FormatMnemonic(instruction.Mnemonic)}{FormatState(instruction.HelperMnemonic ?? instruction.Mnemonic, data, state, ram)}");
    }

    public void TraceAfter(Instruction instruction, byte[] data, State state, Ram ram)
    {
        if (instruction.Mnemonic.StartsWith("SOPSET", StringComparison.InvariantCultureIgnoreCase))
        {
            _trace.Add(string.Empty);

            return;
        }

        _trace.Add($"{new string(' ', 20)}{FormatState(instruction.HelperMnemonic ?? instruction.Mnemonic, data, state, ram)}");

        _trace.Add(string.Empty);
    }

    public List<string> GetTrace()
    {
        return _trace;
    }

    private static string FormatMnemonic(string mnemonic)
    {
        var parts = GetMnemonicParts(mnemonic);

        var builder = new StringBuilder();

        var padding = 20;

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

        return $"{builder}{new string(' ', padding)}";
    }

    private static string ColourOperand(string operand)
    {
        var parts = operand.Split(' ');

        if (parts.Length == 1)
        {
            return ColourSimpleOperand(operand);
        }

        return ColourComplexOperand(operand);
    }

    private static string ColourSimpleOperand(string operand)
    {
        var character = operand[0];

        if (character == '0')
        {
            return $"&Magenta;{operand}";
        }

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

    private static string ColourComplexOperand(string operand)
    {
        var builder = new StringBuilder();

        foreach (var c in operand)
        {
            if (char.IsUpper(c))
            {
                builder.Append($"&Magenta;{c}");
            }
            else if (char.IsWhiteSpace(c))
            {
                builder.Append(" ");
            }
            else if (char.IsPunctuation(c))
            {
                builder.Append($"&Magenta;{c}");
            }
            else if (char.IsSymbol(c))
            {
                builder.Append($"&White;{c}");
            }
            else
            {
                builder.Append($"&Green;{c}");
            }
        }

        return builder.ToString();
    }

    private static string FormatState(string mnemonic, byte[] data, State state, Ram ram)
    {
        var parts = GetMnemonicParts(mnemonic);

        if (parts[0] == "SOPSET")
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        builder.Append($"&Cyan;PC&White;: &Yellow;0x{state.ProgramCounter:X4}");

        builder.Append($"    &Cyan;SP&White;: &Yellow;0x{state.StackPointer:X4}");

        builder.Append($"    &Cyan;Flags&White;: &Magenta;{state.Flags.GetFlags()}");

        if (parts.Length == 1 || parts[1][0] == '0')
        {
            return builder.ToString();
        }

        builder.Append($"    {FormatOperandData(parts[1], data, state, ram)}");

        if (parts.Length > 2)
        {
            builder.Append($"    {FormatOperandData(parts[2], data, state, ram)}");
        }

        return builder.ToString();
    }

    private static string FormatOperandData(string operand, byte[] data, State state, Ram ram)
    {
        var builder = new StringBuilder();

        var isIndirect = false;

        if (operand[0] == '(')
        {
            operand = operand[1..^1];

            isIndirect = true;
        }

        Register? register = null;

        if (char.IsUpper(operand[0]))
        {
            register = Enum.Parse<Register>(operand);
        }

        if (register != null)
        {
            if (operand.Length == 2)
            {
                builder.Append($"&Magenta;{operand,-4}&White;: &Yellow;0x{state.Registers.ReadPair((Register) register):X4}");
            }
            else
            {
                builder.Append($"&Magenta;{operand,-4}&White;: &Yellow;0x{state.Registers[(Register) register]:X2}  ");
            }
        }
        else
        {
            if (operand.Length == 2)
            {
                builder.Append($"&Green;{operand,-4}&White;: &Yellow;0x{data[2] << 8 | data[1]:X4}");
            }
            else
            {
                builder.Append($"&Green;{operand,-4}&White;: &Yellow;0x{data[1]:X2}  ");
            }
        }

        if (isIndirect)
        {
            builder.Append("    ");

            if (register != null)
            {
                builder.Append($"&Magenta;({operand})&White;: &Yellow;0x{ram[state.Registers.ReadPair((Register) register)]:X2}");
            }
            else
            {
                builder.Append($"&Green;({operand})&White;: &Yellow;0x{ram[data[2] << 8 | data[1]]:X2}");
            }
        }

        return builder.ToString();
    }

    private static string[] GetMnemonicParts(string mnemonic)
    {
        var parts = mnemonic.Split(' ', 2, StringSplitOptions.TrimEntries);

        if (parts.Length == 1)
        {
            return new[] { parts[0] };
        }

        var operands = parts[1].Split(',', StringSplitOptions.TrimEntries);

        if (operands.Length > 1)
        {
            return new[] { parts[0], operands[0], operands[1] };
        }

        return new[] { parts[0], operands[0] };
    }
}