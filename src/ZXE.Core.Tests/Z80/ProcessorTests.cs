using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Tests.Z80;

public class ProcessorTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    public ProcessorTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();
    }

    [Fact]
    public void LD_BC_nn()
    {
        _ram[0] = 0x01;
        _ram[1] = 0x39;
        _ram[2] = 0x30;

        _processor.ProcessInstruction(_ram, _state);

        Assert.True(_state.Registers[Register.B] == 0x30);
        Assert.True(_state.Registers[Register.C] == 0x39);
    }
}