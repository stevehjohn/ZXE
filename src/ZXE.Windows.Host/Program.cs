using System;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Windows.Host;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var motherboard = new Motherboard(Model.SpectrumPlus3, null); //, new FileTracer());
        //var motherboard = new Motherboard(Model.SpectrumPlus3, new FormattingTracer());

        motherboard.Reset();

        using var monitor = new Infrastructure.Host(motherboard);

        monitor.Run();
    }
}