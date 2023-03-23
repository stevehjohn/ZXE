using ZXE.ZexTests.Infrastructure;

namespace ZXE.ZexTests
{
    public class Program
    {
        public static void Main()
        {
            var runner = new TestRunner();

            runner.RunZexTests("zexdoc.com");

            Console.ReadKey();
        }
    }
}