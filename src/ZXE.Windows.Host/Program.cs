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
        //var motherboard = new Motherboard(Model.Spectrum48K, new FormattingTracer());

        motherboard.Reset();

        using var monitor = new Monitor(motherboard);

        monitor.Run();
    }
}