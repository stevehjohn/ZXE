using System.Diagnostics.CodeAnalysis;
using ZXE.Core.ThirdPartyTests.Infrastructure;

namespace ZXE.Core.ThirdPartyTests
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main()
        {
            var runner = new TestRunner();

            runner.RunAllTests(true);

            Console.ReadKey();
        }
    }
}