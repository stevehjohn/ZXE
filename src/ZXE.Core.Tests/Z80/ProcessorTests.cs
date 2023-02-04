using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

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
    public void LD_BC_nn()
    {
        _ram[0] = 0x01;
        _ram[1] = 0x39;
        _ram[2] = 0x30;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x30, _state.Registers[Register.B]);
        Assert.Equal(0x39, _state.Registers[Register.C]);
    }

    [Fact]
    public void LD_жBC_A()
    {
        _ram[0] = 0x02;

        _state.Registers[Register.B] = 0x01;
        _state.Registers[Register.C] = 0x02;

        _state.Registers[Register.A] = 0xAB;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0xAB, _ram[0x0102]);
    }

    [Fact]
    public void INC_BC()
    {
        _ram[0] = 0x03;

        _state.Registers[Register.B] = 0x00;
        _state.Registers[Register.C] = 0xFE;

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0xFF, _state.Registers[Register.C]);
        Assert.Equal(0x00, _state.Registers[Register.B]);

        _processor.ProcessInstruction(_ram, _state);

        Assert.Equal(0x00, _state.Registers[Register.C]);
        Assert.Equal(0x01, _state.Registers[Register.B]);
    }
}