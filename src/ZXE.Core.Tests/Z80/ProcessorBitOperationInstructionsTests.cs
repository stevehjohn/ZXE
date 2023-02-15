using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorBitOperationInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    private readonly Bus _bus;

    public ProcessorBitOperationInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void BIT_b_R()
    {
        // BIT 0, B
        _ram[0] = 0xCB;
        _ram[1] = 0x40;

        _state.Registers[Register.B] = 0b11111111;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.False(_state.Flags.Zero);

        _state.ProgramCounter = 0;

        _state.Registers[Register.B] = 0b11111110;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.True(_state.Flags.Zero);
    }
}