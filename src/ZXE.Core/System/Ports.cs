namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _ports;

    public Ports()
    {
        _ports = new byte?[65_536];
    }

    public byte ReadByte(int port)
    {
        if (_ports[port] == null)
        {
            return 0;
        }

        return _ports[port]!.Value;
    }

    public void WriteByte(int port, byte data)
    {
        _ports[port] = data;
    }
}