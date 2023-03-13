using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.ZexTests.Infrastructure;

public class TestRunner
{
    public static void RunZexDoc()
    {
        var motherboard = new Motherboard(Model.Spectrum48K, null);

        var data = File.ReadAllBytes("TestFiles\\zexdoc.com");

        motherboard.Ram.Load(data, 0x0100);
    }
}