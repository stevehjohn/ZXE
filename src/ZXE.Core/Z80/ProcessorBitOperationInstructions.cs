namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public static class ProcessorBitOperationInstructions
{
    public static int BIT_b_R(Input input, byte bit, Register register)
    {
        var data = input.State.Registers[register];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = bit == 0x08 && result != 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = bit == 0x20 && result != 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = bit == 0x80 && result != 0;

        input.State.PutFlagsInFRegister();

        return 0;
    }

    public static int BIT_b_addr_RR(Input input, byte bit, Register register)
    {
        var data = input.Ram[input.State.Registers.ReadPair(register)];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        //input.State.Flags.X1 = (input.State.MemPtr & 0x0800) > 0;
        input.State.Flags.HalfCarry = true;
        //input.State.Flags.X2 = (input.State.MemPtr & 0x2000) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 0x80 && result != 0;

        input.State.PutFlagsInFRegister();

        return 0;
    }

    public static int RES_b_R(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.State.Registers[register] = (byte) (input.State.Registers[register] & mask);

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int RES_b_addr_RR(Input input, byte bit, Register register)
    {
        var mask = (byte) ~bit;

        input.Ram[input.State.Registers.ReadPair(register)] = (byte) (input.Ram[input.State.Registers.ReadPair(register)] & mask);

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int SET_b_R(Input input, byte bit, Register register)
    {
        input.State.Registers[register] |= bit;

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int SET_b_addr_RR(Input input, byte bit, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] |= bit;

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int BIT_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & bit);

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result == 0;
        input.State.Flags.X1 = (address & 0x0800) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (address & 0x2000) > 0;
        input.State.Flags.Zero = (data & bit) == 0;
        input.State.Flags.Sign = bit == 0x80 && result != 0;

        input.State.PutFlagsInFRegister();

        input.State.MemPtr = address;

        return 0;
    }

    public static int RES_b_addr_RR_plus_d_R(Input input, byte bit, Register source, Register destination)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        input.State.Registers[destination] = result;

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = address;

        return 0;
    }
    
    public static int RES_b_addr_RR_plus_d(Input input, byte bit, Register source)
    {
        var address = input.State.Registers.ReadPair(source);

        address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

        var data = input.Ram[address];

        var result = (byte) (data & ~bit);

        input.Ram[address] = result;

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int SET_b_addr_RR_plus_d(Input input, byte bit, Register register)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int SET_b_addr_RR_plus_d_R(Input input, byte bit, Register register, Register destination)
    {
        var address = (ushort) (input.State.Registers.ReadPair(register) + (sbyte) input.Data[0]);

        input.Ram[address] |= bit;

        input.State.Registers[destination] = input.Ram[address];

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = address;

        return 0;
    }

    public static int NEG_R(Input input, Register register)
    {
        //var result = (byte) (0 - input.State.Registers[register]);

        var value = input.State.Registers[register];

        var result = (byte) ~value;

        result++;

        input.State.Registers[register] = result;

        // Flags
        input.State.Flags.Carry = value != 0;
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = value == 0x80;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) + (result & 0x0F) > 0x0F;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (result & 0x80) > 0;

        input.State.PutFlagsInFRegister();

        return 0;
    }
}