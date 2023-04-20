using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly byte[][] _banks;

    private readonly byte[][] _layout;

    private int _screen = 1;

    public byte[] ScreenRam => _screen == 1 ? _banks[5] : _banks[7];

    public bool ProtectRom { get; set; }

    public int Screen
    {
        get => _screen;
        set
        {
            if (value is < 1 or > 2)
            {
                // TODO: Proper exception?
                throw new Exception("Invalid screen");
            }

            _screen = value;
        }
    }

    public Ram()
    {
        _banks = new byte[9][];

        _layout = new byte[4][];

        for (var b = 0; b < 9; b++)
        {
            _banks[b] = new byte[Constants.K16];
        }

        // Note: Bank 9 (i.e. [8]) is a special case to contain the ROM
        // Default 5, 2, 0

        _layout[0] = _banks[8]; // 0x0000 - 0x0FFF
        _layout[1] = _banks[5]; // 0x4000 - 0x3FFF
        _layout[2] = _banks[2]; // 0x8000 - 0xBFFF
        _layout[3] = _banks[0]; // 0xC000 - 0xFFFF
    }

    public byte this[int address]
    {
        get
        {
            address &= 0xFFFF;

            return _layout[(address & 0b1100_0000_0000_0000) >> 14][address & 0b0011_1111_1111_1111];
        }

        set
        {
            address &= 0xFFFF;

            if (address < 0x4000)
            {
                return;
            }

            _layout[(address & 0b1100_0000_0000_0000) >> 14][address & 0b0011_1111_1111_1111] = value;
        }
    }

    public byte[] ReadBlock(Range range)
    {
        return ReadBlock(range.Start.Value, range.End.Value - range.Start.Value);
    }

    public byte[] ReadBlock(int start, int length)
    {
        var data = new byte[length];

        for (var i = 0; i < length; i++)
        {
            data[i] = this[start + i];
        }

        return data;
    }

    public void SetBank(int startAddress, int bankNumber)
    {
        _layout[(startAddress & 0b1100_0000_0000_0000) >> 14] = _banks[bankNumber];
    }

    public void Load(byte[] data, int destination)
    {
        for (var i = 0; i < data.Length; i++)
        {
            this[destination + i] = data[i];
        }
    }

    public void LoadRom(byte[] data)
    {
        Array.Copy(data, 0, _layout[0], 0, data.Length);
    }

    public void LoadIntoPage(int page, byte[] data)
    {
        Array.Copy(data, 0, _banks[page], 0, data.Length);
    }
}