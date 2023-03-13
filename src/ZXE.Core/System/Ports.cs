namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

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

        //ResetPort(port);

        return value;
    }

    public void WriteByte(ushort port, byte data)
    {
        _ports[port] = data;
    }

    private void ResetPort(ushort port)
    {
        if (port == 0xFEFE || port == 0xFDFE || port == 0xFBFE || port == 0xF7FE || port == 0xEFFE || port == 0xDFFE || port == 0xBFFE || port == 0x7FFE)
        {
            _ports[port] = 0xFF;
        }
        else
        {
            _ports[port] = null;
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