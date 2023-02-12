namespace ZXE.Core.Z80;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

public partial class Processor
{
    private static bool DJNZ_e(Input input)
    {
        unchecked
        {
            // TODO: If B != 0, 5 more cycles... how to do this?
            ProcessorArithmeticInstructions.DEC_R(input, Register.B);

            if (!input.State.Flags.Zero)
            {
                input.State.ProgramCounter += (sbyte)input.Data[1];
            }

            // Flags unaffected
        }

        return true;
    }

    public static bool JR_e(Input input)
    {
        unchecked
        {
            input.State.ProgramCounter += (sbyte) input.Data[1];

            input.State.ProgramCounter = (ushort) input.State.ProgramCounter;
        }

        return true;
    }

    private static bool JR_NZ_e(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JR_Z_e(Input input)
    {
        if (input.State.Flags.Zero)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JR_NC_e(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JR_C_e(Input input)
    {
        if (input.State.Flags.Carry)
        {
            JR_e(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET_NZ(Input input)
    {
        // TODO: If condition true, 6 more cycles required.
        if (! input.State.Flags.Zero)
        {
            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_NZ_nn(Input input)
    {
        if (! input.State.Flags.Zero)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_nn(Input input)
    {
        // TODO: Don't like this - 3 thing... maybe return true/false to indicate whether PC should be adjusted by caller...
        input.State.ProgramCounter = (input.Data[2] << 8 | input.Data[1]) - 3;

        // Flags unaffected

        return true;
    }

    private static bool CALL_NZ_nn(Input input)
    {
        // TODO: If condition true, 7 more cycles required.
        if (! input.State.Flags.Zero)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_nn(Input input)
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
        }

        return true;
    }

    private static bool RET_Z(Input input)
    {
        if (input.State.Flags.Zero)
        {
            // TODO: Same old... more cycles if condition met.

            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET(Input input)
    {
        var spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0xFF00) | spContent;

        input.State.StackPointer++;

        spContent = input.Ram[input.State.StackPointer];

        input.State.ProgramCounter = (input.State.ProgramCounter & 0x00FF) | spContent << 8;

        input.State.StackPointer++;

        input.State.ProgramCounter--;

        // Flags unaffected

        return true;
    }

    private static bool JP_Z_nn(Input input)
    {
        if (input.State.Flags.Zero)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_Z_nn(Input input)
    {
        // TODO: If condition true, 7 more cycles required.
        if (input.State.Flags.Zero)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET_NC(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_NC_nn(Input input)
    {
        if (! input.State.Flags.Carry)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET_C(Input input)
    {
        if (input.State.Flags.Carry)
        {
            // TODO: Same old... more cycles if condition met.

            return RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            return JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_C_nn(Input input)
    {
        if (input.State.Flags.Carry)
        {
            return CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET_PO(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_PO_nn(Input input)
    {
        if (! input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool RET_PE(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            return RET(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool JP_addr_RR(Input input, Register register)
    {
        input.State.ProgramCounter = input.State.Registers.ReadPair(register);
        
        // Flags unaffected

        return false;
    }

    private static bool JP_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            return JP_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool CALL_PE_nn(Input input)
    {
        if (input.State.Flags.ParityOverflow)
        {
            CALL_nn(input);
        }
        
        // Flags unaffected

        return true;
    }

    private static bool RET_NS(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_NS_nn(Input input)
    {
        if (! input.State.Flags.Sign)
        {
            CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool RET_S(Input input)
    {
        if (input.State.Flags.Sign)
        {
            RET(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool JP_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            JP_nn(input);
        }

        // Flags unaffected

        return true;
    }

    private static bool CALL_S_nn(Input input)
    {
        if (input.State.Flags.Sign)
        {
            CALL_nn(input);
        }

        // Flags unaffected

        return true;
    }
}