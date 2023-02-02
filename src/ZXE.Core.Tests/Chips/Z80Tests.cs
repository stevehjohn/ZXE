using Xunit;
using ZXE.Core.Chips;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.Tests.Chips;

public class Z80Tests
{
    private const string PathToRoms = "..\\..\\..\\..\\..\\ROM Images";

    [Fact]
    public void Loads_ROM_into_memory()
    {
        var chip = new Z80(Model.Spectrum48K, $"{PathToRoms}\\ZX Spectrum 48K");
    }
}