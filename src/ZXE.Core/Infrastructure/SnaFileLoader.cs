﻿using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class SnaFileLoader
{
    private readonly State _state;

    private readonly Ram _ram;

    public SnaFileLoader(State state, Ram ram)
    {
        _state = state;

        _ram = ram;
    }

    public void Load(string filename)
    {
        var data = File.ReadAllBytes(filename);

        LoadRam(data);

        LoadRegisters(data);
    }
    
    private void LoadRam(byte[] data)
    {
        var dataToLoad = data[0x1B..];

        _ram.Load(dataToLoad, 0x4000);
    }

    private void LoadRegisters(byte[] data)
    {
        _state.Registers[Register.I] = data[0];

        _state.Registers.WritePair(Register.HL_, (ushort) (data[0x02] << 8 | data[0x01]));

        _state.Registers.WritePair(Register.DE_, (ushort) (data[0x04] << 8 | data[0x03]));

        _state.Registers.WritePair(Register.BC_, (ushort) (data[0x06] << 8 | data[0x05]));

        _state.Registers.WritePair(Register.AF_, (ushort) (data[0x08] << 8 | data[0x07]));

        _state.Registers.WritePair(Register.HL, (ushort) (data[0x0A] << 8 | data[0x09]));

        _state.Registers.WritePair(Register.DE, (ushort) (data[0x0C] << 8 | data[0x0B]));

        _state.Registers.WritePair(Register.BC, (ushort) (data[0x0E] << 8 | data[0x0D]));

        _state.Registers.WritePair(Register.IY, (ushort) (data[0x10] << 8 | data[0x0F]));

        _state.Registers.WritePair(Register.IX, (ushort) (data[0x12] << 8 | data[0x11]));

        _state.InterruptFlipFlop1 = (data[0x13] & 0x04) > 0;

        _state.InterruptFlipFlop2 = (data[0x13] & 0x04) > 0;

        _state.Registers[Register.R] = data[0x14];

        _state.Registers.WritePair(Register.AF, (ushort) ((data[0x16] << 8) | data[0x15]));

        _state.StackPointer = (ushort) (data[0x18] << 8 | data[0x17]);

        _state.InterruptMode = (InterruptMode) data[0x19];

        _state.Flags = Flags.FromByte(_state.Registers[Register.F]);

        _state.OpcodePrefix = 0;
        
        var value = (ushort) _ram[_state.StackPointer];

        _state.StackPointer++;

        value |= (ushort) (_ram[_state.StackPointer] << 8);

        _state.StackPointer++;

        _state.ProgramCounter = value;
    }
}