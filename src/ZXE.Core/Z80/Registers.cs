using ZXE.Core.Infrastructure;

namespace ZXE.Core.Z80;

public class Registers
{
    private readonly byte[] _registers;

    public Registers()
    {
        _registers = new byte[16];
    }

    public byte this[Register register]
    {
        get => _registers[(int) register];
        set => _registers[(int) register] = value;
    }
}