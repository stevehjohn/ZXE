// ReSharper disable InconsistentNaming

using System.Diagnostics.CodeAnalysis;

namespace ZXE.Core.ThirdPartyTests.Models;

#pragma warning disable CS8618

[ExcludeFromCodeCoverage]
public class StateDefinition
{
    public int PC { get; set; }

    public int SP { get; set; }

    public byte A { get; set; }

    public byte B { get; set; }

    public byte C { get; set; }

    public byte D { get; set; }

    public byte E { get; set; }

    public byte H { get; set; }

    public byte L { get; set; }

    public byte F { get; set; }

    public ushort IX { get; set; }

    public ushort IY { get; set; }

    public ushort AF_ { get; set; }

    public ushort BC_ { get; set; }

    public ushort DE_ { get; set; }

    public ushort HL_ { get; set; }

    public int[][] Ram { get; set; }
}

#pragma warning restore CS8618
