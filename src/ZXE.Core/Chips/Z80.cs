using System.Runtime.CompilerServices;
using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Chips;

public class Z80
{
    private readonly CpuState _cpuState;

    private Instruction[]? _instructions;

    public Z80(Model model, string path)
    {
        _cpuState = new CpuState(model == Model.Spectrum48K ? Constants.K64 : Constants.K128);

        LoadRoms(model, path);

        InitialiseInstructions();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Tick()
    {
    }

    private void LoadRoms(Model model, string romPath)
    {
        if (! Path.Exists(romPath))
        {
            throw new RomNotFoundException($"No ROMs found at {romPath}.");
        }

        switch (model)
        {
            case Model.Spectrum48K:
                LoadRom($"{romPath}\\image-0.rom", 0);

                break;

            default:
                throw new ChipNotSupportedException($"{model} not yet supported.");
        }
    }

    private void LoadRom(string path, int address)
    {
        var image = File.ReadAllBytes(path);

        Array.Copy(image, 0, _cpuState.Ram, address, image.Length);
    }

    private void InitialiseInstructions()
    {
        // TODO: Probably going to need to add a cycle count.
        var instructions = new Dictionary<int, Instruction>
                           {
                               { 0x0000, new Instruction("NOP", 1, (_, _) => Thread.Sleep(0)) },
                               { 0x0001, new Instruction("LD BC, nn", 3, (i, s) => { s.SetRegister(Register.C, i[1]); s.SetRegister(Register.B, i[2]); }) }
                           };

        _instructions = new Instruction[instructions.Max(i => i.Key + 1)];

        foreach (var instruction in instructions)
        {
            _instructions[instruction.Key] = instruction.Value;
        }
    }
}