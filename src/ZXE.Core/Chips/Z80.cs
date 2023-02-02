using System.Runtime.CompilerServices;
using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Chips;

public class Z80
{
    private readonly Ram _ram;

    private readonly InstructionProcessor _instructionProcessor;

    public Z80(Ram ram)
    {
        var cpuState = new CpuState();

        _ram = ram;

        _instructionProcessor = new InstructionProcessor(cpuState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Tick()
    {
        _instructionProcessor.ProcessInstruction();
    }

    public void LoadRoms(Model model, string romPath)
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