using System.Diagnostics;
using ZXE.Core.Infrastructure;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System.Interfaces;
using ZXE.Core.Z80;

namespace ZXE.Core.System;

public class Motherboard : IDisposable
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly Ports _ports;

    private readonly ITimer _timer;

    private readonly ITracer? _tracer;

    public Ram Ram => _ram;

    public Motherboard(Model model, ITracer? tracer)
    {
        _ram = new Ram(model);

        _ports = new Ports();

        _timer = new Timer(4_000_000)
                 {
                     OnTick = Tick
                 };

        if (tracer != null)
        {
            _processor = new Processor(tracer);

            _tracer = tracer;

            Process.Start("cmd.exe");
        }
        else
        {
            _processor = new Processor();
        }

        switch (model)
        {
            case Model.Spectrum48K:
                var data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 48K\\image-0.rom");

                _ram.Load(data, 0);

                break;
        }

        _timer.Start();
    }

    public void Reset()
    {
        _processor.Reset();
    }

    private void Tick()
    {
        _processor.ProcessInstruction(_ram, _ports);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}