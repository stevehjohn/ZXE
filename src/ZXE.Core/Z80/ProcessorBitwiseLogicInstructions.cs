﻿namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public static class ProcessorBitwiseLogicInstructions
{
    public static bool AND_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool AND_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.State.Registers[source];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Ram[input.State.Registers.ReadPair(source)];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool AND_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Data[1];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool AND_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool AND_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool AND_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] & input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can AND overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ ((input.State.Registers.ReadPair(source) & 0xFF00) >> 8);

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool XOR_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] ^ input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can XOR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | ((input.State.Registers.ReadPair(source) & 0xFF00) >> 8);

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.State.Registers.ReadPair(source) & 0x00FF;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool OR_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var result = input.State.Registers[destination] | input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = false;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO: Can OR overflow?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }
}