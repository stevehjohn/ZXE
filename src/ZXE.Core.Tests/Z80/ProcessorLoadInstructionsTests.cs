﻿using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorLoadInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    private readonly Bus _bus;

    public ProcessorLoadInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void LD_RR_nn()
    {
        // LD BC, 0x1234
        _ram[0] = 0x01;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x12, _state.Registers[Register.B]);
        Assert.Equal(0x34, _state.Registers[Register.C]);
    }

    [Fact]
    public void LD_addr_RR_A()
    {
        // LD (BC), A
        _ram[0] = 0x02;

        _state.Registers[Register.B] = 0x12;
        _state.Registers[Register.C] = 0x34;

        _state.Registers[Register.A] = 0x56;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x56, _ram[0x1234]);
    }

    [Fact]
    public void LD_R_n()
    {
        // LD B, 0x23
        _ram[0] = 0x06;
        _ram[1] = 0x23;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x23, _state.Registers[Register.B]);
    }

    [Fact]
    public void LD_addr_nn_R()
    {
        // LD (0x1234), A
        _ram[0] = 0x32;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _state.Registers[Register.A] = 0x56;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x56, _ram[0x1234]);
    }

    [Fact]
    public void LD_R_addr_nn()
    {
        // LD A, (0x1234)
        _ram[0] = 0x3A;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _ram[0x1234] = 0x56;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x56, _state.Registers[Register.A]);
    }

    [Fact]
    public void LD_R_addr_RR()
    {
        // LD A, (BC)
        _ram[0] = 0x0A;
        _ram[0x1234] = 0x56;

        _state.Registers.WritePair(Register.BC, 0x1234);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x56, _state.Registers[Register.A]);
    }

    [Fact]
    public void LD_addr_nn_RR()
    {
        // LD (0x1234), HL
        _ram[0] = 0x22;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _state.Registers.WritePair(Register.HL, 0x5678);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x78, _ram[0x1234]);
        Assert.Equal(0x56, _ram[0x1235]);
    }

    [Fact]
    public void LD_RR_addr_nn()
    {
        // LD HL, (0x1234)
        _ram[0] = 0x2A;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _ram[0x1234] = 0x56;
        _ram[0x1235] = 0x78;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x56, _state.Registers[Register.L]);
        Assert.Equal(0x78, _state.Registers[Register.H]);
    }

    [Fact]
    public void LD_SP_nn()
    {
        // LD SP, 0x1234
        _ram[0] = 0x31;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1234, _state.StackPointer);
    }

    [Fact]
    public void LD_addr_RR_n()
    {
        // LD (HL), 0xAB
        _ram[0] = 0x36;
        _ram[1] = 0xAB;

        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0xAB, _ram[0x1234]);
    }

    [Fact]
    public void LD_R_R()
    {
        // LD B, C
        _ram[0] = 0x41;

        _state.Registers[Register.C] = 0x12;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x12, _state.Registers[Register.B]);
    }
}