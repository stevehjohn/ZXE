using ZXE.Core.Extensions;

namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public static class ProcessorBitwiseRotationInstructions
{
    public static int RLCA(Input input)
    {
        unchecked
        {
            var topBit = (byte) ((input.State.Registers[Register.A] & 0x80) >> 7);

            var result = (byte) (((input.State.Registers[Register.A] << 1) & 0xFE) | topBit);

            input.State.Registers[Register.A] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RRCA(Input input)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[Register.A] & 0x01;

            var result = (byte) (input.State.Registers[Register.A] >> 1);

            if (bottomBit == 1)
            {
                result |= 0x80;
            }

            input.State.Registers[Register.A] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RLA(Input input)
    {
        unchecked
        {
            var topBit = (input.State.Registers[Register.A] & 0x80) >> 7;

            var result = (byte) (input.State.Registers[Register.A] << 1);

            result |= (byte) (input.State.Flags.Carry ? 1 : 0);

            input.State.Registers[Register.A] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RRA(Input input)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[Register.A] & 0x01;

            var result = (byte) (input.State.Registers[Register.A] >> 1);

            result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

            input.State.Registers[Register.A] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RLC_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (byte) ((input.State.Registers[register] & 0x80) >> 7);

            var result = (byte) (((input.State.Registers[register] << 1) & 0xFE) | topBit);

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RLC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (byte) (data >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RRC_R(Input input, Register register)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[register] & 0x01;

            var result = (byte) (input.State.Registers[register] >> 1);

            if (bottomBit == 1)
            {
                result |= 0x80;
            }

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RRC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var result = (byte) (data >> 1);

            if (bottomBit == 1)
            {
                result |= 0x80;
            }

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RL_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (input.State.Registers[register] & 0x80) >> 7;

            var result = (byte) (input.State.Registers[register] << 1);

            result |= (byte) (input.State.Flags.Carry ? 1 : 0);

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RL_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            result |= (byte) (input.State.Flags.Carry ? 1 : 0);

             input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RR_R(Input input, Register register)
    {
        unchecked
        {
            var bottomBit = input.State.Registers[register] & 0x01;

            var result = (byte) (input.State.Registers[register] >> 1);

            result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RR_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var result = (byte) (data >> 1);

            result |= (byte) (input.State.Flags.Carry ? 0x80 : 0);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLA_R(Input input, Register register)
    {
        unchecked
        {
            var topBit = (input.State.Registers[register] & 0x80) >> 7;

            var result = input.State.Registers[register] <<= 1;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLA_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRA_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var bottomBit = data & 0x01;

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data >> 1);

            result |= topBit;

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRA_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = data & 0x01;

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data >> 1);

            result |= topBit;

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLS_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data << 1);

            result |= 0x01;

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = topBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLS_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var topBit = (byte) (data & 0x80);

            var result = (byte) (data << 1);

            result |= 0x01;

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = topBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRL_R(Input input, Register register)
    {
        unchecked
        {
            var data = input.State.Registers[register];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.State.Registers[register] = result;

            // Flags
            input.State.Flags.Carry = bottomBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRL_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var data = input.Ram[input.State.Registers.ReadPair(register)];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.Ram[input.State.Registers.ReadPair(register)] = result;

            // Flags
            input.State.Flags.Carry = bottomBit > 0;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLA_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int RLC_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int RLC_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | topBit);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RRC_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (bottomBit << 7));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int RRC_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (bottomBit << 7));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RL_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) | (byte) (input.State.Flags.Carry ? 0x01 : 0x00));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int RL_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) ((data & 0x80) >> 7);

            var result = (byte) (((data << 1) & 0xFE) |  (byte) (input.State.Flags.Carry ? 0x01 : 0x00));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int RR_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (byte) (input.State.Flags.Carry ? 0x80 : 0x00));

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int RR_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | (byte) (input.State.Flags.Carry ? 0x80 : 0x00));

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SLA_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) (data << 1);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRA_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) (data & 0x80);

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | topBit);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRA_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (byte) (data & 0x80);

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) ((data >> 1) | topBit);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int SLS_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) ((data << 1) | 0x01);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }

    public static int SLS_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var topBit = (data & 0x80) >> 7;

            var result = (byte) ((data << 1) | 0x01);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = topBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRL_addr_RR_plus_d(Input input, Register source)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();
        }

        return 0;
    }

    public static int SRL_addr_RR_plus_d_R(Input input, Register source, Register destination)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(source);

            address = (ushort) (address + (sbyte) input.Data[0]); // TODO: Wrap around? I think Ram class might cope TBH...

            var data = input.Ram[address];

            var bottomBit = (byte) (data & 0x01);

            var result = (byte) (data >> 1);

            input.State.Registers[destination] = result;

            input.Ram[address] = result;

            // Flags
            input.State.Flags.Carry = bottomBit == 1;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result.IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.PutFlagsInFRegister();

            input.State.MemPtr = address;
        }

        return 0;
    }
}