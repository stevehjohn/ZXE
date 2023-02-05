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

    public ProcessorTests()
    {
        _processor = new Processor();

        _ram = new Ram(Model.Spectrum48K);

        _state = new State();

        _processor.SetState(_state);
    }

    [Fact]
    public void Passes_correct_number_of_bytes_to_instruction()
    {
        _ram[0xFFFD] = 0x01;

        _state.ProgramCounter = 0xFFFD;

        _processor.ProcessInstruction(_ram);

        _ram[0xFFFE] = 0x01;

        _state.ProgramCounter = 0xFFFE;

        Assert.Throws<ArgumentOutOfRangeException>(() => _processor.ProcessInstruction(_ram));
    }

    [Fact]
    public void Calls_tracer_if_present()
    {
        var tracer = new Mock<ITracer>();

        var processor = new Processor(tracer.Object);

        _ram[0] = 0x00;

        processor.ProcessInstruction(_ram);

        tracer.Verify(t => t.TraceBefore("NOP", new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
        tracer.Verify(t => t.TraceAfter("NOP", new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
    }

    [Fact]
    public void NOP()
    {
        // NOP
        _ram[0] = 0x00;

        _processor.ProcessInstruction(_ram);
    }

    [Fact]
    public void LD_RR_nn()
    {
        // LD BC, 0x1234
        _ram[0] = 0x01;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _processor.ProcessInstruction(_ram);

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

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x56, _ram[0x1234]);
    }

    [Fact]
    public void INC_RR()
    {
        // INC BC
        _ram[0] = 0x03;

        _state.Registers[Register.B] = 0x00;
        _state.Registers[Register.C] = 0xFE;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x00, _state.Registers[Register.B]);
        Assert.Equal(0xFF, _state.Registers[Register.C]);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x01, _state.Registers[Register.B]);
        Assert.Equal(0x00, _state.Registers[Register.C]);
    }

    [Fact]
    public void INC_R()
    {
        // INC B
        _ram[0] = 0x04;

        _state.Registers[Register.B] = 0x12;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x13, _state.Registers[Register.B]);
    }

    [Fact]
    public void DEC_R()
    {
        // DEC B
        _ram[0] = 0x05;

        _state.Registers[Register.B] = 0x12;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x11, _state.Registers[Register.B]);
    }

    [Fact]
    public void LD_R_n()
    {
        // LD B, 0x23
        _ram[0] = 0x06;
        _ram[1] = 0x23;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x23, _state.Registers[Register.B]);
    }

    [Fact]
    public void RLCA()
    {
        // RLCA
        _ram[0] = 0x07;

        _state.Registers[Register.A] = 0b01101001;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b11010010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b10100101, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void LD_addr_nn_R()
    {
        // LD (nn), A
        _ram[0] = 0x32;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _state.Registers[Register.A] = 0x56;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x56, _ram[0x1234]);
    }

    [Fact]
    public void LD_R_addr_nn()
    {
        // LD A, (nn)
        _ram[0] = 0x3A;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _ram[0x1234] = 0x56;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x56, _state.Registers[Register.A]);
    }

    [Fact]
    public void EX_RR_RaRa()
    {
        // EX AF, AF'
        _ram[0] = 0x08;

        _state.Registers[Register.A] = 0x01;
        _state.Registers[Register.F] = 0x02;
        _state.Registers[Register.Aa] = 0x03;
        _state.Registers[Register.Fa] = 0x04;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x03, _state.Registers[Register.A]);
        Assert.Equal(0x04, _state.Registers[Register.F]);
        Assert.Equal(0x01, _state.Registers[Register.Aa]);
        Assert.Equal(0x02, _state.Registers[Register.Fa]);
    }

    [Fact]
    public void ADD_RR_RR()
    {
        // ADD HL, BC
        _ram[0] = 0x09;

        _state.Registers.WritePair(Register.HL, 0x0034);
        _state.Registers.WritePair(Register.BC, 0x1200);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1234, _state.Registers.ReadPair(Register.HL));
    }

    [Fact]
    public void LD_R_addr_RR()
    {
        // LD A, (BC)
        _ram[0] = 0x0A;
        _ram[0x1234] = 0x56;

        _state.Registers.WritePair(Register.BC, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x56, _state.Registers[Register.A]);
    }

    [Fact]
    public void DEC_RR()
    {
        // DEC BC
        _ram[0] = 0x0B;

        _state.Registers.WritePair(Register.BC, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1233, _state.Registers.ReadPair(Register.BC));
    }

    [Fact]
    public void HALT()
    {
        // HALT
        _ram[0] = 0x76;

        _processor.ProcessInstruction(_ram);

        Assert.True(_state.Halted);
    }
}