using ZXE.Core.System;

namespace ZXE.Core.Z80;

public class Input
{
    public byte[] Data { get; }

    public State State { get; }

    public Ram Ram { get; }

    public Input(byte[] data, State state, Ram ram)
    {
        Data = data;

        State = state;

        Ram = ram;
    }
}