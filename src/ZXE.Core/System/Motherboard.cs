using System.Diagnostics;
using ZXE.Core.Infrastructure;
using ZXE.Core.System.Interfaces;
using ZXE.Core.Z80;

namespace ZXE.Core.System;

public class Motherboard : IDisposable
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly Ports _ports;

    private readonly ITimer _timer;

    public Motherboard(Model model)
    {
        _ram = new Ram(model);

        _ports = new Ports();

        _timer = new Timer(4_000_000)
                 {
                     OnTick = Tick
                 };

        _processor = new Processor();

        switch (model)
        {
            case Model.Spectrum48K:
                var data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 48K\\image-0.rom");

                _ram.Load(data, 0);

                break;
        }

        _timer.Start();
    }

    private void Tick()
    {
        _processor.ProcessInstruction(_ram, _ports);

        Debugger.Log(0, "INFO", "Tick...\n");
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}