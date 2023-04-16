namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

    public Action<byte>? PagedEvent { set; private get; }

    public Ports()
    {
        _ports = new byte?[0x010000];

        ResetKeyboardPorts();
    }

    public byte ReadByte(ushort port)
    {
        if (_ports[port] == null)
        {
            return 0;
        }

        var value = _ports[port]!.Value;

        return value;
    }

    public void WriteByte(ushort port, byte data)
    {
        _ports[port] = data;

        if ((port == 0x7FFD || port == 0x1FFD) && PagedEvent != null)
        {
            PagedEvent(data);
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