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

    public ushort ReadByName(string register)
    {
        if (register.Length == 1)
        {
            return _registers[(int) Enum.Parse<Register>(register)];
        }

        return ReadPair(Enum.Parse<Register>(register));
    }

    public void WriteLow(Register register, byte value)
    {
        var lowPosition = ((int) register & 0xFF00) >> 8;

        _registers[lowPosition] = value;
    }

    public void WriteHigh(Register register, byte value)
    {
        var highPosition = (int) register & 0x00FF;

        _registers[highPosition] = value;
    }

    public byte ReadLow(Register register)
    {
        var highPosition = ((int) register & 0xFF00) >> 8;

        return _registers[highPosition];
    }

    public byte ReadHigh(Register register)
    {
        var lowPosition = (int) register & 0x00FF;

        return _registers[lowPosition];
    }
}