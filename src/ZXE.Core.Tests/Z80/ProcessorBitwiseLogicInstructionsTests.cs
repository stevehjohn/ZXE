using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorBitwiseLogicInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    private readonly Bus _bus;

    public ProcessorBitwiseLogicInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void AND_R_R()
    {
        // AND A, B
        _ram[0] = 0xA0;
        
        _state.Registers[Register.A] = 0b01111000;
        _state.Registers[Register.B] = 0b00011110;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00011000, _state.Registers[Register.A]);
    }

    [Fact]
    public void AND_R_addr_RR()
    {
        // AND A, (HL)
        _ram[0] = 0xA6;
        _ram[0x1234] = 0b00011110;

        _state.Registers[Register.A] = 0b01111000;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00011000, _state.Registers[Register.A]);
    }

    [Fact]
    public void XOR_R_R()
    {
        // XOR A, B
        _ram[0] = 0xA8;
        
        _state.Registers[Register.A] = 0b01011010;
        _state.Registers[Register.B] = 0b11111111;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
    }

    [Fact]
    public void XOR_R_addr_RR()
    {
        // XOR A, (HL)
        _ram[0] = 0xAE;
        _ram[0x1234] = 0b11111111;

        _state.Registers[Register.A] = 0b01011010;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
    }

    [Fact]
    public void OR_R_R()
    {
        // OR A, B
        _ram[0] = 0xB0;
        
        _state.Registers[Register.A] = 0b00110000;
        _state.Registers[Register.B] = 0b00001100;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00111100, _state.Registers[Register.A]);
    }

    [Fact]
    public void OR_R_addr_RR()
    {
        // OR A, (HL)
        _ram[0] = 0xB6;
        _ram[0x1234] = 0b00001100;

        _state.Registers[Register.A] = 0b00110000;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b00111100, _state.Registers[Register.A]);
    }
}