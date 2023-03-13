using System.Diagnostics;
using ZXE.Common.ConsoleHelpers;
using ZXE.Common.DebugHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.ZexTests.Infrastructure;

public class TestRunner
{
    private bool _complete;

    public void RunZexDoc()
    {
        var tracer = new FormattingTracer();

        var motherboard = new Motherboard(Model.Spectrum48K, tracer);

        motherboard.Reset();

        var data = File.ReadAllBytes("TestFiles\\zexdoc.com");

        motherboard.Ram.Load(data, 0x0100);

        motherboard.Ram[0] = 0xC3;
        motherboard.Ram[1] = 0x00;
        motherboard.Ram[2] = 0x01;
        motherboard.Ram[5] = 0xC9;

        motherboard.Processor.State.ProgramCounter = 0x0100;

        var cpmProcessorExtension = new CpmProcessorExtension(TestsComplete);

        motherboard.Processor.ProcessorExtension = cpmProcessorExtension;

        motherboard.Start();

        var sw = new Stopwatch();

        sw.Start();

        while (! _complete)
        {
            if (sw.Elapsed.Seconds > 1)
            {
                break;
            }
        }

        motherboard.Dispose();

        Dump(tracer);
    }

    private void TestsComplete()
    {
        _complete = true;
    }

    private void Dump(FormattingTracer tracer)
    {
        var trace = tracer.GetTrace();

        foreach (var line in trace)
        {
            FormattedConsole.WriteLine($"    {line}");
        }
    }
}