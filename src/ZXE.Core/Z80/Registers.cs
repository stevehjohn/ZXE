using ZXE.Core.Exceptions;

namespace ZXE.Core.Z80;

public class Registers
{
    private readonly byte[] _registers;

    public Registers()
    {
        _registers = new byte[22];
    }

    public void LoadFromRam(Register register, byte[] value)
    {
        var registerLocations = (int) register;

        _registers[(registerLocations & 0xFF00) >> 8] = value[1];
        _registers[registerLocations & 0x00FF] = value[0];
    }

    public ushort ReadPair(Register register)
    {
        var registerLocations = (int) register;

        return (ushort) (_registers[(registerLocations & 0xFF00) >> 8] << 8 | _registers[registerLocations & 0x00FF]);
    }

    public void WritePair(Register register, ushort value)
    {
        var registerLocations = (int) register;

        _registers[(registerLocations & 0xFF00) >> 8] = (byte) ((value & 0xFF00) >> 8);
        _registers[registerLocations & 0x00FF] = (byte) (value & 0x00FF);
    }

    public byte this[Register register]
    {
        get => _registers[(int) register];
        set => _registers[(int) register] = value;
    }
}