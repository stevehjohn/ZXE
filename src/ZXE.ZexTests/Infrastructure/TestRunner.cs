using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.ZexTests.Infrastructure;

public class TestRunner
{
    private bool _complete;

    public void RunZexDoc()
    {
        var motherboard = new Motherboard(Model.Spectrum48K, null);

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

        while (! _complete)
        {
        }
    }

    private void TestsComplete()
    {
        _complete = true;
    }
}