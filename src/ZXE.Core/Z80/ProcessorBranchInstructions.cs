namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public static class ProcessorBranchInstructions
{
    public static int DJNZ_e(Input input)
    {
        unchecked
        {
            var value = input.State.Registers[Register.B];

            var result = (byte) (value - 1);

            input.State.Registers[Register.B] = result;

            // Flags unaffected
            
            input.State.ResetQ();

            if (result != 0)
            {
                input.State.ProgramCounter += (sbyte) input.Data[1];

                input.State.MemPtr = (ushort) input.State.ProgramCounter;

                return 5;
            }
        }

        return 0;
    }

    public static int JR_e(Input input)
    {
        unchecked
        {
            input.State.ProgramCounter += (sbyte) input.Data[1];

            input.State.ProgramCounter = (ushort) input.State.ProgramCounter;

            input.State.MemPtr = (ushort) input.State.ProgramCounter;
        }
        

        // Flags unaffected

        input.State.ResetQ();

        return 0;
    }

    public static int JR_NZ_e(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            JR_e(input);

            return 5;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JR_Z_e(Input input)
    {
        if (input.State.Flags.Zero)
        {
            JR_e(input);

            return 5;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JR_NC_e(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            JR_e(input);

            return 5;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JR_C_e(Input input)
    {
        if (input.State.Flags.Carry)
        {
            JR_e(input);

            return 5;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int RETI(Input input)
    {
        var value = (ushort) input.Ram[input.State.StackPointer];

        input.State.StackPointer++;

        value |= (ushort) (input.Ram[input.State.StackPointer] << 8);

        input.State.StackPointer++;

        input.State.ProgramCounter = value;

        input.State.MemPtr = (ushort) input.State.ProgramCounter;
        
        // Flags unaffected

        input.State.ResetQ();

        input.State.InterruptType = InterruptType.None;

        return -1;
    }

    public static int RETN(Input input)
    {
        var value = (ushort) input.Ram[input.State.StackPointer];

        input.State.StackPointer++;

        value |= (ushort) (input.Ram[input.State.StackPointer] << 8);

        input.State.StackPointer++;

        input.State.ProgramCounter = value;

        input.State.InterruptFlipFlop1 = input.State.InterruptFlipFlop2;
        
        // Flags unaffected

        input.State.ResetQ();

        input.State.InterruptType = InterruptType.None;

        return -1;
    }

    public static int RET_NZ(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_NZ_nn(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int JP_nn(Input input)
    {
        input.State.ProgramCounter = input.Data[2] << 8 | input.Data[1];

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return -1;
    }

    public static int CALL_NZ_nn(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_nn(Input input)
    {
        unchecked
        {
            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) (((input.State.ProgramCounter + 3) & 0xFF00) >> 8);

            input.State.StackPointer--;

            input.Ram[input.State.StackPointer] = (byte) ((input.State.ProgramCounter + 3) & 0x00FF);

            // TODO: Remove -3 and return false
            input.State.ProgramCounter = (input.Data[2] << 8 | input.Data[1]) - 3;

            // Flags unaffected
            
            input.State.ResetQ();

            input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);
        }

        return 0;
    }

    public static int RET_Z(Input input)
    {
        if (input.State.Flags.Zero)
        {
            // TODO: Same old... more cycles if condition met.
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int RET(Input input)
    {
        var spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0xFF00) | spContent;

        input.State.StackPointer++;

        spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0x00FF) | spContent << 8;

        input.State.StackPointer++;

        input.State.ProgramCounter--;

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) input.State.ProgramCounter;

        input.State.InterruptType = InterruptType.None;

        return 0;
    }

    public static int JP_Z_nn(Input input)
    {
        if (input.State.Flags.Zero)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_Z_nn(Input input)
    {
        if (input.State.Flags.Zero)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_NC(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_C(Input input)
    {
        if (input.State.Flags.Carry)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_PO(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_PE(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_addr_RR(Input input, Register register)
    {
        input.State.ProgramCounter = input.State.Registers.ReadPair(register);

        // Flags unaffected
        
        input.State.ResetQ();

        return -1;
    }

    public static int JP_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_NS(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int RET_S(Input input)
    {
        if (input.State.Flags.Sign)
        {
            RET(input);

            return 6;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        return 0;
    }

    public static int JP_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            return JP_nn(input);
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }

    public static int CALL_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            CALL_nn(input);

            return 7;
        }

        // Flags unaffected
        
        input.State.ResetQ();

        input.State.MemPtr = (ushort) (input.Data[2] << 8 | input.Data[1]);

        return 0;
    }
}