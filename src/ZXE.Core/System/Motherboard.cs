using ZXE.Core.Chips;
using ZXE.Core.Infrastructure;
using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Motherboard : IDisposable
{
    private readonly Z80 _cpu;

    private readonly ITimer _timer;

    public Motherboard(Model model, string romPath)
    {
        _cpu = new Z80(model, romPath);

        _timer = new Timer(3_500_000)
                 {
                     OnTick = Tick
                 };
    }

    private void Tick()
    {
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}