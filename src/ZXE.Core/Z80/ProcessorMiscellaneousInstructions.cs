using ZXE.Core.Extensions;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public static class ProcessorMiscellaneousInstructions
{
    public static bool NOP()
    {
        // Flags unaffected

        return true;
    }

    public static bool EX_RR_R1R1(Input input, Register register1, Register register2)
    {
        var alternate1 = Enum.Parse<Register>($"{register1}1");

        var alternate2 = Enum.Parse<Register>($"{register2}1");

        (input.State.Registers[register1], input.State.Registers[alternate1]) = (input.State.Registers[alternate1], input.State.Registers[register1]);

        (input.State.Registers[register2], input.State.Registers[alternate2]) = (input.State.Registers[alternate2], input.State.Registers[register2]);

        // Flags unaffected

        return true;
    }

    // TODO: Lol, good luck adding a unit test for this one!
    public static bool DAA(Input input)
    {
        var adjust = 0;

        if (input.State.Flags.HalfCarry || (input.State.Registers[Register.A] & 0x0F) > 0x09)
        {
            adjust++;
        }

        if (input.State.Flags.Carry || input.State.Registers[Register.A] > 0x99)
        {
            adjust += 2;

            input.State.Flags.Carry = true;
        }

        if (input.State.Flags.AddSubtract && ! input.State.Flags.HalfCarry)
        {
            input.State.Flags.HalfCarry = false;
        }
        else
        {
            if (input.State.Flags.AddSubtract && input.State.Flags.HalfCarry)
            {
                input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < 0x06;
            }
            else
            {
                input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) >= 0x0A;
            }
        }

        switch (adjust)
        {
            case 1:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0xFA : 0x06);

                break;
            case 2:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0xA0 : 0x60);

                break;
            case 3:
                input.State.Registers[Register.A] += (byte) (input.State.Flags.AddSubtract ? 0x9A : 0x66);

                break;
        }

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = true;
        // TODO: ParityOverflow
        input.State.Flags.X1 = (input.State.Registers[Register.A] & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (input.State.Registers[Register.A] & 0x20) > 0;
        input.State.Flags.Zero = input.State.Registers[Register.A] == 0;
        input.State.Flags.Sign = (input.State.Registers[Register.A] & 0x80) > 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool CPL(Input input)
    {
        unchecked
        {
            var result = input.State.Registers[Register.A] ^ 0xFF;

            input.State.Registers[Register.A] = (byte) result;

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = true;
            // ParityOverflow unaffected
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = true;
            input.State.Flags.X2 = (result & 0x20) > 0;
            // Zero unaffected
            // Sign unaffected

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool SCF(Input input)
    {
        input.State.Flags.Carry = true;

        // TODO: XOR with Q register?
        var xFlags = input.State.Flags.ToByte() | input.State.Registers[Register.A];

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (xFlags & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (xFlags & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        return true;
    }

    public static bool CCF(Input input)
    {
        var value = input.State.Flags.Carry;

        input.State.Flags.Carry = ! input.State.Flags.Carry;

        // TODO: XOR with Q register?
        var xFlags = input.State.Flags.ToByte() | input.State.Registers[Register.A];

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (xFlags & 0x08) > 0;
        input.State.Flags.HalfCarry = value;
        input.State.Flags.X2 = (xFlags & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        return true;
    }

    public static bool HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected

        return false;
    }

    public static bool CP_R_R(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.State.Registers[right];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool CP_R_addr_RR(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.Ram[input.State.Registers.ReadPair(right)];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool POP_RR(Input input, Register register)
    {
        var data = (ushort) input.Ram[input.State.StackPointer];

        input.State.StackPointer++;

        data |= (ushort) (input.Ram[input.State.StackPointer] << 8);

        input.State.StackPointer++;

        input.State.Registers.WritePair(register, data);

        // Flags unaffected

        return true;
    }

    public static bool PUSH_RR(Input input, Register register)
    {
        unchecked
        {
            input.State.StackPointer--;

            var data = input.State.Registers.ReadPair(register);

            input.Ram[input.State.StackPointer] = (byte) ((data & 0xFF00) >> 8);

            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) (data & 0x00FF);

            // Flags unaffected
        }

        return true;
    }

    public static bool RST(Input input, byte pageZeroAddress)
    {
        var pc = input.State.ProgramCounter; // + 1;

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) ((pc & 0xFF00) >> 8);

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) (pc & 0x00FF);

        input.State.ProgramCounter = pageZeroAddress;

        // Flags unaffected

        return false;
    }

    public static bool OUT_addr_n_R(Input input, Register register)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return true;
    }

    public static bool EXX(Input input)
    {
        var bc = input.State.Registers.ReadPair(Register.BC);

        var de = input.State.Registers.ReadPair(Register.DE);

        var hl = input.State.Registers.ReadPair(Register.HL);

        input.State.Registers.WritePair(Register.BC, input.State.Registers.ReadPair(Register.BC1));

        input.State.Registers.WritePair(Register.DE, input.State.Registers.ReadPair(Register.DE1));

        input.State.Registers.WritePair(Register.HL, input.State.Registers.ReadPair(Register.HL1));

        input.State.Registers.WritePair(Register.BC1, bc);

        input.State.Registers.WritePair(Register.DE1, de);

        input.State.Registers.WritePair(Register.HL1, hl);

        // Flags unaffected

        return true;
    }

    public static bool EX_addr_SP_RR(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WriteLow(register, input.Ram[input.State.StackPointer + 1]);

        input.State.Registers.WriteHigh(register, input.Ram[input.State.StackPointer]);

        input.Ram[input.State.StackPointer] = (byte) (value & 0x00FF);

        input.Ram[input.State.StackPointer + 1] = (byte) ((value & 0xFF00) >> 8);

        // Flags unaffected

        return true;
    }

    public static bool EX_RR_RR(Input input, Register left, Register right)
    {
        var swap = input.State.Registers.ReadPair(left);

        input.State.Registers.WritePair(left, input.State.Registers.ReadPair(right));

        input.State.Registers.WritePair(right, swap);
        
        // Flags unaffected

        return true;
    }

    public static bool DI(Input input)
    {
        input.State.InterruptFlipFlop1 = false;
        
        input.State.InterruptFlipFlop2 = false;
        
        return true;
    }

    public static bool EI(Input input)
    {
        input.State.InterruptFlipFlop1 = true;
        
        input.State.InterruptFlipFlop2 = true;

        return true;
    }

    public static bool CP_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] - input.Data[1];

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

    public static bool CP_R_RRh(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = (input.State.Registers.ReadPair(right) & 0xFF00) >> 8;

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool CP_R_RRl(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.State.Registers.ReadPair(right) & 0x00FF;

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool CP_R_addr_RR_plus_d(Input input, Register left, Register right)
    {
        unchecked
        {
            var leftValue = input.State.Registers[left];

            var rightValue = input.Ram[input.State.Registers.ReadPair(right) + (sbyte) input.Data[1]];

            var difference = leftValue - rightValue;

            // Flags
            input.State.Flags.Carry = rightValue > leftValue;
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = false; // TODO: Can CP overflow?
            input.State.Flags.X1 = (rightValue & 0x08) > 0;
            input.State.Flags.HalfCarry = (leftValue & 0x0F) < (rightValue & 0x0F);
            input.State.Flags.X2 = (rightValue & 0x20) > 0;
            input.State.Flags.Zero = difference == 0;
            input.State.Flags.Sign = (byte) difference > 0x7F;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return true;
    }

    public static bool IM_m(Input input, InterruptMode mode)
    {
        input.State.InterruptMode = mode;

        // Flags unaffected

        return true;
    }

    public static bool IN_b_R_addr_n(Input input, Register register)
    {
        var result = input.Ports.ReadByte(input.Data[1]);

        input.State.Registers[register] = result;

        // Flags
        // Carry unaffected
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

    public static bool OUT_b_addr_n_R(Input input, Register register)
    {
        input.Ports.WriteByte(input.Data[1], input.State.Registers[register]);

        // Flags unaffected

        return true;
    }

    public static bool TST_R(Input input, Register register)
    {
        var result = (byte) (input.State.Registers[Register.A] & input.State.Registers[register]);

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool TST_addr_R(Input input, Register register)
    {
        var result = (byte) (input.State.Registers[Register.A] & input.State.Registers.ReadPair(register));

        // Flags
        input.State.Flags.Carry = false;
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = result.IsEvenParity();
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool IN_R_addr_RR(Input input, Register destination, Register source)
    {
        var value = input.Ports.ReadByte(input.State.Registers.ReadPair(source));

        input.State.Registers[destination] = value;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value.IsEvenParity();
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = value == 0;
        input.State.Flags.Sign = (sbyte) value < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool OUT_addr_RR_R(Input input, Register destination, Register source)
    {
        // Flags unaffected

        return true;
    }

    //public static bool MLT_RR(Input input, Register register)
    //{
    //    var value = (int) input.State.Registers.ReadLow(register);

    //    value *= input.State.Registers.ReadHigh(register);

    //    input.State.Registers.WritePair(register, (ushort) value);

    //    // Flags unaffected

    //    return true;
    //}

    public static bool IN_addr_RR(Input input, Register source)
    {
        var value = input.Ports.ReadByte(input.State.Registers.ReadPair(source));

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value.IsEvenParity();
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = value == 0;
        input.State.Flags.Sign = (sbyte) value < 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool OUT_addr_R_n(Input input, Register register, byte data)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return true;
    }

    public static bool LDI(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        input.Ram[input.State.Registers.ReadPair(Register.DE)] = value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.DE, (ushort) (input.State.Registers.ReadPair(Register.DE) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        value += input.State.Registers[Register.A];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool LDIR(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        input.Ram[input.State.Registers.ReadPair(Register.DE)] = value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.DE, (ushort) (input.State.Registers.ReadPair(Register.DE) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        value += input.State.Registers[Register.A];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
        
        // TODO: Correctly account for extra cycles?

        if (input.State.Registers.ReadPair(Register.BC) != 0)
        {
            input.State.ProgramCounter--;

            return false;
        }

        return true;
    }

    public static bool CPI(Input input)
    {
        var value = input.State.Registers.ReadPair(Register.HL);

        var difference = input.State.Registers[Register.A] - value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        // Flags
        input.State.Flags.Carry = value > input.State.Registers[Register.A];
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = difference == 0;
        input.State.Flags.Sign = difference > 0x7F;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool CPIR(Input input)
    {
        var value = input.State.Registers.ReadPair(Register.HL);

        var difference = input.State.Registers[Register.A] - value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        // Flags
        input.State.Flags.Carry = value > input.State.Registers[Register.A];
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = difference == 0;
        input.State.Flags.Sign = difference > 0x7F;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
        
        // TODO: Correctly account for extra cycles?

        if (input.State.Registers.ReadPair(Register.BC) != 0 && difference != 0)
        {
            input.State.ProgramCounter--;

            return false;
        }

        return true;
    }

    public static bool INI(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            return true;
        }
    }

    public static bool INIR(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            // TODO: Correctly account for extra cycles?

            if (input.State.Registers.ReadPair(Register.BC) != 0)
            {
                input.State.ProgramCounter--;

                return false;
            }

            return true;
        }
    }

    public static bool OUTI(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            return true;
        }
    }

    public static bool OTIR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            // TODO: Correctly account for extra cycles?

            if (input.State.Registers.ReadPair(Register.BC) != 0)
            {
                input.State.ProgramCounter--;

                return false;
            }

            return true;
        }
    }

    public static bool LDD(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        input.Ram[input.State.Registers.ReadPair(Register.DE)] = value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

        input.State.Registers.WritePair(Register.DE, (ushort) (input.State.Registers.ReadPair(Register.DE) - 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        value += input.State.Registers[Register.A];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool LDDR(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        input.Ram[input.State.Registers.ReadPair(Register.DE)] = value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

        input.State.Registers.WritePair(Register.DE, (ushort) (input.State.Registers.ReadPair(Register.DE) - 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        value += input.State.Registers[Register.A];

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (value & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
        
        // TODO: Correctly account for extra cycles?

        if (input.State.Registers.ReadPair(Register.BC) != 0)
        {
            input.State.ProgramCounter--;

            return false;
        }

        return true;
    }

    public static bool CPD(Input input)
    {
        var value = input.State.Registers.ReadPair(Register.HL);

        var difference = input.State.Registers[Register.A] - value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        // Flags
        input.State.Flags.Carry = value > input.State.Registers[Register.A];
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = difference == 0;
        input.State.Flags.Sign = difference > 0x7F;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return true;
    }

    public static bool CPDR(Input input)
    {
        var value = input.State.Registers.ReadPair(Register.HL);

        var difference = input.State.Registers[Register.A] - value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        // Flags
        input.State.Flags.Carry = value > input.State.Registers[Register.A];
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
        input.State.Flags.X1 = (value & 0x08) > 0;
        input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
        input.State.Flags.X2 = (value & 0x20) > 0;
        input.State.Flags.Zero = difference == 0;
        input.State.Flags.Sign = difference > 0x7F;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();
        
        // TODO: Correctly account for extra cycles?

        if (input.State.Registers.ReadPair(Register.BC) != 0)
        {
            input.State.ProgramCounter--;

            return false;
        }

        return true;
    }

    public static bool IND(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            return true;
        }
    }

    public static bool INDR(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            // TODO: Correctly account for extra cycles?

            if (input.State.Registers.ReadPair(Register.BC) != 0)
            {
                input.State.ProgramCounter--;

                return false;
            }

            return true;
        }
    }

    public static bool OUTD(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            return true;
        }
    }

    public static bool OTDR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            // TODO: Correctly account for extra cycles?

            if (input.State.Registers.ReadPair(Register.BC) != 0)
            {
                input.State.ProgramCounter--;

                return false;
            }

            return true;
        }
    }
}