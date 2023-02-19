using System.IO;
using ZXE.Common.DebugHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;

namespace ZXE.Windows.Host;

public static class Program
{
    public static void Main()
    {
        var motherboard = new Motherboard(Model.Spectrum48K, null); //, new FormattingTracer());
        //var motherboard = new Motherboard(Model.Spectrum48K, new FormattingTracer());

        motherboard.Reset();

        var data = File.ReadAllBytes("..\\..\\..\\..\\..\\Game Images\\Treasure Island Dizzy\\image-0.z80");

        motherboard.LoadData(data, 0x5B00);

        motherboard.Reset(0x5B00);

        using var monitor = new Monitor(motherboard);

        monitor.Run();
    }
}