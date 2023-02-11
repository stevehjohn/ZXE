using ZXE.Core.System;

namespace ZXE.Core.Z80;

public class Input
{
    public byte[] Data { get; }

    public State State { get; }

    public Ram Ram { get; }

    public Ports Ports { get; }

    public Input(byte[] data, State state, Ram ram, Ports ports)
    {
        Data = data;

        State = state;

        Ram = ram;

        Ports = ports;
    }
}