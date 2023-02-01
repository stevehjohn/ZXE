using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.Chips;

public class Z80
{
    private readonly byte[] _ram;

    public Z80(Model model, string path)
    {
        _ram = new byte[model == Model.Spectrum48K ? Constants.K64 : Constants.K128];

        LoadRoms(model, path);
    }

    private void LoadRoms(Model model, string path)
    {
        if (! Path.Exists(path))
        {
            throw new RomNotFoundException($"No ROMs found at {path}.");
        }

        switch (model)
        {
            case Model.Spectrum48K:
                LoadRom($"{path}\\image-0.rom", 0);

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