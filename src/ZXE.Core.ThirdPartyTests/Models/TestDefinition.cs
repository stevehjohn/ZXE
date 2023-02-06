using System.Diagnostics.CodeAnalysis;

namespace ZXE.Core.ThirdPartyTests.Models;

#pragma warning disable CS8618

[ExcludeFromCodeCoverage]
public class TestDefinition
{
    public string Name { get; set; }

    public StateDefinition Initial { get; set; }

    public StateDefinition Final { get; set; }

    public object[][] Cycles { get; set; }
}

#pragma warning restore CS8618
