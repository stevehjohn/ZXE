namespace ZXE.Core.System;

public class Ports
{
    private readonly byte[] _ports;

    public Action<byte, byte>? PagedEvent { set; private get; }

    public Ports()
    {
        _ports = new byte[0x01_0000];

        ResetKeyboardPorts();
    }

    public byte ReadByte(ushort port)
    {
        var value = (byte) 0x00;

        if ((port & 0xFE) == 0xFE)
        {
            value = 0xFF;

            var high = (port & 0xFF00) >> 8;

            if ((high & 0b0000_0001) == 0) value &= _ports[0b1111_1110_1111_1110];
            if ((high & 0b0000_0010) == 0) value &= _ports[0b1111_1101_1111_1110];
            if ((high & 0b0000_0100) == 0) value &= _ports[0b1111_1011_1111_1110];
            if ((high & 0b0000_1000) == 0) value &= _ports[0b1111_0111_1111_1110];
            if ((high & 0b0001_0000) == 0) value &= _ports[0b1110_1111_1111_1110];
            if ((high & 0b0010_0000) == 0) value &= _ports[0b1101_1111_1111_1110];
            if ((high & 0b0100_0000) == 0) value &= _ports[0b1011_1111_1111_1110];
            if ((high & 0b1000_0000) == 0) value &= _ports[0b0111_1111_1111_1110];
        }

        return value;
    }

    private bool _pagingDisabled;

    public void WriteByte(ushort port, byte data)
    {
        _ports[port] = data;

        if (port == 0x7FFD && PagedEvent != null && ! _pagingDisabled)
        {
            if ((data & 0b00100000) > 0)
            {
                _pagingDisabled = true;
            }

            PagedEvent(0x7F, data);
        }

        if (port == 0x1FFD && PagedEvent != null)
        {
            PagedEvent(0x1F, data);
        }
    }

    private void ResetKeyboardPorts()
    {
        _ports[0xFEFE] = 0xFF;
        _ports[0xFDFE] = 0xFF;
        _ports[0xFBFE] = 0xFF;
        _ports[0xF7FE] = 0xFF;
        _ports[0xEFFE] = 0xFF;
        _ports[0xDFFE] = 0xFF;
        _ports[0xBFFE] = 0xFF;
        _ports[0x7FFE] = 0xFF;
    }
}