using ZXE.Common.DebugHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;

namespace ZXE.Windows.Host;

public static class Program
{
    public static void Main()
    {
        var motherboard = new Motherboard(Model.Spectrum48K, new FormattingTracer());
        //var motherboard = new Motherboard(Model.SpectrumPlus3, null);

        motherboard.Reset();

        //var data = File.ReadAllBytes("..\\..\\..\\..\\..\\Game Images\\Treasure Island Dizzy\\load-screen.bin");

        //motherboard.LoadData(data, 0x4000);

        //motherboard.Reset(0x4000);

        using var monitor = new Monitor(motherboard);

        monitor.Run();
    }
}