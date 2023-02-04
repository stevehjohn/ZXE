using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

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