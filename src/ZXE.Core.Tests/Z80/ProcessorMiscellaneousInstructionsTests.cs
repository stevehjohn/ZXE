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

    private readonly Bus _bus;

    public ProcessorMiscellaneousInstructionsTests()
    {
        _processor = new Processor();

        _ram = new Ram();

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void NOP()
    {
        // NOP
        _ram[0x4000] = 0x00;

        _processor.ProcessInstruction(_ram, _ports, _bus);
    }

    [Fact]
    public void EX_RR_RaRa()
    {
        // EX AF, AF'
        _ram[0x4000] = 0x08;

        _state.Registers[Register.A] = 0x01;
        _state.Registers[Register.F] = 0x02;
        _state.Registers[Register.A_] = 0x03;
        _state.Registers[Register.F_] = 0x04;

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x03, _state.Registers[Register.A]);
        Assert.Equal(0x04, _state.Registers[Register.F]);
        Assert.Equal(0x01, _state.Registers[Register.A_]);
        Assert.Equal(0x02, _state.Registers[Register.F_]);
    }

    [Fact]
    public void CPL()
    {
        // CPL
        _ram[0x4000] = 0x2F;

        _state.Registers[Register.A] = 0b10100101;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01011010, _state.Registers[Register.A]);
    }

    [Fact]
    public void SCF()
    {
        // SCF
        _ram[0x4000] = 0x37;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void CCF()
    {
        // CCF
        _ram[0x4000] = 0x3F;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void HALT()
    {
        // HALT
        _ram[0x4000] = 0x76;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.True(_state.Halted);
    }

    [Fact]
    public void CP_R_R()
    {
        // CP A, B
        _ram[0x4000] = 0xB8;
        
        _state.Registers[Register.A] = 0b00110000;
        _state.Registers[Register.B] = 0b00001100;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.False(_state.Flags.Carry);
        Assert.True(_state.Flags.AddSubtract);
        Assert.False(_state.Flags.ParityOverflow);
        Assert.True(_state.Flags.X1);
        Assert.True(_state.Flags.HalfCarry);
        Assert.False(_state.Flags.X2);
        Assert.False(_state.Flags.Zero);
        Assert.False(_state.Flags.Sign);
    }

    [Fact]
    public void CP_R_addr_RR()
    {
        // CP A, (HL)
        _ram[0x4000] = 0xBE;
        _ram[0x4234] = 0b00001100;

        _state.Registers[Register.A] = 0b00110000;
        _state.Registers.WritePair(Register.HL, 0x4234);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.False(_state.Flags.Carry);
        Assert.True(_state.Flags.AddSubtract);
        Assert.False(_state.Flags.ParityOverflow);
        Assert.True(_state.Flags.X1);
        Assert.True(_state.Flags.HalfCarry);
        Assert.False(_state.Flags.X2);
        Assert.False(_state.Flags.Zero);
        Assert.False(_state.Flags.Sign);
    }

    [Fact]
    public void IN_b_R_addr_n()
    {
        // IN_0 B, (n)
        _ram[0x4000] = 0xED;
        _ram[0x4001] = 0x00;
        _ram[0x4002] = 0x10;

        _ports.WriteByte(0x10, 0x24);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);
        
        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x24, _state.Registers[Register.B]);
    }

    [Fact]
    public void LDIR()
    {
        // LDIR
        _ram[0x4000] = 0xED;
        _ram[0x4001] = 0xB0;

        _ram[0x4234] = 0xAB;

        _state.Registers.WritePair(Register.HL, 0x4234);

        _state.Registers.WritePair(Register.DE, 0x5678);

        _state.Registers.WritePair(Register.BC, 0x0001);
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);
        
        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0xAB, _ram[0x5678]);

        Assert.Equal(0x0000, _state.Registers.ReadPair(Register.BC));

        _ram[0x4234] = 0xAB;

        _state.ProgramCounter = 0x4000;

        _state.Registers.WritePair(Register.HL, 0x4234);

        _state.Registers.WritePair(Register.DE, 0x5678);

        _state.Registers.WritePair(Register.BC, 0x0002);

        _processor.ProcessInstruction(_ram, _ports, _bus);
        
        _processor.ProcessInstruction(_ram, _ports, _bus);
        
        _processor.ProcessInstruction(_ram, _ports, _bus);
        
        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0xAB, _ram[0x5678]);

        Assert.Equal(0x0000, _state.Registers.ReadPair(Register.BC));
    }
}