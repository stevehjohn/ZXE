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

        _instructions = new Instruction[instructions.Max(i => i.Key)];

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

        instructions[0x000001] = new Instruction("LD BC, nn", 3, (i, s, r) => { });
    }
}