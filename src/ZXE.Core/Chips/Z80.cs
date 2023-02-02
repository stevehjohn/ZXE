﻿using System.Runtime.CompilerServices;
using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.Chips;

public class Z80
{
    private readonly byte[] _ram;

    private short _pc;
    private short _sp;

    private short _ix;
    private short _iy;

    private byte _accumulator;
    private byte _flags;

    private readonly byte[] _registers = new byte[16];

    public Z80(Model model, string path)
    {
        _ram = new byte[model == Model.Spectrum48K ? Constants.K64 : Constants.K128];

        LoadRoms(model, path);
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

        Array.Copy(image, 0, _ram, address, image.Length);
    }
}