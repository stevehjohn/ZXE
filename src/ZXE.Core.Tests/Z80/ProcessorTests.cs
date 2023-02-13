using Moq;
using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.Infrastructure.Interfaces;
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

    private readonly Ports _ports;

    public ProcessorTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();
    }

    [Fact]
    public void Passes_correct_number_of_bytes_to_instruction()
    {
        _ram[0xFFFD] = 0x01;

        _state.ProgramCounter = 0xFFFD;

        _processor.ProcessInstruction(_ram, _ports);
    }

    [Fact]
    public void Calls_tracer_if_present()
    {
        var tracer = new Mock<ITracer>();

        var processor = new Processor(tracer.Object);

        _ram[0] = 0x00;

        processor.ProcessInstruction(_ram, _ports);

        tracer.Verify(t => t.TraceBefore(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
        tracer.Verify(t => t.TraceAfter(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
    }

    [Fact]
    public void RLCA()
    {
        // RLCA
        _ram[0] = 0x07;

        _state.Registers[Register.A] = 0b01101001;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b11010010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void RRCA()
    {
        // RRCA
        _ram[0] = 0x0F;

        _state.Registers[Register.A] = 0b11010010;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01101001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b10110100, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);
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
    public void RLA()
    {
        // RLA
        _ram[0] = 0x17;

        _state.Registers[Register.A] = 0b10000001;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b00000010, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b00000101, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b00001010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);
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
    public void RRA()
    {
        // RRA
        _ram[0] = 0x1F;

        _state.Registers[Register.A] = 0b10000010;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01000001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b00100000, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b10010000, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);
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

    [Fact]
    public void Can_execute_DD_instruction()
    {
        // LD IX, nn
        _ram[0] = 0xDD;
        _ram[1] = 0x21;
        _ram[2] = 0x34;
        _ram[3] = 0x12;

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x1234, _state.Registers.ReadPair(Register.IX));
    }

    [Fact]
    public void Can_execute_FD_instruction()
    {
        // LD IY, nn
        _ram[0] = 0xFD;
        _ram[1] = 0x21;
        _ram[2] = 0x34;
        _ram[3] = 0x12;

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0x1234, _state.Registers.ReadPair(Register.IY));
    }

    [Fact]
    public void Can_execute_DDCB_instruction()
    {
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

        _ram[0x1234] = 0b00100010;

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);
    }

    [Fact]
    public void SLA_addr_RR_plus_d_C()
    {
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

        _ram[0x1234] = 0b00100010;

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);

        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

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

        _ram[0x1234] = 0b10100010;

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        _processor.ProcessInstruction(_ram, _ports);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);

        Assert.True(_state.Flags.Carry);
    }
}