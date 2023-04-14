using ZXE.Common.ConsoleHelpers;
using ZXE.Common.DebugHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.ZexTests.Infrastructure;

public class TestRunner
{
    private bool _complete;

    public void RunZexTests(string filename)
    {
        var tracer = new FormattingTracer();

        var processor = new Processor(tracer);

        var ram = new Ram(Model.Spectrum48K);

        var data = File.ReadAllBytes($"TestFiles\\{filename}");

        ram.ProtectRom = false;

        ram.Load(data, 0x0100);

        ram[5] = 0xC9;

        processor.State.ProgramCounter = 0x0100;

        processor.State.InterruptFlipFlop1 = false;
        processor.State.InterruptFlipFlop2 = false;

        processor.State.InterruptMode = InterruptMode.Mode0;

        processor.State.StackPointer = 0xFFFF;

        var cpmProcessorExtension = new CpmProcessorExtension(TestsComplete);

        processor.ProcessorExtension = cpmProcessorExtension;

        var ports = new Ports();

        var bus = new Bus();

        var count = 0;

        while (! _complete)
        {
            processor.ProcessInstruction(ram, ports, bus);

            if (processor.State.ProgramCounter == 0)
            {
                break;
            }

            if (Console.KeyAvailable)
            {
                break;
            }

            count++;

            //if (count == 3_168 + 2_000)
            //{
            //    break;
            //}
        }

        Dump(tracer);
    }

    private void TestsComplete()
    {
        _complete = true;
    }

    private static void Dump(FormattingTracer tracer)
    {
        var trace = tracer.GetTrace();

        var start = trace.Count - 2_000;

        if (start < 0)
        {
            start = 0;
        }

        for (var i = start; i < trace.Count - 1; i++)
        {
            FormattedConsole.WriteLine($"    {trace[i]}");
        }
    }
}