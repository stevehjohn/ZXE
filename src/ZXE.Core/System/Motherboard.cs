using ZXE.Core.Infrastructure;
using ZXE.Core.System.Interfaces;
using ZXE.Core.Z80;

namespace ZXE.Core.System;

public class Motherboard : IDisposable
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly ITimer _timer;

    public Motherboard(Model model)
    {
        _ram = new Ram(model);

        _timer = new Timer(3_500_000)
                 {
                     OnTick = Tick
                 };

        _processor = new Processor();
    }

    public void Load(byte[] data, int destination)
    {
        _ram.Load(data, destination);
    }

    private void Tick()
    {
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}