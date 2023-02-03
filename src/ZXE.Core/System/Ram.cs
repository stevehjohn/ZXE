using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    private readonly byte[] _ram;

    public Ram(Model model)
    {
        _ram = new byte[model == Model.Spectrum48K ? Constants.K64 : Constants.K128];
    }

    public byte this[int address]
    {
        get => _ram[address];
        set => _ram[address] = value;
    }

    public byte[] this[Range range] => _ram[range];

    public void Load(byte[] data, int destination)
    {
        Array.Copy(data, 0, _ram, destination, data.Length);
    }
}