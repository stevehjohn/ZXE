namespace ZXE.Core.System;

public class Ports
{
    private readonly Queue<byte>?[] _input;

    public Ports()
    {
        _input = new Queue<byte>[65_536];
    }

    public byte ReadByte(int port)
    {
        return _input[port]!.Dequeue();
    }

    public void EnqueueInput(int port, byte data)
    {
        if (_input[port] == null)
        {
            _input[port] = new Queue<byte>();
        }

        _input[port]!.Enqueue(data);
    }
}