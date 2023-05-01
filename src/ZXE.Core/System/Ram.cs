using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ram
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly byte[][] _banks;

    private readonly byte[][] _layout;

    private readonly byte[] _bankNumbers;

    private readonly byte[] _frame;

    private int _screen = 1;

    private int _rom = -1;

    public byte[] ScreenRam => _frame;

    public bool ProtectRom { get; set; }

    public byte[] BankNumbers => _bankNumbers;

    public int Rom => _rom;

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

        _bankNumbers = new byte[4];

        for (var b = 0; b < 9; b++)
        {
            _banks[b] = new byte[Constants.K16];
        }

        _frame = new byte[Constants.K16];

        // Note: Bank 9 (i.e. [8]) is a special case to contain the ROM
        // Default 5, 2, 0

        _layout[0] = _banks[8]; // 0x0000 - 0x3FFF
        _layout[1] = _banks[5]; // 0x4000 - 0x7FFF
        _layout[2] = _banks[2]; // 0x8000 - 0xBFFF
        _layout[3] = _banks[0]; // 0xC000 - 0xFFFF

        _bankNumbers[0] = 8;
        _bankNumbers[1] = 5;
        _bankNumbers[2] = 2;
        _bankNumbers[3] = 0;
    }

    public void FrameReady()
    {
        Array.Copy(_screen == 1 ? _banks[5] : _banks[7], 0, _frame, 0, Constants.K16);
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

            if (address < 0x4000 && ProtectRom)
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

        _bankNumbers[(startAddress & 0b1100_0000_0000_0000) >> 14] = (byte) bankNumber;
    }

    public void SetBankBySlotNumber(int slotNumber, int bankNumber)
    {
        _layout[slotNumber] = _banks[bankNumber];

        _bankNumbers[slotNumber] = (byte) bankNumber;
    }

    public void Load(byte[] data, int destination)
    {
        for (var i = 0; i < data.Length; i++)
        {
            this[destination + i] = data[i];
        }
    }

    public void LoadRom(byte[] data, int number, bool force = false)
    {
        if (number != _rom || force)
        {
            Array.Copy(data, 0, _layout[0], 0, data.Length);

            _rom = number;
        }
    }

    public void LoadIntoPage(int page, byte[] data)
    {
        Array.Copy(data, 0, _banks[page], 0, data.Length);
    }

    public byte[] ReadBank(int bank)
    {
        var result = new byte[Constants.K16];

        Array.Copy(_banks[bank], 0, result, 0, Constants.K16);

        return result;
    }
}