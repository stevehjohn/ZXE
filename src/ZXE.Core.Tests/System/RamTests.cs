using Xunit;
using ZXE.Core.System;

namespace ZXE.Core.Tests.System;

public class RamTests
{
    [Fact]
    public void Loads_data_correctly()
    {
        var ram = new Ram();

        ram.Load(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14 }, 0x20);

        Assert.Equal(0x10, ram[0x20]);
        Assert.Equal(0x11, ram[0x21]);
        Assert.Equal(0x12, ram[0x22]);
        Assert.Equal(0x13, ram[0x23]);
        Assert.Equal(0x14, ram[0x24]);
    }
}