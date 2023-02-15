namespace ZXE.Core.System;

public class Bus
{
    public byte? Data { get; set; }

    public bool NonMaskableInterrupt { get; set; }

    public bool Interrupt { get; set; }
}