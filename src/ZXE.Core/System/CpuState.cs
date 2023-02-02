using ZXE.Core.Infrastructure;

namespace ZXE.Core.System;

public class CpuState
{
    public readonly byte[] Registers = new byte[16];

    public short PC = 0;

    //private short _sp;

    //private short _ix;
    //private short _iy;

    //private byte _accumulator;
    //private byte _flags;

    public void SetRegister(Register register, byte value)
    {
        Registers[(int) register] = value;
    }
}