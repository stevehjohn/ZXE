using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorBranchInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    public ProcessorBranchInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();
    }

    [Fact]
    public void DJNZ_e()
    {
        // DJNZ 0x20
        _ram[0] = 0x10;
        _ram[1] = 0x20;

        _state.Registers[Register.B] = 0x10;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x0F, _state.Registers[Register.B]);
        Assert.Equal(0x22, _state.ProgramCounter);

        _state.ProgramCounter = 0;
        _state.Registers[Register.B] = 0x01;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x00, _state.Registers[Register.B]);
        Assert.Equal(0x02, _state.ProgramCounter);
    }

    [Fact]
    public void JR_e()
    {
        // JR 0x20
        _ram[0] = 0x18;
        _ram[1] = 0x20;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x22, _state.ProgramCounter);
    }

    [Fact]
    public void JR_NZ_e()
    {
        // JR NZ, 0x10
        _ram[0] = 0x20;
        _ram[1] = 0x10;

        _state.Flags.Zero = true;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Zero = false;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void JR_Z_e()
    {
        // JR Z, 0x10
        _ram[0] = 0x28;
        _ram[1] = 0x10;

        _state.Flags.Zero = false;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Zero = true;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void JR_NC_e()
    {
        // JR NC, 0x10
        _ram[0] = 0x30;
        _ram[1] = 0x10;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Carry = false;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void JR_C_e()
    {
        // JR NC, 0x10
        _ram[0] = 0x38;
        _ram[1] = 0x10;

        _state.Flags.Carry = false;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Carry = true;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x12, _state.ProgramCounter);
    }
}