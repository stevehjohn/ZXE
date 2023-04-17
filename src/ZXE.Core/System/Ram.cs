using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    private const int RamSize = Constants.K64;

    private readonly byte[] _ram;

    private readonly byte[][] _banks;

    private int _bank;

    private int _screen = 1;

    public bool ProtectRom { get; set; }

    public byte[] ScreenRam
    {
        get
        {
            var screenRam = new byte[Constants.K16];

            if (_screen == 1)
            {
                Array.Copy(_ram, 0x4000, screenRam, 0, Constants.K16);
            }
            else
            {
                Array.Copy(_banks[7], 0, screenRam, 0, Constants.K16);
            }

            return screenRam;
        }
    }

    public Ram()
    {
        _ram = new byte[RamSize];

        _banks = new byte[8][];

        for (var b = 0; b < 8; b++)
        {
            _banks[b] = new byte[Constants.K16];
        }
    }

    public void SetScreen(int screen)
    {
        if (screen < 1 || screen > 2)
        {
            throw new RamException($"Invalid screen {screen}");
        }

        _screen = screen;
    }

    public void SetBank(int bank)
    {
        _bank = bank;

        Array.Copy(_banks[bank], 0, _ram, 0xC000, Constants.K16);
    }

    public void LoadIntoPage(int pageNumber, byte[] data)
    {
        Array.Copy(data, 0, _banks[pageNumber], 0, data.Length);

        if (pageNumber == 5)
        {
            Array.Copy(data, 0, _ram, 0x4000, data.Length);
        }

        if (pageNumber == 2)
        {
            Array.Copy(data, 0, _ram, 0x8000, data.Length);
        }

        if (pageNumber == 0)
        {
            Array.Copy(data, 0, _ram, 0xC000, data.Length);
        }
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