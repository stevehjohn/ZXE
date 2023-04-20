using Xunit;
using ZXE.Core.System;

namespace ZXE.Core.Tests.System;

public class RamTests
{
    private readonly Ram _ram = new();

    [Fact]
    public void Copies_data_across_layout_boundaries()
    {
        _ram[0x0FFE] = 0x01;
        _ram[0x0FFF] = 0x02;
        _ram[0x1000] = 0x03;
        _ram[0x1001] = 0x04;

        var read = _ram.ReadBlock(0x0FFE..0x1002);

        Assert.Equal(4, read.Length);

        Assert.Equal(0x01, read[0]);
        Assert.Equal(0x02, read[1]);
        Assert.Equal(0x03, read[2]);
        Assert.Equal(0x04, read[3]);
    }
}