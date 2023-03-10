using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    private readonly byte[] _ram;

    public int Size { get; }

    public bool ProtectRom { get; set; }

    public Ram(Model model)
    {
        var size = model == Model.Spectrum48K ? Constants.K64 : Constants.K128;

        _ram = new byte[size];

        Size = size;
    }

    public byte this[int address]
    {
        get
        {
            if (address >= Size)
            {
                address -= Size;
            }

            if (address < 0)
            {
                address += Size;
            }

            return _ram[address];
        }

        set
        {
            if (address < 0x4000 && ProtectRom)
            {
                return;
            }

            if (address >= Size)
            {
                address -= Size;
            }

            if (address < 0)
            {
                address += Size;
            }

            _ram[address] = value;
        }
    }

    public byte[] this[Range range] => _ram[range];

    public void Load(byte[] data, int destination)
    {
        Array.Copy(data, 0, _ram, destination, data.Length);
    }

    public byte[] GetData(int start, int length)
    {
        // TODO: Wrap?

        if (start + length - 1 < Size)
        {
            return this[start..(start + length)];
        }

        var bytes = new List<byte>();

        var position = start;

        while (length > 0)
        {
            bytes.Add(_ram[position]);

            length--;

            position++;

            if (position >= Size)
            {
                position = 0;
            }
        }

        return bytes.ToArray();
    }
}