namespace ZXE.Core.System;

public class Ports
{
    private readonly byte?[] _input;

    public Ports()
    {
        _input = new byte?[65_536];
    }

    public byte ReadByte(int port)
    {
        if (_input[port] == null)
        {
            return 0;
        }

        return _input[port]!.Value;
    }

    public void WriteByte(int port, byte data)
    {
        _input[port] = data;
    }
}