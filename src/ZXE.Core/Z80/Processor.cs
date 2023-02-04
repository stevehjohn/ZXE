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
        instructions[0x00] = new Instruction("NOP", 1, _ => Thread.Sleep(0), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => LD_rr_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => LD_addr_rr_A(i, Register.BC), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_rr(i, Register.BC), 6);
        
        instructions[0x04] = new Instruction("INC B", 1, i => INC_r(i, Register.B), 4);
        
        instructions[0x05] = new Instruction("DEC B", 1, i => DEC_r(i, Register.B), 4);
        
        instructions[0x06] = new Instruction("LD B, n", 2, i => LD_r_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, RLCA, 4);

        //instructions[0x0000C0] = new Instruction("RET NZ", 1, (_, s, _) =>
        //{
        //});
    }

    // TODO: Deal with undocumented flags?

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

        input.State.SetFlag(Flags.Carry, input.State.Flags[Flags.Carry]);

        input.State.Flags = (byte) (input.State.Flags & Flags.Carry);
        input.State.Flags |= (sbyte) result < 0 ? Flags.Sign : (byte) 0;
        input.State.Flags |= result == 0 ? Flags.Zero : (byte) 0;
        input.State.Flags |= (result & 0x10) > 0 ? Flags.HalfCarry : (byte) 0;
        input.State.Flags |= value == 0x7F ? Flags.ParityOverflow : (byte) 0;

        // FLAGS
    }

    private static void DEC_r(Input input, Register register)
    {
        input.State.Registers[register] = (byte) (input.State.Registers[register] - 1);

        // FLAGS
    }

    private static void LD_r_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];
    }

    private static void RLCA(Input input)
    {
        var topBit = (byte) ((input.State.Registers[Register.A] & 0x80) >> 7);

        input.State.Registers[Register.A] = (byte) (((input.State.Registers[Register.A] << 1) & 0xFE) | topBit);

        input.State.Flags = (byte) (input.State.Flags & (Flags.Sign | Flags.Zero | Flags.ParityOverflow));
        input.State.Flags |= topBit;
    }
}