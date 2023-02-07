using ZXE.Core.Infrastructure;
using ZXE.Core.Z80;

namespace ZXE.Core.System;

public class Ram
{
    private readonly byte[] _ram;

    public int Size { get; }

    public Ram(Model model)
    {
        var size = model == Model.Spectrum48K ? Constants.K64 : Constants.K128;
        
        _ram = new byte[size];

        Size = size;
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

    public byte[] GetData(int start, int length)
    {
        if (start + length - 1 < Size)
        {
            return this[start..(start + length)];
        }

        return Array.Empty<byte>();
    }
}