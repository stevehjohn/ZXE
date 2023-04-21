using System.Diagnostics;
using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

    public Action<byte, byte>? PagedEvent { set; private get; }

    private readonly Model _model;

    public Ports(Model model = Model.Spectrum48K)
    {
        _ports = new byte?[0x0001_0000];

        // Kempston. Required for Manic Miner.
        _ports[0x001f] = 0;
        _ports[0x011f] = 0;

        _model = model;

        ResetKeyboardPorts();
    }

    public byte ReadByte(ushort port)
    {
        var value = _ports[port] ?? 0xFF;

        // Keyboard.
        if ((port & 0xFE) == 0xFE)
        {
            value = 0xFF;

            var high = (port & 0xFF00) >> 8;

            if ((high & 0b0000_0001) == 0) value &= _ports[0b1111_1110_1111_1110] ?? 0xFF;
            if ((high & 0b0000_0010) == 0) value &= _ports[0b1111_1101_1111_1110] ?? 0xFF;
            if ((high & 0b0000_0100) == 0) value &= _ports[0b1111_1011_1111_1110] ?? 0xFF;
            if ((high & 0b0000_1000) == 0) value &= _ports[0b1111_0111_1111_1110] ?? 0xFF;
            if ((high & 0b0001_0000) == 0) value &= _ports[0b1110_1111_1111_1110] ?? 0xFF;
            if ((high & 0b0010_0000) == 0) value &= _ports[0b1101_1111_1111_1110] ?? 0xFF;
            if ((high & 0b0100_0000) == 0) value &= _ports[0b1011_1111_1111_1110] ?? 0xFF;
            if ((high & 0b1000_0000) == 0) value &= _ports[0b0111_1111_1111_1110] ?? 0xFF;
        }

        // Disk drive.
        //if ((port & 0x2FFD) > 0)
        //{
        //    value = 0xFF;
        //}

        return value;
    }

    // TODO: This should really be on the motherboard.
    private bool _pagingDisabled;

    public void WriteByte(ushort port, byte data)
    {
        _ports[port] = data;

        if (PagedEvent == null || _model == Model.Spectrum48K || _pagingDisabled)
        {
            return;
        }

        if ((port & 0x01) != 0)
        {
            var paging = (port & 0b1000_0000_0000_0010) == 0;

            if (_model == Model.SpectrumPlus3)
            {
                paging &= (port & 0b0100_0000_0000_0000) > 0;
            }

            if (paging)
            {
                if ((data & 0b00100000) > 0)
                {
                    _pagingDisabled = true;

                    return;
                }

                PagedEvent(0x7F, data);
            }

            if (_model != Model.SpectrumPlus3)
            {
                return;
            }
            
            paging = (port & 0b1110_0000_0000_0010) == 0;

            paging &= (port & 0b0001_0000_0000_0000) > 0;

            if (paging)
            {
                PagedEvent(0x1F, data);
            }
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