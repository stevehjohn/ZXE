using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram2
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly byte[][] _banks;

    private readonly byte[][] _layout;

    private int _screen = 1;

    public byte[] ScreenRam => _screen == 1 ? _banks[5] : _banks[7];

    public int Screen 
    {
        get => _screen;
        set 
        {
            if (value is not 1 or 2)
            {
                // TODO: Proper exception?
                throw new Exception("Invalid screen");
            }

            _screen = value;
        }
    }

    public Ram2()
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
        get => _layout[address & 0b1100_0000_0000_0000 >> 14][address & 0x0011_1111_1111_1111];
        set => _layout[address & 0b1100_0000_0000_0000 >> 14][address & 0x0011_1111_1111_1111] = value;
    }
}