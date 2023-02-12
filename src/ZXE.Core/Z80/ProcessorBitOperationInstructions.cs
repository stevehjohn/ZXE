using ZXE.Core.Extensions;

namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public static class ProcessorBitOperationInstructions
{
    public static bool BIT_b_R(Input input, byte bit, Register register)
    {
        var data = input.State.Registers[register];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool BIT_b_addr_RR(Input input, byte bit, Register register)
    {
        var data = input.Ram[input.State.Registers.ReadPair(register)];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool RES_b_R(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.State.Registers[register] = (byte) (input.State.Registers[register] & mask);

        // Flags unaffected

        return true;
    }

    public static bool RES_b_addr_RR(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.Ram[input.State.Registers.ReadPair(register)] = (byte) (input.Ram[input.State.Registers.ReadPair(register)] & mask);

        // Flags unaffected

        return true;
    }

    public static bool SET_b_R(Input input, byte bit, Register register)
    {
        input.State.Registers[register] |= bit;

        // Flags unaffected

        return true;
    }

    public static bool SET_b_addr_RR(Input input, byte bit, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] |= bit;

        // Flags unaffected

        return true;
    }

    public static bool BIT_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (data & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (data & 0x20) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 7 && result != 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool RES_b_addr_RR_plus_d_R(Input input, byte bit, Register source, Register destination)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        input.State.Registers[destination] = result;

        // Flags unaffected

        return true;
    }
    
    public static bool RES_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        // Flags unaffected

        return true;
    }

    public static bool SET_b_addr_RR_plus_d(Input input, byte bit, Register register)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        // Flags unaffected

        return true;
    }

    public static bool SET_b_addr_RR_plus_d_R(Input input, byte bit, Register register, Register destination)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        input.State.Registers[destination] = input.Ram[address];

        // Flags unaffected

        return true;
    }

    public static bool NEG_R(Input input, Register register)
    {
        var result = (byte) (0 - input.State.Registers[register]);

        input.State.Registers[register] = result;

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }
}