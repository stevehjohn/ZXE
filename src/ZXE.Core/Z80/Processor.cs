using ZXE.Core.System;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;

public class Processor
{
    private readonly Instruction[] _instructions;

    public Processor()
    {
        var instructions = new Dictionary<int, Instruction>();

        InitialiseInstructions(instructions);

        _instructions = new Instruction[instructions.Max(i => i.Key) + 1];

        foreach (var instruction in instructions)
        {
            _instructions[instruction.Key] = instruction.Value;
        }
    }

    public void ProcessInstruction(Ram ram, State state)
    {
        var instruction = _instructions[ram[state.ProgramCounter]];

        var data = ram[state.ProgramCounter..(state.ProgramCounter + instruction.Length)];

        instruction.Action(new Input(data, state, ram));

        state.ProgramCounter += instruction.Length;
    }

    private static void InitialiseInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0x00] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => LD_rr_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => LD_addr_rr_A(i, Register.BC), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_rr(i, Register.BC), 6);
        
        instructions[0x04] = new Instruction("INC B", 1, i => INC_r(i, Register.B), 4);
        
        instructions[0x05] = new Instruction("DEC B", 1, i => DEC_r(i, Register.B), 4);
        
        instructions[0x06] = new Instruction("LD B, n", 2, i => LD_r_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, RLCA, 4);

        instructions[0x76] = new Instruction("HALT", 1, HALT, 4);
    }

    private static void NOP()
    {
    }

    private static void LD_rr_nn(Input input, Register register)
    {
        input.State.Registers.LoadFromRam(register, input.Data[1..3]);
    }

    private static void LD_addr_rr_A(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.State.Registers[Register.A];
    }

    private static void INC_rr(Input input, Register register)
    {
        input.State.Registers.WritePair(register, (ushort) (input.State.Registers.ReadPair(register) + 1));
    }

    private static void INC_r(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value + 1);

        input.State.Registers[register] = result;

        // FLAGS
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value == 0x7F;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }

    private static void DEC_r(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value - 1);

        input.State.Registers[register] = result;

        // FLAGS
        // Carry unaffected
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = value == 0x80;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) < 1;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }

    private static void LD_r_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];
    }

    private static void RLCA(Input input)
    {
        var topBit = (byte) ((input.State.Registers[Register.A] & 0x80) >> 7);

        var result = (byte) (((input.State.Registers[Register.A] << 1) & 0xFE) | topBit);

        input.State.Registers[Register.A] = result;

        // FLAGS
        input.State.Flags.Carry = topBit == 1;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected
    }

    private static void HALT(Input input)
    {
        input.State.Halted = true;
    }
}