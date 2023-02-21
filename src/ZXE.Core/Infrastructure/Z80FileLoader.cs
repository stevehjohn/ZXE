using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class Z80FileLoader
{
    private State _state;

    private Ram _ram;

    public Z80FileLoader(State state, Ram ram)
    {
        _state = state;

        _ram = ram;
    }

    public void Load(string fileName)
    {
        var data = File.ReadAllBytes(fileName);

        LoadRegisters(data);
    }

    private void LoadRegisters(byte[] data)
    {
        _state.Registers.WritePair(Register.AF, (ushort) (data[0] << 8 | data[1]));

        _state.Registers.WritePair(Register.BC, (ushort) (data[2] << 8 | data[3]));

        _state.Registers.WritePair(Register.HL, (ushort) (data[4] << 8 | data[5]));

        _state.ProgramCounter = (ushort) (data[6] << 8 | data[7]);

        _state.StackPointer = (ushort) (data[8] << 8 | data[9]);

        _state.Registers[Register.I] = data[10];

        _state.Registers[Register.R] = (byte) (data[11] | data[12] & 0x80);

        _state.Registers.WritePair(Register.DE, (ushort) (data[13] << 8 | data[14]));

        _state.Registers.WritePair(Register.BC1, (ushort) (data[15] << 8 | data[16]));

        _state.Registers.WritePair(Register.DE1, (ushort) (data[17] << 8 | data[18]));

        _state.Registers.WritePair(Register.HL1, (ushort) (data[19] << 8 | data[20]));

        _state.Registers.WritePair(Register.AF1, (ushort) (data[21] << 8 | data[21]));

        _state.Registers.WritePair(Register.IX, (ushort) (data[23] << 8 | data[24]));

        _state.Registers.WritePair(Register.IY, (ushort) (data[25] << 8 | data[26]));

        _state.InterruptFlipFlop1 = data[27] != 0;

        _state.InterruptFlipFlop2 = data[28] != 0;

        _state.InterruptMode = (InterruptMode) (data[29] & 0x03);
    }
}