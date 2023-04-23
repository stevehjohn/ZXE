using System.Text.Json;
using ZXE.Core.Infrastructure.Models;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class ZxeFileAdapter
{
    private readonly State _state;

    private readonly Ram _ram;

    public ZxeFileAdapter(State state, Ram ram)
    {
        _state = state;

        _ram = ram;
    }

    public string? Load(string filename)
    {
        var json = File.ReadAllText(filename);

        var model = JsonSerializer.Deserialize<ZxeFile>(json);

        if (model == null)
        {
            return null;
        }

        _state.Flags = model.State!.Flags;

        _state.Halted = model.State.Halted;

        _state.InterruptFlipFlop1 = model.State.InterruptFlipFlop1;

        _state.InterruptFlipFlop2 = model.State.InterruptFlipFlop2;

        _state.InterruptMode = model.State.InterruptMode;

        _state.MemPtr = model.State.MemPtr;

        _state.OpcodePrefix = model.State.OpcodePrefix;

        _state.ProgramCounter = model.State.ProgramCounter;

        _state.Q = model.State.Q;

        _state.StackPointer = model.State.StackPointer;

        // TODO: Registers, RAM Banks, ROM

        return model.RomTitle;
    }

    public void Save(string filename, string romTitle)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);

        var data = new ZxeFile
                   {
                       State = _state,
                       RomTitle = romTitle
                   };

        data.Registers.Add("AF", _state.Registers.ReadPair(Register.AF));
        data.Registers.Add("BC", _state.Registers.ReadPair(Register.BC));
        data.Registers.Add("DE", _state.Registers.ReadPair(Register.DE));
        data.Registers.Add("HL", _state.Registers.ReadPair(Register.HL));

        data.Registers.Add("AF'", _state.Registers.ReadPair(Register.AF_));
        data.Registers.Add("BC'", _state.Registers.ReadPair(Register.BC_));
        data.Registers.Add("DE'", _state.Registers.ReadPair(Register.DE_));
        data.Registers.Add("HL'", _state.Registers.ReadPair(Register.HL_));

        data.Registers.Add("IX'", _state.Registers.ReadPair(Register.IX));
        data.Registers.Add("IY'", _state.Registers.ReadPair(Register.IY));

        data.Registers.Add("IR'", (ushort) ((_state.Registers[Register.I] << 8) & _state.Registers[Register.R]));

        for (var i = 0; i < 8; i++)
        {
            data.RamBanks.Add(i, _ram.ReadBank(i));
        }

        for (var i = 0; i < 4; i++)
        {
            data.PageConfiguration.Add(i, _ram.BankNumbers[i]);
        }

        data.Rom = _ram.ReadBank(8);

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                                                  {
                                                      WriteIndented = true
                                                  });

        File.WriteAllText(filename, json);
    }
}