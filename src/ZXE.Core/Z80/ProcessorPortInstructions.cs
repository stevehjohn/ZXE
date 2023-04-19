using ZXE.Core.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Core.Z80;

public static class ProcessorPortInstructions
{
    public static int IN_addr_RR(Input input, Register source)
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

        input.State.PutFlagsInFRegister();

        return 0;
    }

    public static int IN_b_R_addr_n(Input input, Register register)
    {
        // TODO: Not sure this is correct.
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

        input.State.PutFlagsInFRegister();

        return 0;
    }

    public static int IN_R_C(Input input, Register register)
    {
        var value = input.Ports.ReadByte(input.State.Registers.ReadPair(Register.BC));

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

        input.State.PutFlagsInFRegister();

        input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

        return 0;
    }

    public static int IN_R_p(Input input, Register register)
    {
        var value = input.Ports.ReadByte((ushort) (input.State.Registers[register] << 8 | input.Data[1]));

        input.State.MemPtr = (ushort) ((input.State.Registers[register] << 8 | input.Data[1]) + 1);

        input.State.Registers[register] = value;
                
        // Flags unaffected

        input.State.ResetQ();

        return 0;
    }

    public static int OUT_addr_n_R(Input input, Register register)
    {
        input.Ports.WriteByte((ushort) (input.Data[1] & (input.State.Registers[Register.A] << 8)), input.State.Registers[Register.A]);

        // Flags unaffected
                
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (((input.Data[1] + 1) & 0xFF) | (input.State.Registers[register] << 8));

        return 0;
    }

    public static int OUT_addr_RR_R(Input input, Register destination, Register source)
    {
        input.Ports.WriteByte(input.State.Registers.ReadPair(destination), input.State.Registers[source]);

        // Flags unaffected
                
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (((input.Ram[input.State.Registers.ReadPair(destination)] + 1) & 0xFF) | (input.State.Registers[source] << 8));

        return 0;
    }

    public static int OUT_addr_RR_n(Input input, Register register, byte data)
    {
        input.Ports.WriteByte(input.State.Registers.ReadPair(register), data);

        // Flags unaffected
                
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

        return 0;
    }

    public static int OUT_b_addr_n_R(Input input, Register register)
    {
        input.Ports.WriteByte(input.Data[1], input.State.Registers[register]);

        // Flags unaffected
                
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (((input.Data[1] + 1) & 0xFF) | (input.State.Registers[register] << 8));

        return 0;
    }

    public static int INI(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

            input.State.Registers[Register.B]--;

            // Flags
            input.State.Flags.Carry = value + ((input.State.Registers[Register.C] + 1) & 0xFF) > 0xFF;
            input.State.Flags.AddSubtract = (value & 0x80) > 0;
            input.State.Flags.ParityOverflow = ((ushort) (((value + ((input.State.Registers[Register.C] + 1) & 0xFF)) & 0x07) ^ input.State.Registers[Register.B])).IsEvenParity();
            input.State.Flags.X1 = (input.State.Registers[Register.B] & 0x08) > 0;
            input.State.Flags.HalfCarry = value + ((input.State.Registers[Register.C] + 1) & 0xFF) > 0xFF;
            input.State.Flags.X2 = (input.State.Registers[Register.B] & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();

            return 0;
        }
    }

    public static int INIR(Input input)
    {
        unchecked
        {
            var address = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(address);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) + 1));

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();
            
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

            // TODO: Correctly account for extra cycles?
            input.State.Registers[Register.B]--;

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter -= 2;

                return 5;
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
            
            input.State.PutFlagsInFRegister();

            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

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
            
            input.State.PutFlagsInFRegister();
            
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) + 1);

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter -= 2;

                return 5;
            }

            return 0;
        }
    }

    public static int IND(Input input)
    {
        unchecked
        {
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) - 1);

            input.State.Registers[Register.B]--;

            var port = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(port);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();

            return 0;
        }
    }

    public static int INDR(Input input)
    {
        unchecked
        {
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) - 1);

            input.State.Registers[Register.B]--;

            var port = input.State.Registers.ReadPair(Register.BC);

            var value = input.Ports.ReadByte(port);

            input.Ram[input.State.Registers.ReadPair(Register.HL)] = value;

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            // Flags
            input.State.Flags.Carry = value > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (value & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (value & 0x0F);
            input.State.Flags.X2 = (value & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter -= 2;

                return 5;
            }

            return 0;
        }
    }

    public static int OUTD(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.State.Registers[Register.B]--;

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();
            
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) - 1);

            return 0;
        }
    }

    public static int OTDR(Input input)
    {
        unchecked
        {
            var port = input.State.Registers.ReadPair(Register.BC);

            var address = input.State.Registers.ReadPair(Register.HL);

            var data = input.Ram[address];

            input.State.Registers[Register.B]--;

            input.Ports.WriteByte(port, data);

            input.State.Registers.WritePair(Register.HL, (ushort) (input.State.Registers.ReadPair(Register.HL) - 1));

            // Flags
            input.State.Flags.Carry = data > input.State.Registers[Register.A];
            input.State.Flags.AddSubtract = true;
            input.State.Flags.ParityOverflow = input.State.Registers.ReadPair(Register.BC) != 0;
            input.State.Flags.X1 = (data & 0x08) > 0;
            input.State.Flags.HalfCarry = (input.State.Registers[Register.A] & 0x0F) < (data & 0x0F);
            input.State.Flags.X2 = (data & 0x20) > 0;
            input.State.Flags.Zero = input.State.Registers[Register.B] == 0;
            input.State.Flags.Sign = (sbyte) input.State.Registers[Register.B] < 0;
            
            input.State.PutFlagsInFRegister();
            
            input.State.MemPtr = (ushort) (input.State.Registers.ReadPair(Register.BC) - 1);

            if (input.State.Registers[Register.B] != 0)
            {
                input.State.ProgramCounter -= 2;

                return 5;
            }

            return 0;
        }
    }
}