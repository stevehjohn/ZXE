namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

    public Ports()
    {
        _ports = new byte?[256];
    }

    public byte ReadByte(byte port)
    {
        if (_ports[port] == null)
        {
            return 0;
        }

        return _ports[port]!.Value;
    }

    public void WriteByte(byte port, byte data)
    {
        _ports[port] = data;
    }
}