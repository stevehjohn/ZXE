using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using ZXE.Common.DebugHelpers;
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

        var ram = new Ram();

        var state = new State();

        var ports = new Ports();

        var bus = new Bus();

        processor.SetState(state);

        ram.Load(new byte[]
                 {
                     0x00,             // NOP
                     0x3E, 0x01,       // LD A, 0x01
                     0x06, 0x08,       // LD B, 8
                     0x17,             // RLA
                     0x10, 0xFD,       // DJNZ -3
                     0x76              // HALT
                 }, 0);

        while (! state.Halted)
        {
            processor.ProcessInstruction(ram, ports, bus);
        }

        return tracer.GetTrace();
    }
}