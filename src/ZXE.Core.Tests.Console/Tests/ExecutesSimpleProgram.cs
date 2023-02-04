using System.Diagnostics.CodeAnalysis;
using ZXE.Core.Tests.Console.Infrastructure;

namespace ZXE.Core.Tests.Console.Tests;

[ExcludeFromCodeCoverage]
[Test(1, "Successfully executes a simple machine code program and outputs relevant trace information.")]
public class ExecutesSimpleProgram : ITest
{
    public List<string> Execute()
    {
        throw new NotImplementedException();
    }
}