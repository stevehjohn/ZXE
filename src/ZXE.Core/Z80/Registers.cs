using ZXE.Core.Infrastructure;

namespace ZXE.Core.Z80;

public class Registers
{
    private readonly byte[] _registers;

    public Registers()
    {
        _registers = new byte[16];
    }

    public ushort this[Register register]
    {
        get 
        { 
            var registerLocations = (int) register;

            if (registerLocations < 256)
            {
                return _registers[registerLocations];
            }
            else
            {
                // TODO: Verify byte ordering, somehow...
                return _registers[registerLocations];
            }
        }
        set
        {
            var registerLocations = (int) register;

            if (registerLocations < 256)
            {
                _registers[registerLocations] = (byte) (value & 0xFF);
            }
            else
            {
                // TODO: Verify byte ordering, somehow...
                _registers[(registerLocations & 0xFF00) >> 8] = (byte) (value & 0x00FF);
                _registers[registerLocations & 0x00FF] = (byte) ((value & 0xFF00) >> 8);
            }
        }
    }
}