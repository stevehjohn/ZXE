using System.Text;

namespace ZXE.Core.Infrastructure;

public static class TraceFormatter
{
    public static string Format(string mnemonic)
    {
        var parts = mnemonic.Split(' ', StringSplitOptions.TrimEntries).Select(p => p.Replace(",", string.Empty)).ToArray();

        var builder = new StringBuilder();

        builder.Append($"&White;{parts[0]}");

        var padding = 20;

        if (parts.Length > 1)
        {
            builder.Append($" &Magenta;{parts[1]}");

            padding -= 7;
        }

        if (parts.Length > 2)
        {
            builder.Append($"&White;, &Green;{parts[2]}");

            padding -= 11;
        }

        return $"{builder}{new string(' ', padding)} X";
    }
}