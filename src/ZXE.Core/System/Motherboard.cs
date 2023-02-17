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

    private readonly Bus _bus;

    private readonly Ports _ports;

    private readonly ITimer _timer;

    private readonly ITracer? _tracer;

    private readonly Process? _console;

    public Ram Ram => _ram;

    public Bus Bus => _bus;

    public Ports Ports => _ports;

    public Motherboard(Model model, ITracer? tracer)
    {
        _ram = new Ram(model);

        _ports = new Ports();

        _bus = new Bus();

        _timer = new Timer(4_000_000)
                 {
                     OnTick = Tick
                 };

        if (tracer != null)
        {
            _processor = new Processor(tracer);

            _tracer = tracer;

            var process = new Process();

            process.StartInfo.FileName = "..\\..\\..\\..\\ZXE.Console.LogViewer\\bin\\Debug\\net7.0\\ZXE.Console.LogViewer.exe";

            _console = process;

            process.Start();
        }
        else
        {
            _processor = new Processor();
        }

        byte[] data;

        switch (model)
        {
            case Model.Spectrum48K:
                data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 48K\\image-0.rom");

                _ram.Load(data, 0);

                break;

            case Model.SpectrumPlus3:
                data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum +3\\image-0.rom");

                _ram.Load(data, 0);

                break;
        }
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Reset()
    {
        _processor.Reset();
    }

    private int Tick()
    {
        var cycles = _processor.ProcessInstruction(_ram, _ports, _bus);

        if (_tracer != null)
        {
            var trace = _tracer!.GetTrace();

            foreach (var line in trace)
            {
                // _console!.StandardInput.WriteLine(line);
            }

            _tracer!.ClearTrace();
        }

        return cycles;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}