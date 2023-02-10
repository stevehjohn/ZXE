using System.Text;
using ZXE.Common.ConsoleHelpers;
using ZXE.Utilities.Infrastructure;
using ZXE.Utilities.Utilities;

namespace ZXE.Utilities;

public static class Entrypoint
{
    //public static void Main(string[] args)
    public static void Main()
    {
        var disassembler = new Disassembler();

        disassembler.LoadData(File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 48K\\image-0.rom"));

        var code = disassembler.Disassemble();

        FormattedConsole.WriteLine(string.Empty);

        foreach (var line in code)
        {
            FormattedConsole.WriteLine($"  &Cyan;{line.Address:X4}&White;: {FormatMnemonic(line)}");
        }
    }

    private static string FormatMnemonic(CodeLine line)
    {
        var space = line.Mnemonic.IndexOf(' ');

        if (space < 0)
        {
            return $"&Green;{line.Mnemonic}";
        }

        var name = line.Mnemonic[..line.Mnemonic.IndexOf(' ')];

        var operands = line.Mnemonic[space..].Split(',', StringSplitOptions.TrimEntries);

        var builder = new StringBuilder();

        builder.Append($"&Green;{name} {FormatOperand(operands[0], line)}");

        if (operands.Length > 1)
        {
            builder.Append($"&White;, {FormatOperand(operands[1], line)}");
        }

        return builder.ToString();
    }

    // TODO: So much duplication...
    private static string FormatOperand(string operand, CodeLine line)
    {
        var indirect = false;

        if (operand[0] == '(')
        {
            operand = operand[1..^1];

            indirect = true;
        }

        var value = string.Empty;

        if (char.IsUpper(operand[0]))
        {
            value = operand;
        }
        else
        {
            if (line.Parameters == null)
            {
                throw new NullReferenceException();
            }

            if (line.Parameters.Length > 0)
            {
                if (operand.Length == 1)
                {
                    value = $"0x{line.Parameters[0]:X2}";
                }
                else
                {
                    var word = line.Parameters[0] | (line.Parameters[1] << 8);

                    value = $"0x{word:X4}";
                }

                if (indirect)
                {
                    return $"&Magenta;({value})";
                }

                return $"&Magenta;{value}";
            }

            if (indirect)
            {
                return $"&Yellow;({operand})";
            }

            return $"&Yellow;{operand}";
        }

        return $"&Magenta;{operand}";
    }
}