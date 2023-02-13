using ZXE.Windows.Host.Display;

namespace ZXE.Windows.Host;

public static class Program
{
    public static void Main()
    {
        using var monitor = new Monitor();

        monitor.Run();
    }
}