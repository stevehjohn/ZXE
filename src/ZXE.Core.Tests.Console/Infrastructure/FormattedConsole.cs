using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ZXE.Core.Tests.Console.Infrastructure;

[ExcludeFromCodeCoverage]
public static class FormattedConsole
{
    public static void Write(string output)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < output.Length; i++)
        {
            if (output[i] == '&')
            {
                var end = output.IndexOf(';', i);

                if (end > 0)
                {
                    global::System.Console.Write(builder);

                    builder.Clear();

                    var colour = output[(i + 1)..end];

                    var colourEnum = Enum.Parse<ConsoleColor>(colour);

                    global::System.Console.ForegroundColor = colourEnum;

                    i = end;

                    continue;
                }
            }

            builder.Append(output[i]);
        }

        global::System.Console.Write(builder);
    }

    public static void WriteLine(string output)
    {
        Write($"{output}\n");
    }
}