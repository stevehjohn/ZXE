using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Tests.Z80;

public class ProcessorArithmeticInstructionsTests
{
    private readonly Processor _processor;

    private readonly Ram _ram;

    private readonly State _state;

    private readonly Ports _ports;

    private readonly Bus _bus;

    public ProcessorArithmeticInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void INC_RR()
    {
        // INC BC
        _ram[0x4000] = 0x03;

        _state.Registers[Register.B] = 0x00;
        _state.Registers[Register.C] = 0xFE;

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x00, _state.Registers[Register.B]);
        Assert.Equal(0xFF, _state.Registers[Register.C]);

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x01, _state.Registers[Register.B]);
        Assert.Equal(0x00, _state.Registers[Register.C]);
    }

    [Fact]
    public void INC_R()
    {
        // INC B
        _ram[0x4000] = 0x04;

        _state.Registers[Register.B] = 0x12;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x13, _state.Registers[Register.B]);
    }

    [Fact]
    public void DEC_R()
    {
        // DEC B
        _ram[0x4000] = 0x05;

        _state.Registers[Register.B] = 0x12;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x11, _state.Registers[Register.B]);
    }

    [Fact]
    public void ADD_RR_RR()
    {
        // ADD HL, BC
        _ram[0x4000] = 0x09;

        _state.Registers.WritePair(Register.HL, 0x0034);
        _state.Registers.WritePair(Register.BC, 0x1200);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1234, _state.Registers.ReadPair(Register.HL));
    }

    [Fact]
    public void DEC_RR()
    {
        // DEC BC
        _ram[0x4000] = 0x0B;

        _state.Registers.WritePair(Register.BC, 0x1234);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1233, _state.Registers.ReadPair(Register.BC));
    }

    [Fact]
    public void INC_SP()
    {
        // INC SP
        _ram[0x4000] = 0x33;

        _state.StackPointer = 0x1234;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1235, _state.StackPointer);
    }

    [Fact]
    public void INC_addr_RR()
    {
        // INC (HL)
        _ram[0x4000] = 0x34;
        _ram[0x4123] = 0x09;

        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x0A, _ram[0x4123]);
    }

    [Fact]
    public void DEC_addr_RR()
    {
        // DEC (HL)
        _ram[0x4000] = 0x35;
        _ram[0x4123] = 0x0A;

        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x09, _ram[0x4123]);
    }

    [Fact]
    public void ADD_RR_SP()
    {
        // ADD HL, SP
        _ram[0x4000] = 0x39;

        _state.Registers.WritePair(Register.HL, 0x0101);
        _state.StackPointer = 0xA0A0;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0xA1A1, _state.Registers.ReadPair(Register.HL));
    }

    [Fact]
    public void DEC_SP()
    {
        // DEC SP
        _ram[0x4000] = 0x3B;

        _state.StackPointer = 0x1235;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1234, _state.StackPointer);
    }

    [Fact]
    public void ADD_R_R()
    {
        // ADD A, B
        _ram[0x4000] = 0x80;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1A, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADD_R_addr_RR()
    {
        // ADD A, (HL)
        _ram[0x4000] = 0x86;
        _ram[0x4123] = 0x0A;

        _state.Registers[Register.A] = 0x10;
        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1A, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADC_R_R()
    {
        // ADC A, B
        _ram[0x4000] = 0x88;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1A, _state.Registers[Register.A]);

        _state.ProgramCounter = 0x4000;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1B, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADC_R_addr_RR()
    {
        // ADC A, (HL)
        _ram[0x4000] = 0x8E;
        _ram[0x4123] = 0x0A;

        _state.Registers[Register.A] = 0x10;
        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1A, _state.Registers[Register.A]);

        _state.ProgramCounter = 0x4000;

        _state.Registers[Register.A] = 0x10;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1B, _state.Registers[Register.A]);
    }

    [Fact]
    public void SUB_R_R()
    {
        // SUB A, B
        _ram[0x4000] = 0x90;

        _state.Registers[Register.A] = 0x0A;
        _state.Registers[Register.B] = 0x05;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x05, _state.Registers[Register.A]);
    }

    [Fact]
    public void SUB_R_addr_RR()
    {
        // SUB A, (HL)
        _ram[0x4000] = 0x96;
        _ram[0x4123] = 0x10;

        _state.Registers[Register.A] = 0x20;
        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x10, _state.Registers[Register.A]);
    }

    [Fact]
    public void SBC_R_R()
    {
        // SBC A, B
        _ram[0x4000] = 0x98;

        _state.Registers[Register.A] = 0x20;
        _state.Registers[Register.B] = 0x10;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x10, _state.Registers[Register.A]);

        _state.ProgramCounter = 0x4000;

        _state.Registers[Register.A] = 0x20;
        _state.Registers[Register.B] = 0x10;
        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x0F, _state.Registers[Register.A]);
    }

    [Fact]
    public void SBC_R_addr_RR()
    {
        // SBC A, (HL)
        _ram[0x4000] = 0x9E;
        _ram[0x4123] = 0x10;

        _state.Registers[Register.A] = 0x20;
        _state.Registers.WritePair(Register.HL, 0x4123);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x10, _state.Registers[Register.A]);
    }
}