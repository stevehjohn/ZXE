using Moq;
using Xunit;
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

    private readonly Bus _bus;

    public ProcessorTests()
    {
        _processor = new Processor();

        _ram = new Ram();

        _state = new State();

        _processor.SetState(_state);

        _ports = new Ports();

        _bus = new Bus();
    }

    [Fact]
    public void Passes_correct_number_of_bytes_to_instruction()
    {
        _ram[0xFFFD] = 0x01;

        _state.ProgramCounter = 0xFFFD;

        _processor.ProcessInstruction(_ram, _ports, _bus);
    }

    [Fact]
    public void Calls_tracer_if_present()
    {
        var tracer = new Mock<ITracer>();

        var processor = new Processor(tracer.Object);

        _ram[0] = 0x00;

        processor.ProcessInstruction(_ram, _ports, _bus);

        tracer.Verify(t => t.TraceBefore(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
        tracer.Verify(t => t.TraceAfter(It.Is<Instruction>(i => i.Mnemonic == "NOP"), new byte[] { 0x00 }, It.IsAny<State>(), It.IsAny<Ram>()));
    }

    [Fact]
    public void Can_execute_DD_instruction()
    {
        // LD IX, nn
        _ram[0x4000] = 0xDD;
        _ram[0x4001] = 0x21;
        _ram[0x4002] = 0x34;
        _ram[0x4003] = 0x42;

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x4234, _state.Registers.ReadPair(Register.IX));
    }

    [Fact]
    public void Can_execute_FD_instruction()
    {
        // LD IY, nn
        _ram[0x4000] = 0xFD;
        _ram[0x4001] = 0x21;
        _ram[0x4002] = 0x34;
        _ram[0x4003] = 0x12;

        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0x1234, _state.Registers.ReadPair(Register.IY));
    }

    [Fact]
    public void Can_execute_DDCB_instruction()
    {
        // LD IXl, 0x24
        _ram[0x4000] = 0xDD;
        _ram[0x4001] = 0x2E;
        _ram[0x4002] = 0x2A;
        // LD IXh, 0x12
        _ram[0x4003] = 0xDD;
        _ram[0x4004] = 0x26;
        _ram[0x4005] = 0x42;
        // SLA (IX + d), C
        _ram[0x4006] = 0xDD;
        _ram[0x4007] = 0xCB;
        _ram[0x4008] = 0x0A;
        _ram[0x4009] = 0x21;

        _ram[0x4234] = 0b00100010;
        
        _state.ProgramCounter = 0x4000;

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        _processor.ProcessInstruction(_ram, _ports, _bus);

        Assert.Equal(0b01000100, _state.Registers[Register.C]);
    }
}