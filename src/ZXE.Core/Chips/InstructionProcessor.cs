using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Chips;

public class InstructionProcessor
{
    private readonly CpuState _state;
    
    private Instruction[]? _instructions;

    public InstructionProcessor(CpuState state)
    {
        InitialiseInstructions();

        _state = state;
    }

    public void ProcessInstruction()
    {
    }

    private void InitialiseInstructions()
    {
        // TODO: Probably going to need to add a cycle count.
        var instructions = new Dictionary<int, Instruction>
                           {
                               { 0x0000, new Instruction("NOP", 1, (_, _) => Thread.Sleep(0)) },
                               { 0x0001, new Instruction("LD BC, nn", 3, (i, s) => { s.SetRegister(Register.C, i[1]); s.SetRegister(Register.B, i[2]); }) }
                           };

        _instructions = new Instruction[instructions.Max(i => i.Key + 1)];

        foreach (var instruction in instructions)
        {
            _instructions[instruction.Key] = instruction.Value;
        }
    }
}