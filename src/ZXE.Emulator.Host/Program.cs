using System;

namespace ZXE.Emulator.Host
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
