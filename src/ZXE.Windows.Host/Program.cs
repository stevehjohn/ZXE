using System;

namespace ZXE.Windows.Host;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        using var monitor = new Infrastructure.Host();

        monitor.Run();
    }
}