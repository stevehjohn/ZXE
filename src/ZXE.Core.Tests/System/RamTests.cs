using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Tests.System;

public class RamTests
{
    [Theory]
    [InlineData(Model.Spectrum48K, 65_536)]
    [InlineData(Model.Spectrum128K, 131_072)]
    [InlineData(Model.SpectrumPlus2, 131_072)]
    [InlineData(Model.SpectrumPlus2A, 131_072)]
    [InlineData(Model.SpectrumPlus3, 131_072)]
    public void Size_is_correct(Model model, int expected)
    {
        var ram = new Ram(model);

        Assert.Equal(expected, ram.Size);
    }

    [Fact]
    public void Loads_data_correctly()
    {
        var ram = new Ram(Model.Spectrum48K);

        ram.Load(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14 }, 0x20);

        Assert.Equal(0x10, ram[0x20]);
        Assert.Equal(0x11, ram[0x21]);
        Assert.Equal(0x12, ram[0x22]);
        Assert.Equal(0x13, ram[0x23]);
        Assert.Equal(0x14, ram[0x24]);
    }
}