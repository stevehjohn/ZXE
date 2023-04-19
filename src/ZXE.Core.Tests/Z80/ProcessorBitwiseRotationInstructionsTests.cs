using Xunit;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorBitwiseRotationInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    private readonly Bus _bus;

    public ProcessorBitwiseRotationInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram();

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void RLCA()
    {
        // RLCA
        _ram[0x4000] = 0x07;

        _state.Registers[Register.A] = 0b01101001;
        
        _processor.State.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b11010010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void RRCA()
    {
        // RRCA
        _ram[0x4000] = 0x0F;

        _state.Registers[Register.A] = 0b11010010;
        
        _processor.State.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01101001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b10110100, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void RLA()
    {
        // RLA
        _ram[0x4000] = 0x17;

        _state.Registers[Register.A] = 0b10000001;
        
        _processor.State.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00000010, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00000101, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00001010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);
    }

    [Fact]
    public void RRA()
    {
        // RRA
        _ram[0x4000] = 0x1F;

        _state.Registers[Register.A] = 0b10000010;
        
        _processor.State.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01000001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00100000, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b10010000, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);
    }

    [Fact]
    public void SLA_addr_RR_plus_d_C()
    {
        // LD IXl, 0x24
        _ram[0x4000] = 0xDD;
        _ram[0x4001] = 0x2E;
        _ram[0x4002] = 0x2A;
        // LD IXh, 0x12
        _ram[0x4003] = 0xDD;
        _ram[0x4004] = 0x26;
        _ram[0x4005] = 0x41;
        // SLA (IX + d), C
        _ram[0x4006] = 0xDD;
        _ram[0x4007] = 0xCB;
        _ram[0x4008] = 0x0A;
        _ram[0x4009] = 0x21;

        _ram[0x4134] = 0b00100010;

        _processor.State.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);

        Assert.False(_state.Flags.Carry);

        _processor.State.ProgramCounter = 0x4000;

        // LD IXl, 0x24
        _ram[0] = 0xDD;
        _ram[1] = 0x2E;
        _ram[2] = 0x2A;
        // LD IXh, 0x12
        _ram[3] = 0xDD;
        _ram[4] = 0x26;
        _ram[5] = 0x12;
        // SLA (IX + d), C
        _ram[6] = 0xDD;
        _ram[7] = 0xCB;
        _ram[8] = 0x0A;
        _ram[9] = 0x21;

        _ram[0x4134] = 0b10100010;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);

        Assert.True(_state.Flags.Carry);
    }
}