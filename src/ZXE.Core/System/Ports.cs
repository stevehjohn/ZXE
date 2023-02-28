namespace ZXE.Core.System;

public class Ports
{
    private readonly byte[] _ports;

    public Ports()
    {
        _ports = new byte[256];

        ResetKeyboardPorts();
    }

    public byte ReadByte(byte port)
    {
        return _ports[port];
    }

    public void WriteByte(byte port, byte data)
    {
        _ports[port] = data;
    }

    public void Clear()
    {
        for (var i = 0; i <= 0xFF; i++)
        {
            _ports[i] = 0;
        }

        ResetKeyboardPorts();
    }

    private void ResetKeyboardPorts()
    {
        _ports[0xFE] = 0xFF;
        _ports[0xFD] = 0xFF;
        _ports[0xFB] = 0xFF;
        _ports[0xF7] = 0xFF;
        _ports[0xEF] = 0xFF;
        _ports[0xDF] = 0xFF;
        _ports[0xBF] = 0xFF;
        _ports[0x7F] = 0xFF;
    }
}