using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Tests.Console.Infrastructure;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo

namespace ZXE.Core.Tests.Console.Tests;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Test(1, "Successfully executes a simple machine code program that has a loop.")]
public class ExecutesSimpleProgramWithLoop : ITest
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
                     0x06, 0x05,       // LD B, 5
                     0x10, 0xFE,       // DJNZ -2
                     0x76              // HALT
                 }, 0);

        while (! state.Halted)
        {
            processor.ProcessInstruction(ram);

            //var trace = tracer.GetTrace();

            //global::System.Console.Clear();

            //foreach (var line in trace)
            //{
            //    FormattedConsole.WriteLine(line);

            //    global::System.Console.ReadKey();
            //}
        }

        return tracer.GetTrace();
    }
}