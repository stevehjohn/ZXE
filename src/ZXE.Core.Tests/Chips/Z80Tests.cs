using Xunit;
using ZXE.Core.Chips;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Tests.Chips;

public class Z80Tests
{
    private const string PathToRoms = "..\\..\\..\\..\\..\\ROM Images";

    [Fact]
    public void Loads_ROM_into_memory()
    {
        var ram = new Ram(Model.Spectrum48K);

        var chip = new Z80(ram);

        chip.LoadRoms(
    }
}