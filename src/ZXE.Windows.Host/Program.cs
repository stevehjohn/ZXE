using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;

namespace ZXE.Windows.Host;

public static class Program
{
    public static void Main()
    {
        var motherboard = new Motherboard(Model.Spectrum48K, null); //, new FormattingTracer());
        //var motherboard = new Motherboard(Model.SpectrumPlus3, new FormattingTracer());

        motherboard.Reset();

        using var monitor = new Monitor(motherboard);

        monitor.Run();
    }
}