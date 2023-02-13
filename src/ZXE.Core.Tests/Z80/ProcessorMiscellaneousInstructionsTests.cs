using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorMiscellaneousInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;
    
    public ProcessorMiscellaneousInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();
    }

    [Fact]
    public void NOP()
    {
        // NOP
        _ram[0] = 0x00;

        _processor.ProcessInstruction(_ram, _ports);
    }

    [Fact]
    public void EX_RR_RaRa()
    {
        // EX AF, AF'
        _ram[0] = 0x08;

        _state.Registers[Register.A] = 0x01;
        _state.Registers[Register.F] = 0x02;
        _state.Registers[Register.A1] = 0x03;
        _state.Registers[Register.F1] = 0x04;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x03, _state.Registers[Register.A]);
        Assert.Equal(0x04, _state.Registers[Register.F]);
        Assert.Equal(0x01, _state.Registers[Register.A1]);
        Assert.Equal(0x02, _state.Registers[Register.F1]);
    }

    [Fact]
    public void CPL()
    {
        // CPL
        _ram[0] = 0x2F;

        _state.Registers[Register.A] = 0b10100101;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01011010, _state.Registers[Register.A]);
    }

    [Fact]
    public void SCF()
    {
        // SCF
        _ram[0] = 0x37;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.True(_state.Flags.Carry);
    }
}