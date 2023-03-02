using System.Diagnostics;
using ZXE.Core.Extensions;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace ZXE.Core.Z80;

public static class ProcessorMiscellaneousInstructions
{
    public static int NOP()
    {
        // Flags unaffected

        return 0;
    }

    public static int EX_RR_R1R1(Input input, Register register1, Register register2)
    {
        var alternate1 = Enum.Parse<Register>($"{register1}_");

        var alternate2 = Enum.Parse<Register>($"{register2}_");

        (input.State.Registers[register1], input.State.Registers[alternate1]) = (input.State.Registers[alternate1], input.State.Registers[register1]);

        (input.State.Registers[register2], input.State.Registers[alternate2]) = (input.State.Registers[alternate2], input.State.Registers[register2]);

        // Flags unaffected

        return 0;
    }

    // TODO: Lol, good luck adding a unit test for this one!
    public static int DAA(Input input)
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
        input.State.Flags.ParityOverflow = input.State.Registers[Register.A].IsEvenParity();
        input.State.Flags.X1 = (input.State.Registers[Register.A] & 0x08) > 0;
        input.State.Flags.HalfCarry = true;
        input.State.Flags.X2 = (input.State.Registers[Register.A] & 0x20) > 0;
        input.State.Flags.Zero = input.State.Registers[Register.A] == 0;
        input.State.Flags.Sign = (input.State.Registers[Register.A] & 0x80) > 0;

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }

    public static int CPL(Input input)
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

        return 0;
    }

    public static int SCF(Input input)
    {
        input.State.Flags.Carry = true;

        // TODO: XOR with Q register?
        //var xFlags = input.State.Flags.ToByte() | input.State.Registers[Register.A];

        // Flags
        // Carry adjusted by operation
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        // X1 unaffected
        input.State.Flags.HalfCarry = false;
        // X2 unaffected
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }

    public static int CCF(Input input)
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

        return 0;
    }

    public static int HALT(Input input)
    {
        input.State.Halted = true;

        // Flags unaffected

        return -1;
    }

    public static int CP_R_R(Input input, Register left, Register right)
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

        return 0;
    }

    public static int CP_R_addr_RR(Input input, Register left, Register right)
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

        return 0;
    }

    public static int POP_RR(Input input, Register register)
    {
        var data = (ushort) input.Ram[input.State.StackPointer];

        input.State.StackPointer++;

        data |= (ushort) (input.Ram[input.State.StackPointer] << 8);

        input.State.StackPointer++;

        input.State.Registers.WritePair(register, data);

        // Flags unaffected

        return 0;
    }

    public static int PUSH_RR(Input input, Register register)
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

        return 0;
    }

    public static int RST(Input input, byte pageZeroAddress)
    {
        var pc = input.State.ProgramCounter + 1;

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) ((pc & 0xFF00) >> 8);

        input.State.StackPointer--;

        input.Ram[input.State.StackPointer] = (byte) (pc & 0x00FF);

        input.State.ProgramCounter = pageZeroAddress;

        // Flags unaffected

        return -1;
    }

    public static int OUT_addr_n_R(Input input, Register register)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return 0;
    }

    public static int EXX(Input input)
    {
        var bc = input.State.Registers.ReadPair(Register.BC);

        var de = input.State.Registers.ReadPair(Register.DE);

        var hl = input.State.Registers.ReadPair(Register.HL);

        input.State.Registers.WritePair(Register.BC, input.State.Registers.ReadPair(Register.BC_));

        input.State.Registers.WritePair(Register.DE, input.State.Registers.ReadPair(Register.DE_));

        input.State.Registers.WritePair(Register.HL, input.State.Registers.ReadPair(Register.HL_));

        input.State.Registers.WritePair(Register.BC_, bc);

        input.State.Registers.WritePair(Register.DE_, de);

        input.State.Registers.WritePair(Register.HL_, hl);

        // Flags unaffected

        return 0;
    }

    public static int EX_addr_SP_RR(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WriteLow(register, input.Ram[input.State.StackPointer + 1]);

        input.State.Registers.WriteHigh(register, input.Ram[input.State.StackPointer]);

        input.Ram[input.State.StackPointer] = (byte) (value & 0x00FF);

        input.Ram[input.State.StackPointer + 1] = (byte) ((value & 0xFF00) >> 8);

        // Flags unaffected

        return 0;
    }

    public static int EX_RR_RR(Input input, Register left, Register right)
    {
        var swap = input.State.Registers.ReadPair(left);

        input.State.Registers.WritePair(left, input.State.Registers.ReadPair(right));

        input.State.Registers.WritePair(right, swap);
        
        // Flags unaffected

        return 0;
    }

    public static int DI(Input input)
    {
        input.State.InterruptFlipFlop1 = false;
        
        input.State.InterruptFlipFlop2 = false;
        
        return 0;
    }

    public static int EI(Input input)
    {
        input.State.InterruptFlipFlop1 = true;
        
        input.State.InterruptFlipFlop2 = true;

        return 0;
    }

    public static int CP_R_n(Input input, Register destination)
    {
        unchecked
        {
            var result = input.State.Registers[destination] - input.Data[1];

            // Flags
            input.State.Flags.Carry = input.Data[1] > input.State.Registers[destination];
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = false; // TODO
            input.State.Flags.X1 = (result & 0x08) > 0;
            input.State.Flags.HalfCarry = false;
            input.State.Flags.X2 = (result & 0x20) > 0;
            input.State.Flags.Zero = result == 0;
            input.State.Flags.Sign = (sbyte) result < 0;

            input.State.Registers[Register.F] = input.State.Flags.ToByte();
        }

        return 0;
    }

    public static int CP_R_RRh(Input input, Register left, Register right)
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

        return 0;
    }

    public static int CP_R_RRl(Input input, Register left, Register right)
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

        return 0;
    }

    public static int CP_R_addr_RR_plus_d(Input input, Register left, Register right)
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

        return 0;
    }

    public static int IM_m(Input input, InterruptMode mode)
    {
        input.State.InterruptMode = mode;

        // Flags unaffected

        return 0;
    }

    public static int IN_b_R_addr_n(Input input, Register register)
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

        return 0;
    }

    public static int OUT_b_addr_n_R(Input input, Register register)
    {
        input.Ports.WriteByte(input.Data[1], input.State.Registers[register]);

        // Flags unaffected

        return 0;
    }

    public static int TST_R(Input input, Register register)
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

        return 0;
    }

    public static int TST_addr_R(Input input, Register register)
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

        return 0;
    }

    public static int IN_R_p(Input input, Register register)
    {
        var value = input.Ports.ReadByte(input.Data[1]);

        if (value != 255)
        {
            Debugger.Log(0, "INFO", $"IN_R_p Checking {input.Data[1]:X2} Value: {value}\n");
        }

        input.State.Registers[register] = value;

        return 0;
    }

    public static int IN_R_C(Input input, Register register)
    {
        var value = input.Ports.ReadByte(input.State.Registers[Register.C]);

        if (value != 255)
        {
            Debugger.Log(0, "INFO", $"IN_R_C Checking {input.State.Registers[Register.C]:X2} Value: {value}\n");
        }

        input.State.Registers[register] = value;

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

        return 0;
    }

    public static int OUT_addr_RR_R(Input input, Register destination, Register source)
    {
        // Flags unaffected

        return 0;
    }

    public static int IN_addr_RR(Input input, Register source)
    {
        var value = input.Ports.ReadByte(input.Ram[input.State.Registers.ReadPair(source)]);
        
        if (value != 255)
        {
            Debugger.Log(0, "INFO", $"IN_R_C Checking {input.State.Registers[Register.C]:X2} Value: {value}\n");
        }

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

        return 0;
    }

    public static int OUT_addr_R_n(Input input, Register register, byte data)
    {
        // TODO: Hmm. Might have to get into buses and stuff for this one... bugger.

        // Flags unaffected

        return 0;
    }

    public static int LDI(Input input)
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
        input.State.Flags.X2 = (value & 0x02) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }

    public static int LDIR(Input input)
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

            return -1;
        }

        return 0;
    }

    public static int CPI(Input input)
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

        return 0;
    }

    public static int CPIR(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        var difference = input.State.Registers[Register.A] - value;

        input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

        input.State.Registers.WritePair(Register.BC, (ushort) (input.State.Registers.ReadPair(Register.BC) - 1));

        // Flags
        // Carry unaffected
        //input.State.Flags.Carry = value > input.State.Registers[Register.A];
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

            return -1;
        }

        return 0;
    }

    public static int INI(Input input)
    {
        unchecked
        {
            var address = input.State.Registers[Register.C];

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

            return 0;
        }
    }

    public static int INIR(Input input)
    {
        unchecked
        {
            var address = input.State.Registers[Register.C];

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

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter--;

                return -1;
            }

            return 0;
        }
    }

    public static int OUTI(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = (data & 0x80) > 0;
            input.State.Flags.ParityOverflow = input.State.Registers[Register.B].IsEvenParity();
            input.State.Flags.X1 = (input.State.Registers[Register.B] & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (input.State.Registers[Register.B] & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            return 0;
        }
    }

    public static int OTIR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.Registers[Register.B]--;

            // Flags
            // Carry unaffected
            input.State.Flags.AddSubtract = false;
            input.State.Flags.ParityOverflow = data.IsEvenParity();
            input.State.Flags.X1 = (data & 0x08) > 0;
            // Half carry unknown
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = true;
            // Sign unknown
            
            input.State.Registers[Register.F] = input.State.Flags.ToByte();

            // TODO: Correctly account for extra cycles?

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter--;

                return -1;
            }

            return 0;
        }
    }

    public static int LDD(Input input)
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
        input.State.Flags.X2 = (value & 0x02) > 0;
        // Zero unaffected
        // Sign unaffected

        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }

    public static int LDDR(Input input)
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

            return -1;
        }

        return 0;
    }

    public static int CPD(Input input)
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

        return 0;
    }

    public static int CPDR(Input input)
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

            return -1;
        }

        return 0;
    }

    public static int IND(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

            var value = input.Ports.ReadByte(port);

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

            return 0;
        }
    }

    public static int INDR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

            var value = input.Ports.ReadByte(port);

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

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter--;

                return -1;
            }

            return 0;
        }
    }

    public static int OUTD(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

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

            return 0;
        }
    }

    public static int OTDR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers[Register.C];

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

                return -1;
            }

            return 0;
        }
    }

    public static int RRD(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        var al = (byte) (input.State.Registers[Register.A] & 0x0F);

        var ah = (byte) (input.State.Registers[Register.A] & 0xF0);

        var vl = (byte) (value & 0x0F);

        var vh = (byte) (value & 0xF0);

        input.State.Registers[Register.A] = (byte) (ah | vl);

        value = (byte) ((al << 4) | (vh >> 4));

        input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers[Register.A].IsEvenParity();
        input.State.Flags.X1 = (input.State.Registers[Register.A] & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (input.State.Registers[Register.A] & 0x20) > 0;
        input.State.Flags.Zero = input.State.Registers[Register.A] == 0;
        input.State.Flags.Sign = (sbyte) input.State.Registers[Register.A] < 0;
            
        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }

    public static int RLD(Input input)
    {
        var value = input.Ram[input.State.Registers.ReadPair(Register.HL)];

        var al = (byte) (input.State.Registers[Register.A] & 0x0F);

        var ah = (byte) (input.State.Registers[Register.A] & 0xF0);

        var vl = (byte) (value & 0x0F);

        var vh = (byte) (value & 0xF0);

        input.State.Registers[Register.A] = (byte) (ah | vh >> 4);

        value = (byte) ((vl << 4) | al);

        input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

        // Flags
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = input.State.Registers[Register.A].IsEvenParity();
        input.State.Flags.X1 = (input.State.Registers[Register.A] & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (input.State.Registers[Register.A] & 0x20) > 0;
        input.State.Flags.Zero = input.State.Registers[Register.A] == 0;
        input.State.Flags.Sign = (sbyte) input.State.Registers[Register.A] < 0;
            
        input.State.Registers[Register.F] = input.State.Flags.ToByte();

        return 0;
    }
}