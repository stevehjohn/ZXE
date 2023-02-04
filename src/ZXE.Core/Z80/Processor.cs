using ZXE.Core.System;

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
        // TODO: Account for cycles...
        instructions[0x00] = new Instruction("NOP", 1, _ => Thread.Sleep(0), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => LD_rr_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 3, i => LD_addr_rr_A(i, Register.BC), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_rr(i, Register.BC), 6);
        
        instructions[0x04] = new Instruction("INC B", 1, i => INC_r(i, Register.B), 4);

        //instructions[0x000002] = new Instruction("LD (BC), A", 1, (_, s, r) =>
        //{
        //    var address = s.Registers[Register.B] << 8 | s.Registers[Register.C];

        //    r[address] = s.Registers[Register.A];
        //});

        //instructions[0x000003] = new Instruction("INC BC", 1, (_, s, _) =>
        //{
        //    s.Registers[Register.C]++;

        //    if (s.Registers[Register.C] == 0)
        //    {
        //        s.Registers[Register.B]++;
        //    }
        //});

        //instructions[0x0000C0] = new Instruction("RET NZ", 1, (_, s, _) =>
        //{
        //});
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
        input.State.Registers[register] = (byte) (input.State.Registers[register] + 1);
    }
}