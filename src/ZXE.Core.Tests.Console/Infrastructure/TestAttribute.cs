using System.Diagnostics.CodeAnalysis;

namespace ZXE.Core.Tests.Console.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestAttribute : Attribute
{
    public int Sequence { get; }

    public string Description { get; }

    public TestAttribute(int sequence, string description)
    {
        Sequence = sequence;

        Description = description;
    }
}