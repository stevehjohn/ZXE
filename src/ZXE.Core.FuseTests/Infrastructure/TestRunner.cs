using ZXE.Common.ConsoleHelpers;
using ZXE.Core.FuseTests.Models;

namespace ZXE.Core.FuseTests.Infrastructure;

public static class TestRunner
{
    public static void RunTests()
    {
        var input = File.ReadAllLines("TestDefinitions\\input.fuse");

        var test = new List<string>();

        for (var i = 0; i < input.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(input[i]))
            {
                if (test.Count > 0)
                {
                    RunTest(new TestInput(test.ToArray()));

                    test.Clear();
                }

                continue;
            }

            test.Add(input[i]);
        }
    }

    private static void RunTest(TestInput input)
    {
        FormattedConsole.WriteLine($"\n  &Cyan;Test&White;: &Yellow;{input.Name}");
    }
}