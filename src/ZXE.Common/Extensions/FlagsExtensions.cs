using System.Text;
using ZXE.Core.Z80;

namespace ZXE.Common.Extensions;

public static class FlagsExtensions
{
    public static string GetFlags(this Flags flags)
    {
        var builder = new StringBuilder();

        builder.Append(flags.Carry ? 'C' : ' ');
        builder.Append(flags.AddSubtract ? 'N' : ' ');
        builder.Append(flags.ParityOverflow ? 'P' : ' ');
        builder.Append(flags.X1 ? '3' : ' ');
        builder.Append(flags.HalfCarry ? 'H' : ' ');
        builder.Append(flags.X2 ? '5' : ' ');
        builder.Append(flags.Zero ? 'Z' : ' ');
        builder.Append(flags.Sign ? 'S' : ' ');

        return builder.ToString();
    }
}