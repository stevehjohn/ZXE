using ZXE.Core.Infrastructure;
using ZXE.Core.System.Interfaces;
using ZXE.Core.Z80;

namespace ZXE.Core.System;

public class Motherboard : IDisposable
{
    private readonly Ram _ram;

    private readonly ITimer _timer;

    private readonly Cpu _cpu;

    public Motherboard(Model model)
    {
        _ram = new Ram(model);

        _timer = new Timer(3_500_000)
                 {
                     OnTick = Tick
                 };

        _cpu = new Cpu();
    }

    private void Tick()
    {
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}