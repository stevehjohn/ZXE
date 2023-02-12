namespace ZXE.Core.Z80;

public static class ProcessorLoadInstructions
{
    public static bool LD_RR_nn(Input input, Register register)
    {
        input.State.Registers.LoadFromRam(register, input.Data[1..3]);

        // Flags unaffected

        return true;
    }

    public static bool LD_addr_RR_R(Input input, Register target, Register source)
    {
        input.Ram[input.State.Registers.ReadPair(target)] = input.State.Registers[source];

        // Flags unaffected

        return true;
    }    

    public static bool LD_R_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];

        // Flags unaffected

        return true;
    }

    public static bool LD_addr_nn_R(Input input, Register register)
    {
        input.Ram[(input.Data[2] << 8) | input.Data[1]] = input.State.Registers[register];

        // Flags unaffected

        return true;
    }

    public static bool LD_R_addr_nn(Input input, Register register)
    {
        input.State.Registers[register] = input.Ram[(input.Data[2] << 8) | input.Data[1]];

        // Flags unaffected

        return true;
    }

    public static bool LD_R_addr_RR(Input input, Register target, Register source)
    {
        input.State.Registers[target] = input.Ram[input.State.Registers.ReadPair(source)];

        // Flags unaffected

        return true;
    }

    public static bool LD_addr_nn_RR(Input input, Register register)
    {
        unchecked
        {
            var address = input.Data[2] << 8 | input.Data[1];

            var data = input.State.Registers.ReadPair(register);

            input.Ram[address] = (byte) (data & 0x00FF);
            input.Ram[address + 1] = (byte) ((data & 0xFF00) >> 8);

            // Flags unaffected
        }

        return true;
    }

    public static bool LD_RR_addr_nn(Input input, Register register)
    {
        unchecked
        {
            var address = input.Data[2] << 8 | input.Data[1];

            input.State.Registers.WritePair(register, (ushort) (input.Ram[address + 1] << 8 | input.Ram[address]));

            // Flags unaffected
        }

        return true;
    }

    public static bool LD_SP_nn(Input input)
    {
        input.State.StackPointer = input.Data[2] << 8 | input.Data[1];

        // Flags unaffected

        return true;
    }

    public static bool LD_addr_RR_n(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.Data[1];

        // Flags unaffected

        return true;
    }

    public static bool LD_R_R(Input input, Register destination, Register source)
    {
        input.State.Registers[destination] = input.State.Registers[source];

        // Flags unaffected
        // TODO: Flags might be affected...

        return true;
    }

    public static bool LD_RR_RR(Input input)
    {
        input.State.StackPointer = input.State.Registers.ReadPair(Register.HL);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRh_n(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WritePair(register, (ushort) ((input.Data[1] << 8) | (value & 0x00FF)));
        
        // Flags unaffected

        return true;
    }

    public static bool LD_RRl_n(Input input, Register register)
    {
        var value = input.State.Registers.ReadPair(register);

        input.State.Registers.WritePair(register, (ushort) (input.Data[1] | (value & 0xFF00)));
        
        // Flags unaffected

        return true;
    }

    public static bool LD_addr_RR_plus_d_n(Input input, Register register)
    {
        var address = (int) input.State.Registers.ReadPair(register);

        address += (sbyte) input.Data[1];

        input.Ram[address] = input.Data[2];

        // Flags unaffected

        return true;
    }

    public static bool LD_R_RRh(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(source);

        input.State.Registers[destination] = (byte) ((value & 0xFF00) >> 8);
        
        // Flags unaffected

        return true;
    }

    public static bool LD_R_RRl(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(source);

        input.State.Registers[destination] = (byte) (value & 0x00FF);
        
        // Flags unaffected

        return true;
    }

    public static bool LD_R_addr_RR_plus_d(Input input, Register destination, Register source)
    {
        var address = (int) input.State.Registers.ReadPair(source);

        address += (sbyte) input.Data[1];

        input.State.Registers[destination] = input.Ram[address];

        // Flags unaffected
        return true;
    }

    public static bool LD_addr_RR_plus_d_R(Input input, Register destination, Register source)
    {
        var address = (int) input.State.Registers.ReadPair(destination);

        address += (sbyte) input.Data[1];

        input.Ram[address] = input.State.Registers[source];

        // Flags unaffected

        return true;
    }

    public static bool LD_RRh_R(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(destination);

        value = (ushort) ((value & 0x00FF) | (input.State.Registers[source] << 8));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRh_RRh(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0x00FF) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRh_RRl(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0x00FF) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRl_R(Input input, Register destination, Register source)
    {
        var value = input.State.Registers.ReadPair(destination);

        value = (ushort) ((value & 0xFF00) | input.State.Registers[source]);

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRl_RRh(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);

        var value = (ushort) ((left & 0xFF00) | (right & 0xFF00));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_RRl_RRl(Input input, Register destination, Register source)
    {
        var left = input.State.Registers.ReadPair(destination);

        var right = input.State.Registers.ReadPair(source);
        
        var value = (ushort) ((left & 0xFF00) | (right & 0x00FF));

        input.State.Registers.WritePair(destination, value);

        // Flags unaffected

        return true;
    }

    public static bool LD_SP_RR(Input input, Register register)
    {
        input.State.StackPointer = input.State.Registers.ReadPair(register);

        // Flags unaffected

        return true;
    }

    public static bool LD_addr_nn_SP(Input input)
    {
        unchecked
        {
            var address = input.Data[2] << 8 | input.Data[1];

            var data = input.State.StackPointer;

            input.Ram[address] = (byte) (data & 0x00FF);
            input.Ram[address + 1] = (byte) ((data & 0xFF00) >> 8);

            // Flags unaffected
        }

        return true;
    }
}