using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    private const int RamSize = Constants.K64;

    private readonly byte[] _ram;

    private readonly byte[][] _banks;

    private int _bank;

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

    public void SetBank(int bank)
    {
        _bank = bank;

        Array.Copy(_banks[bank], 0, _ram, 0xC000, Constants.K16);
    }

    public byte this[int address]
    {
        get => _ram[address & 0xFFFF];

        set
        {
            address &= 0xFFFF;

            _ram[address] = address < 0x4000 && ProtectRom ? _ram[address] : value;

            if (address >= 0xC000)
            {
                _banks[_bank][address - 0xC000] = value;
            }
        }
    }

    public byte[] this[Range range] => _ram[range];

    public void Load(byte[] data, int destination)
    {
        Array.Copy(data, 0, _ram, destination, data.Length);

        Array.Copy(_ram, 0xC000, _banks[_bank], 0, Constants.K16);
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