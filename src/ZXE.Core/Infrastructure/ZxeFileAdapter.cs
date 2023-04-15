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

    public void Load(string filename)
    {
        var json = File.ReadAllText(filename);

        var model = JsonSerializer.Deserialize<ZxeFile>(json);

        if (model == null)
        {
            return;
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
    }

    public void Save(string filename)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);

        var data = new ZxeFile
                   {
                       State = _state,
                       Ram = _ram[0x4000..],
                       Registers = _state.Registers.GetRawRegisters()
                   };

        var json = JsonSerializer.Serialize(data);

        File.WriteAllText(filename, json);
    }
}