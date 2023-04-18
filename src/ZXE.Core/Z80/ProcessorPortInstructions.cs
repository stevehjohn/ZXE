using ZXE.Core.Extensions;

namespace ZXE.Core.Z80;

public static class ProcessorPortInstructions
{
    public static int IN_addr_RR(Input input, Register source)
    {
        //var value = input.Ports.ReadByte(input.Ram[input.State.Registers.ReadPair(source)]);
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

        return 0;
    }
}