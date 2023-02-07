using System.Diagnostics.CodeAnalysis;
using ZXE.Core.ThirdPartyTests.Infrastructure;

namespace ZXE.Core.ThirdPartyTests
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main()
        {
            TestRunner.RunAllTests(true);

            Console.ReadKey();
        }
    }
}