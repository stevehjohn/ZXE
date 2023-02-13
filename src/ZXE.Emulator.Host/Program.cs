using System;
using ZXE.Emulator.Host.Display;

namespace ZXE.Emulator.Host
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new Monitor())
            {
                game.Run();
            }
        }
    }
}
