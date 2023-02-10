using ZXE.Utilities.Utilities;

namespace ZXE.Utilities
{
    public static class Entrypoint
    {
        //public static void Main(string[] args)
        public static void Main()
        {
            var disassembler = new Disassembler();

            disassembler.LoadData(File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 48K\\image-0.rom"));
        }
    }
}