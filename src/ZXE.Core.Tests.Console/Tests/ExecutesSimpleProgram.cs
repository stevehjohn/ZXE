using System.Diagnostics.CodeAnalysis;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Tests.Console.Infrastructure;
using ZXE.Core.Z80;

namespace ZXE.Core.Tests.Console.Tests;

[ExcludeFromCodeCoverage]
[Test(1, "Successfully executes a simple machine code program and outputs relevant trace information.")]
public class ExecutesSimpleProgram : ITest
{
    public List<string> Execute()
    {
        var output = new List<string>();

        var processor = new Processor();

        var ram = new Ram(Model.Spectrum48K);

        var state = new State();

        processor.SetState(state);

        ram.Load(new byte[]
                 {
                     0x00,             // NOP
                     0x01, 0x34, 0x12, // LD BC, 0x1234
                     0x02,             // LD (BC), A
                     0x03,             // INC BC
                     0x76              // HALT
                 }, 0);

        while (! state.Halted)
        {
            output.Add(processor.ProcessInstruction(ram, true));
        }

        return output;
    }
}