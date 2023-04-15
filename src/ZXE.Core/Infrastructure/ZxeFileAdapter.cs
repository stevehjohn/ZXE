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

        _state.Registers = model.State.Registers;

        _state.StackPointer = model.State.StackPointer;

        _state.Registers.PutRawRegisters(model.Registers!);

        _ram.Load(model.Ram!, 0x4000);

        return model.RomTitle;
    }

    public void Save(string filename, string romTitle)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);

        var data = new ZxeFile
                   {
                       State = _state,
                       Ram = _ram[0x4000..],
                       Registers = _state.Registers.GetRawRegisters(),
                       RomTitle = romTitle
                   };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                                                  {
                                                      WriteIndented = true
                                                  });

        File.WriteAllText(filename, json);
    }
}