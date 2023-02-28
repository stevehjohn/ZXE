﻿using ZXE.Core.Extensions;

namespace ZXE.Core.Z80;

public static class ProcessorArithmeticInstructions
{
    public static int INC_RR(Input input, Register register)
    {
        unchecked
        {
            input.State.Registers.WritePair(register, (ushort) (input.State.Registers.ReadPair(register) + 1));
        }

        // Flags unaffected

        return 0;
    }

    public static int INC_R(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers[register];

            var result = (byte) (value + 1);

            input.State.Registers[register] = result;

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int DEC_R(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers[register];

            var result = (byte) (value - 1);

            input.State.Registers[register] = result;

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = value == 0x80;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) < 1;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }    

    public static int ADD_RR_RR(Input input, Register target, Register operand)
    {
        unchecked
        {
            var source = (int) input.State.Registers.ReadPair(target);

            var result = source + input.State.Registers.ReadPair(operand);

            input.State.Registers.WritePair(target, (ushort) result);

            // Flags
            input.State.Flags.Carry = result > 0xFFFF;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x0800) > 0;
            input.State.Flags.HalfCarry = (source & 0x0800) > 0 && (result & 0x1000) > 0;
            input.State.Flags.X2 = (result & 0x2000) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int DEC_RR(Input input, Register register)
    {
        var result = input.State.Registers.ReadPair(register);

        result--;

        input.State.Registers.WritePair(register, result);

        // Flags unaffected

        return 0;
    }

    public static int INC_SP(Input input)
    {
        input.State.StackPointer++;

        // Flags unaffected

        return 0;
    }

    public static int INC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(register);

            var value = input.Ram[address];

            var result = ++input.Ram[address];

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0x0F;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int DEC_addr_RR(Input input, Register register)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(register);

            var value = input.Ram[address];

            var result = --input.Ram[address];

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = value == 0x80;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) < 1;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADD_RR_SP(Input input, Register register)
    {
        unchecked
        {
            var source = input.State.StackPointer;

            var result = source + input.State.Registers.ReadPair(register);

            input.State.Registers.WritePair(register, (ushort) result);

            // Flags
            input.State.Flags.Carry = result > 0xFFFF;
            input.State.Flags.AddSubtract = false;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (source & 0x0800) > 0 && (result & 0x1000) > 0;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int DEC_SP(Input input)
    {
        input.State.StackPointer--;

        // Flags unaffected

        return 0;
    }

    public static int ADD_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var result = valueD + valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = ((valueD ^ valueS) & 0x80) == 0 && ((valueD ^ result) & 0x80) != 0;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADD_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var result = valueD + valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = ((valueD ^ valueS) & 0x80) == 0 && ((valueD ^ result) & 0x80) != 0;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = ((valueD ^ valueS) & 0x80) == 0 && ((valueD ^ result) & 0x80) != 0;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = ((valueD ^ valueS) & 0x80) == 0 && ((valueD ^ result) & 0x80) != 0;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_R(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers[source];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_addr_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source)];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADD_R_n(Input input, Register register)
    {
        unchecked
        {
            var original = input.State.Registers[register];

            var result = input.State.Registers[register] + input.Data[1];

            input.State.Registers[Register.A] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (original & 0x0F) < (result & 0x10);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_n(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Data[1];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = ((byte) result).IsEvenParity();
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_n(Input input, Register register)
    {
        unchecked
        {
            var valueD = input.State.Registers[register];

            var valueS = input.Data[1];

            var result = (valueD - valueS);

            input.State.Registers[register] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = ((valueD ^ valueS) & 0x80) != 0 && ((valueS ^ result) & 0x80) == 0;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_n(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Data[1];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int INC_RRh(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) ((value & 0xFF00) + 0x0100);

            result |= (byte) (value & 0x00FF);

            input.State.Registers.WritePair(register, result);
            
            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;
        }

        return 0;
    }

    public static int DEC_RRh(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) ((value & 0xFF00) - 0x0100);

            result += (byte) (value & 0x00FF);

            input.State.Registers.WritePair(register, result);
            
            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int INC_RRl(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) (byte) ((value & 0x00FF) + 1);

            result |= (ushort) (value & 0xFF00);
            
            input.State.Registers.WritePair(register, result);

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;
        }

        return 0;
    }

    public static int DEC_RRl(Input input, Register register)
    {
        unchecked
        {
            var value = input.State.Registers.ReadPair(register);

            var result = (ushort) (byte) ((value & 0x00FF) - 1);

            result += (ushort) (value & 0xFF00);
            
            input.State.Registers.WritePair(register, result);
            
            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = value == 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = (sbyte) result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int INC_addr_RR_plus_d(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address]++;
        
        // Flags unaffected

        return 0;
    }

    public static int DEC_addr_RR_plus_d(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address]--;
        
        // Flags unaffected

        return 0;
    }

    public static int ADD_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var result = valueD + valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0xFF;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADD_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var result = valueD + valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0xFF;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADD_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var address = (int) input.State.Registers.ReadPair(source);

            address += (sbyte) input.Data[1];

            var valueS = input.Ram[address];

            var result = valueD + valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0xFF;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + (valueS & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var address = (int) input.State.Registers.ReadPair(source);

            address += (sbyte) input.Data[1];

            var valueS = input.Ram[address];
            
            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SUB_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            var result = valueD - valueS;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < (valueS & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_RRh(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = (input.State.Registers.ReadPair(source) & 0xFF00) >> 8;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_RRl(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.State.Registers.ReadPair(source) & 0x00FF;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers[destination];

            var valueS = input.Ram[input.State.Registers.ReadPair(source) + (sbyte) input.Data[1]];

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers[destination] = (byte) result;

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_RR_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.Registers.ReadPair(source);

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers.WritePair(destination, (ushort) result);

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_RR_RR(Input input, Register destination, Register source)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.Registers.ReadPair(source);

            var carry = (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers.WritePair(destination, (ushort) result);

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int SBC_RR_SP(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.StackPointer;

            var carry = (byte) (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD - valueS - carry;

            input.State.Registers.WritePair(destination, (ushort) result);

            // Flags
            input.State.Flags.Carry = result < 0;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = result < -0x80; // TODO: Potential bug here?
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) < ((valueS + carry) & 0x0F);
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int ADC_RR_SP(Input input, Register destination)
    {
        unchecked
        {
            var valueD = input.State.Registers.ReadPair(destination);

            var valueS = input.State.StackPointer;

            var carry = (input.State.Flags.Carry ? 0x01 : 0x00);

            var result = valueD + valueS + carry;

            input.State.Registers.WritePair(destination, (ushort) result);

            // Flags
            input.State.Flags.Carry = result > 0xFF;
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = result > 0x7F;
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = (valueD & 0x0F) + ((valueS + carry) & 0x0F) > 0xF;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }
}