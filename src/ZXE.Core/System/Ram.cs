using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    private const int RamSize = Constants.K64;

    private readonly byte[] _ram;

    private readonly byte[][] _banks;

    public bool ProtectRom { get; set; }

    public Ram()
    {
        _ram = new byte[RamSize];

        _banks = new byte[8][];

        for (var b = 0; b < 8; b++)
        {
            _banks[b] = new byte[Constants.K16];
        }
    }

    public byte this[int address]
    {
        get => _ram[address & 0xFFFF];

        set => _ram[address & 0xFFFF] = address < 0x4000 && ProtectRom ? _ram[address] : value;
    }

    public byte[] this[Range range] => _ram[range];

    public void Load(byte[] data, int destination)
    {
        Array.Copy(data, 0, _ram, destination, data.Length);
    }

    public byte[] GetData(int start, int length)
    {
        // TODO: Wrap?

        if (start + length - 1 < RamSize)
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

            if (position >= RamSize)
            {
                position = 0;
            }
        }

        return bytes.ToArray();
    }
}