using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

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
    public void Passes_correct_number_of_bytes_to_instruction()
    {
        _ram[0xFFFD] = 0x01;

        var state = new State
                    {
                        ProgramCounter = 0xFFFD
                    };

        _processor.ProcessInstruction(_ram, state);

        _ram[0xFFFE] = 0x01;

        state = new State
                    {
                        ProgramCounter = 0xFFFE
                    };

        Assert.Throws<ArgumentOutOfRangeException>(() => _processor.ProcessInstruction(_ram, state));
    }

    [Fact]
    public void LD_rr_nn()
    {
        // LD BC, 0x1234
        _ram[0] = 0x01;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x12, _state.Registers[Register.B]);
        Assert.Equal(0x34, _state.Registers[Register.C]);
    }

    [Fact]
    public void LD_addr_rr_A()
    {
        // LD (BC), A
        _ram[0] = 0x02;

        _state.Registers[Register.B] = 0x12;
        _state.Registers[Register.C] = 0x34;

        _state.Registers[Register.A] = 0x56;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x56, _ram[0x1234]);
    }

    [Fact]
    public void INC_rr()
    {
        // INC BC
        _ram[0] = 0x03;

        _state.Registers[Register.B] = 0x00;
        _state.Registers[Register.C] = 0xFE;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x00, _state.Registers[Register.B]);
        Assert.Equal(0xFF, _state.Registers[Register.C]);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x01, _state.Registers[Register.B]);
        Assert.Equal(0x00, _state.Registers[Register.C]);
    }

    [Fact]
    public void INC_r()
    {
        // INC B
        _ram[0] = 0x04;

        _state.Registers[Register.B] = 0x12;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x13, _state.Registers[Register.B]);
    }

    [Fact]
    public void DEC_r()
    {
        // DEC B
        _ram[0] = 0x05;

        _state.Registers[Register.B] = 0x12;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x11, _state.Registers[Register.B]);
    }

    [Fact]
    public void LD_R_n()
    {
        // LD B, 0x23
        _ram[0] = 0x06;
        _ram[1] = 0x23;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x23, _state.Registers[Register.B]);
    }

    [Fact]
    public void RLCA()
    {
        // RLCA
        _ram[0] = 0x07;

        _state.Registers[Register.A] = 0b01101001;
        
        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0b11010010, _state.Registers[Register.A]);
        Assert.Equal(_state.Flags & Flags.Carry, 0);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
        Assert.True((_state.Flags & Flags.Carry) > 0);
    }
}