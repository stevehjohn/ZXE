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

        ram[0] = 0xC3;
        ram[1] = 0x00;
        ram[2] = 0x01;
        ram[5] = 0xC9;

        processor.State.ProgramCounter = 0x0100;

        processor.State.InterruptFlipFlop1 = false;
        processor.State.InterruptFlipFlop2 = false;

        processor.State.StackPointer = 0xF000;

        var cpmProcessorExtension = new CpmProcessorExtension(TestsComplete);

        processor.ProcessorExtension = cpmProcessorExtension;

        var ports = new Ports();

        var bus = new Bus();

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

        for (var i = trace.Count - 1; i > trace.Count - 2_000; i--)
        {
            FormattedConsole.WriteLine($"    {trace[i]}");
        }
    }
}