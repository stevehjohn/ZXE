using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Tests.Console.Infrastructure;
using ZXE.Core.Z80;

namespace ZXE.Core.Tests.Console.Tests;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Test(3, "Test bit manipulation operations.")]
public class BitManipulationExercises : ITest
{
    public List<string> Execute()
    {
        var tracer = new FormattingTracer();

        var processor = new Processor(tracer);

        var ram = new Ram(Model.Spectrum48K);

        var state = new State();

        processor.SetState(state);

        ram.Load(new byte[]
                 {
                     0x00,             // NOP
                     0x3E, 0x01,       // LD A, 0x01
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x17,             // RLA
                     0x76              // HALT
                 }, 0);

        while (! state.Halted)
        {
            processor.ProcessInstruction(ram);
        }

        return tracer.GetTrace();
    }
}