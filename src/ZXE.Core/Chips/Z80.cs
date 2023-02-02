using System.Runtime.CompilerServices;
using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Chips;

public class Z80
{
    private readonly Ram _ram;

    private readonly CpuState _cpuState;

    private readonly InstructionProcessor _instructionProcessor;

    public Z80(Model model, string path)
    {
        _cpuState = new CpuState();

        _ram = new Ram(model);

        LoadRoms(model, path);

        _instructionProcessor = new InstructionProcessor(_cpuState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Tick()
    {
        _instructionProcessor.ProcessInstruction();
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

        _ram.Load(image, address);
    }
}