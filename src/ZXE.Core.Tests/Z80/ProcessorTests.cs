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

        tracer.Verify(t => t.TraceBefore(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
        tracer.Verify(t => t.TraceAfter(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
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
        // LD (0x1234), A
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
        // LD A, (0x1234)
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
        _state.Registers[Register.A1] = 0x03;
        _state.Registers[Register.F1] = 0x04;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x03, _state.Registers[Register.A]);
        Assert.Equal(0x04, _state.Registers[Register.F]);
        Assert.Equal(0x01, _state.Registers[Register.A1]);
        Assert.Equal(0x02, _state.Registers[Register.F1]);
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
    public void RRCA()
    {
        // RRCA
        _ram[0] = 0x0F;

        _state.Registers[Register.A] = 0b11010010;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b01101001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

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

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x0F, _state.Registers[Register.B]);
        Assert.Equal(0x22, _state.ProgramCounter);

        _state.ProgramCounter = 0;
        _state.Registers[Register.B] = 0x01;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x00, _state.Registers[Register.B]);
        Assert.Equal(0x02, _state.ProgramCounter);
    }

    [Fact]
    public void RLA()
    {
        // RLA
        _ram[0] = 0x17;

        _state.Registers[Register.A] = 0b10000001;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00000010, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00000101, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00001010, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);
    }

    [Fact]
    public void JR_e()
    {
        // JR 0x20
        _ram[0] = 0x18;
        _ram[1] = 0x20;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x22, _state.ProgramCounter);
    }

    [Fact]
    public void RRA()
    {
        // RRA
        _ram[0] = 0x1F;

        _state.Registers[Register.A] = 0b10000010;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b01000001, _state.Registers[Register.A]);
        Assert.False(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00100000, _state.Registers[Register.A]);
        Assert.True(_state.Flags.Carry);

        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

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

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Zero = false;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void LD_addr_nn_RR()
    {
        // LD (0x1234), HL
        _ram[0] = 0x22;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _state.Registers.WritePair(Register.HL, 0x5678);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x78, _ram[0x1234]);
        Assert.Equal(0x56, _ram[0x1235]);
    }

    [Fact]
    public void JR_Z_e()
    {
        // JR Z, 0x10
        _ram[0] = 0x28;
        _ram[1] = 0x10;

        _state.Flags.Zero = false;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Zero = true;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x12, _state.ProgramCounter);
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

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x56, _state.Registers[Register.L]);
        Assert.Equal(0x78, _state.Registers[Register.H]);
    }

    [Fact]
    public void CPL()
    {
        // CPL
        _ram[0] = 0x2F;

        _state.Registers[Register.A] = 0b10100101;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b01011010, _state.Registers[Register.A]);
    }
    
    [Fact]
    public void JR_NC_e()
    {
        // JR NC, 0x10
        _ram[0] = 0x30;
        _ram[1] = 0x10;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Carry = false;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void LD_SP_nn()
    {
        // LD SP, 0x1234
        _ram[0] = 0x31;
        _ram[1] = 0x34;
        _ram[2] = 0x12;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1234, _state.StackPointer);
    }

    [Fact]
    public void INC_SP()
    {
        // INC SP
        _ram[0] = 0x33;

        _state.StackPointer = 0x1234;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1235, _state.StackPointer);
    }

    [Fact]
    public void INC_addr_RR()
    {
        // INC (HL)
        _ram[0] = 0x34;
        _ram[0x1234] = 0x09;

        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x0A, _ram[0x1234]);
    }

    [Fact]
    public void DEC_addr_RR()
    {
        // DEC (HL)
        _ram[0] = 0x35;
        _ram[0x1234] = 0x0A;

        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x09, _ram[0x1234]);
    }

    [Fact]
    public void LD_addr_RR_n()
    {
        // LD (HL), 0xAB
        _ram[0] = 0x36;
        _ram[1] = 0xAB;

        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0xAB, _ram[0x1234]);
    }

    [Fact]
    public void SCF()
    {
        // SCF
        _ram[0] = 0x37;

        _processor.ProcessInstruction(_ram);

        Assert.True(_state.Flags.Carry);
    }
        
    [Fact]
    public void JR_C_e()
    {
        // JR NC, 0x10
        _ram[0] = 0x38;
        _ram[1] = 0x10;

        _state.Flags.Carry = false;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x02, _state.ProgramCounter);

        _state.Flags.Carry = true;
        _state.ProgramCounter = 0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x12, _state.ProgramCounter);
    }

    [Fact]
    public void ADD_RR_SP()
    {
        // ADD HL, SP
        _ram[0] = 0x39;

        _state.Registers.WritePair(Register.HL, 0x0101);
        _state.StackPointer = 0xA0A0;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0xA1A1, _state.Registers.ReadPair(Register.HL));
    }

    [Fact]
    public void DEC_SP()
    {
        // DEC SP
        _ram[0] = 0x3B;

        _state.StackPointer = 0x1235;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1234, _state.StackPointer);
    }

    [Fact]
    public void CCF()
    {
        // CCF
        _ram[0] = 0x3F;

        _processor.ProcessInstruction(_ram);

        Assert.True(_state.Flags.Carry);
    }

    [Fact]
    public void LD_R_R()
    {
        // LD B, C
        _ram[0] = 0x41;

        _state.Registers[Register.C] = 0x12;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x12, _state.Registers[Register.B]);
    }

    [Fact]
    public void HALT()
    {
        // HALT
        _ram[0] = 0x76;

        _processor.ProcessInstruction(_ram);

        Assert.True(_state.Halted);
    }

    [Fact]
    public void ADD_R_R()
    {
        // ADD A, B
        _ram[0] = 0x80;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1A, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADD_R_addr_RR()
    {
        // ADD A, (HL)
        _ram[0] = 0x86;
        _ram[0x1234] = 0x0A;

        _state.Registers[Register.A] = 0x10;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1A, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADC_R_R()
    {
        // ADC A, B
        _ram[0] = 0x88;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1A, _state.Registers[Register.A]);

        _state.ProgramCounter = 0;

        _state.Registers[Register.A] = 0x10;
        _state.Registers[Register.B] = 0x0A;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1B, _state.Registers[Register.A]);
    }

    [Fact]
    public void ADC_R_addr_RR()
    {
        // ADC A, (HL)
        _ram[0] = 0x8E;
        _ram[0x1234] = 0x0A;

        _state.Registers[Register.A] = 0x10;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1A, _state.Registers[Register.A]);

        _state.ProgramCounter = 0;

        _state.Registers[Register.A] = 0x10;

        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x1B, _state.Registers[Register.A]);
    }

    [Fact]
    public void SUB_R_R()
    {
        // SUB A, B
        _ram[0] = 0x90;

        _state.Registers[Register.A] = 0x0A;
        _state.Registers[Register.B] = 0x05;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x05, _state.Registers[Register.A]);
    }

    [Fact]
    public void SUB_R_addr_RR()
    {
        // SUB A, (HL)
        _ram[0] = 0x96;
        _ram[0x1234] = 0x10;

        _state.Registers[Register.A] = 0x20;
        _state.Registers.WritePair(Register.HL, 0x1234);
        
        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x10, _state.Registers[Register.A]);
    }

    [Fact]
    public void SBC_R_R()
    {
        // SBC A, B
        _ram[0] = 0x98;

        _state.Registers[Register.A] = 0x20;
        _state.Registers[Register.B] = 0x10;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x10, _state.Registers[Register.A]);

        _state.ProgramCounter = 0;

        _state.Registers[Register.A] = 0x20;
        _state.Registers[Register.B] = 0x10;
        _state.Flags.Carry = true;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x0F, _state.Registers[Register.A]);
    }

    [Fact]
    public void SBC_R_addr_RR()
    {
        // SBC A, (HL)
        _ram[0] = 0x9E;
        _ram[0x1234] = 0x10;

        _state.Registers[Register.A] = 0x20;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0x10, _state.Registers[Register.A]);
    }

    [Fact]
    public void AND_R_R()
    {
        // AND A, B
        _ram[0] = 0xA0;
        
        _state.Registers[Register.A] = 0b00011000;
        _state.Registers[Register.B] = 0b00111100;

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00011000, _state.Registers[Register.A]);
    }

    [Fact]
    public void AND_R_addr_RR()
    {
        // AND A, (HL)
        _ram[0] = 0xA6;
        _ram[0x1234] = 0b00111100;

        _state.Registers[Register.A] = 0b00011000;
        _state.Registers.WritePair(Register.HL, 0x1234);

        _processor.ProcessInstruction(_ram);

        Assert.Equal(0b00011000, _state.Registers[Register.A]);
    }
}