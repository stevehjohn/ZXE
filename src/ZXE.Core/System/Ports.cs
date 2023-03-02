using System.Diagnostics;

namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

    public Ports()
    {
        _ports = new byte?[256];

        ResetKeyboardPorts();
    }

    public byte ReadByte(byte port)
    {
        if (_ports[port] == null)
        {
            return 0;
        }

        var value = _ports[port]!.Value;

        Debugger.Log(0, "INFO", $"Port: {port:X2}, Value: {value}\n");

        ResetPort(port);

        return value;
    }

    public void WriteByte(byte port, byte data)
    {
        _ports[port] = data;
    }

    private void ResetPort(byte port)
    {
        if (port == 0xFE || port == 0xFD || port == 0xFB || port == 0xF7 || port == 0xEF || port == 0xDF || port == 0xBF || port == 0x7F)
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