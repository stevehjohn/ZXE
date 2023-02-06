using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZXE.Common.ConsoleHelpers;
using ZXE.Core.Tests.Console.Infrastructure;

namespace ZXE.Core.Tests.Console;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main()
    {
        var testClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(TestAttribute)).Any());

        var ordered = testClasses.OrderBy(c => c.GetCustomAttribute<TestAttribute>()!.Sequence);

        FormattedConsole.WriteLine("\n &Yellow;Running tests...\n");

        foreach (var test in ordered)
        {
            var testInfo = test.GetCustomAttribute<TestAttribute>()!;

            FormattedConsole.WriteLine($"   &Cyan;Test: &Blue;{testInfo.Description}\n");
            
            FormattedConsole.WriteLine("   &Cyan;Test output...\n");

            var output = ((ITest) Activator.CreateInstance(test)!).Execute();

            foreach (var line in output)
            {
                FormattedConsole.WriteLine($"     {line}");
            }
        }

        global::System.Console.ReadKey();
    }
}