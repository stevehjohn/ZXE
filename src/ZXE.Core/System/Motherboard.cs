//#define TRACE_OVER_IP

#if TRACE_OVER_IP
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif
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

    private readonly Model _model;

    private byte _last7FFD;

    private byte _last1FFD;

    private readonly Dictionary<int, byte[]> _romCache = new();

#if TRACE_OVER_IP
    private readonly Process? _console;
#endif

#if TRACE_OVER_IP
    private bool _tracing = true;
#endif

    public Ram Ram => _ram;

    public Ports Ports => _ports;

    public Processor Processor => _processor;

    public Model Model => _model;

    public bool Fast
    {
        get => _timer.Fast;
        set => _timer.Fast = value;
    }

    public Motherboard(Model model, ITracer? tracer)
    {
        _ram = new Ram
               {
                   ProtectRom = true
               };

        _ports = new Ports(model)
                 {
                     PagedEvent = PagedEvent
                 };

        _bus = new Bus();

        _timer = new Timer(4_000_000)
                 {
                     OnTick = Tick,
                     HandleRefreshInterrupt = RefreshInterrupt
                 };

        if (tracer != null)
        {
            _processor = new Processor(tracer);

            _tracer = tracer;

#if TRACE_OVER_IP
            var process = new Process();

            process.StartInfo.FileName = "..\\..\\..\\..\\ZXE.Console.LogViewer\\bin\\Debug\\net7.0\\ZXE.Console.LogViewer.exe";

            _console = process;

            process.Start();
#endif
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

                _ram.LoadRom(data, 0);

                break;

            case Model.Spectrum128:
                data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum 128\\image-0.rom");

                _ram.LoadRom(data, 0);

                break;

            case Model.SpectrumPlus2:
                data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum +2\\image-0.rom");

                _ram.LoadRom(data, 0);

                break;

            case Model.SpectrumPlus3:
                data = File.ReadAllBytes("..\\..\\..\\..\\..\\ROM Images\\ZX Spectrum +3\\image-0.rom");

                _ram.LoadRom(data, 0);

                break;
        }

        _model = model;
    }

    private void PagedEvent(byte port, byte data)
    {
        if (port == 0x1F && (data & 0x01) > 0)
        {
            // Special paging.
            switch (data & 0b0110 >> 1)
            {
                case 0:
                    _ram.SetBank(0xC000, 3);
                    _ram.SetBank(0x8000, 2);
                    _ram.SetBank(0x4000, 1);
                    _ram.SetBank(0x0000, 0);

                    break;

                case 1:
                    _ram.SetBank(0xC000, 7);
                    _ram.SetBank(0x8000, 6);
                    _ram.SetBank(0x4000, 5);
                    _ram.SetBank(0x0000, 4);

                    break;

                case 2:
                    _ram.SetBank(0xC000, 3);
                    _ram.SetBank(0x8000, 6);
                    _ram.SetBank(0x4000, 5);
                    _ram.SetBank(0x0000, 4);
                    
                    break;

                case 3:
                    _ram.SetBank(0xC000, 3);
                    _ram.SetBank(0x8000, 6);
                    _ram.SetBank(0x4000, 7);
                    _ram.SetBank(0x0000, 4);

                    break;
            }

            return;
        }

        if (port == 0x7F)
        {
            _ram.SetBank(0xC000, data & 0b0000_0111);

            _ram.Screen = (data & 0b0000_1000) > 0 ? 2 : 1;
        }

        var folder = _model switch 
        {
            Model.Spectrum128 => "ZX Spectrum 128",
            Model.SpectrumPlus2 => "ZX Spectrum +2",
            Model.SpectrumPlus3 => "ZX Spectrum +3",
            // TODO: Proper exception?
            _ => throw new Exception("Invalid model")
        };

        var romNumber = 0;

        if (port == 0x7F)
        {
            _last7FFD = data;

            romNumber = (data & 0b0001_0000) > 0 ? 1 : 0;
            romNumber += (_last1FFD & 0b0000_0100) > 0 ? 2 : 0;
        }

        if (port == 0x1F)
        {
            _last1FFD = data;

            romNumber = (_last7FFD & 0b0001_0000) > 0 ? 1 : 0;
            romNumber += (data & 0b0000_0100) > 0 ? 2 : 0;
        }

        if (! _romCache.ContainsKey(romNumber))
        {
            _romCache.Add(romNumber, File.ReadAllBytes($"..\\..\\..\\..\\..\\ROM Images\\{folder}\\image-{romNumber}.rom"));
        }

        _ram.LoadRom(_romCache[romNumber], romNumber);
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Pause()
    {
        _timer.Pause();

        Thread.Sleep(100);
    }

    public void Resume()
    {
        _timer.Resume();
    }

    public void Reset(int programCounter = 0x0000)
    {
        _processor.Reset(programCounter);
    }

    public void SetTraceState(bool enabled)
    {
#if TRACE_OVER_IP
        _tracing = enabled;
#endif
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    private int Tick()
    {
        var result = _processor.ProcessInstruction(_ram, _ports, _bus);

#if TRACE_OVER_IP
        if (_tracer != null)
        {
            if (_tracing)
            {
                using var client = new TcpClient();

                client.Connect(IPAddress.Loopback, 1234);

                using var stream = client.GetStream();

                var trace = _tracer!.GetTrace();

                foreach (var line in trace)
                {
                    stream.Write(Encoding.ASCII.GetBytes($"{line}\n"));
                }

                stream.Flush();
            }

            _tracer!.ClearTrace();
        }
#endif

        return result.Cycles;
    }

    private void RefreshInterrupt()
    {
        _bus.Data = 0xFF;

        _bus.Interrupt = true;
    }
}