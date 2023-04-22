using System;
using ZXE.Common.DebugHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;

namespace ZXE.Windows.Host;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var motherboard = new Motherboard(Model.SpectrumPlus2, null); //, new FileTracer());
        //var motherboard = new Motherboard(Model.SpectrumPlus3, new FormattingTracer());

        motherboard.Reset();

        using var monitor = new Monitor(motherboard);

        monitor.Run();
    }
}