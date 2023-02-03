using ZXE.Core.Infrastructure;
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

        var operationData = ram[state.ProgramCounter..(state.ProgramCounter + instruction.Length)];

        instruction.Action(operationData, state, ram);
    }

    private static void InitialiseInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0x000000] = new Instruction("NOP", 1, (_, _, _) => Thread.Sleep(0));

        // TODO: REALLY, REALLY, REALLY, verify the byte order of these instructions before continuing much further.
        instructions[0x000001] = new Instruction("LD BC, nn", 3, (i, s, _) =>
        {
            s.Registers[Register.B] = i[2];
            s.Registers[Register.C] = i[1];
        });

        instructions[0x000002] = new Instruction("LD (BC), A", 1, (_, s, r) =>
        {
            var address = s.Registers[Register.B] << 8 | s.Registers[Register.C];

            r[address] = s.Registers[Register.A];
        });

        instructions[0x000003] = new Instruction("INC BC", 1, (_, s, _) =>
        {
            s.Registers[Register.C]++;

            if (s.Registers[Register.C] == 0)
            {
                s.Registers[Register.B]++;
            }
        });
    }
}