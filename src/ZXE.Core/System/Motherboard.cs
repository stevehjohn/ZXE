using ZXE.Core.Chips;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Motherboard
{
    private readonly Z80 _cpu;

    public Motherboard(Model model, string romPath)
    {
        _cpu = new Z80(model, romPath);
    }
}