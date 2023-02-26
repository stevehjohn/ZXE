using ZXE.Common.ConsoleHelpers;
using ZXE.Core.FuseTests.Exceptions;
using ZXE.Core.FuseTests.Models;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.FuseTests.Infrastructure;

public static class TestRunner
{
    public static void RunTests()
    {
        var input = File.ReadAllLines("TestDefinitions\\input.fuse");

        var test = new List<string>();

        for (var i = 0; i < input.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(input[i]))
            {
                if (test.Count > 0)
                {
                    RunTest(new TestInput(test.ToArray()));

                    test.Clear();
                }

                continue;
            }

            test.Add(input[i]);
        }
    }

    private static void RunTest(TestInput input)
    {
        FormattedConsole.WriteLine($"\n  &Cyan;Test&White;: &Yellow;{input.Name}");

        var processor = new Processor();

        SetProcessorState(processor, input);

        var ram = new Ram(Model.Spectrum48K);

        PopulateRam(ram, input);

        var ports = new Ports();

        var bus = new Bus();

        var tStates = 0;

        while (tStates < input.TStates)
        {
            tStates += processor.ProcessInstruction(ram, ports, bus);
        }

        var expectedResult = LoadExpectedResult(input.Name);
    }

    private static TestExpectedResult LoadExpectedResult(string testName)
    {
        var results = File.ReadAllLines("TestDefinitions\\expected.fuse");

        var line = 0;

        int? startLine = null;

        while (line < results.Length)
        {
            if (results[line] == testName)
            {
                startLine = line;
            }

            while (! string.IsNullOrWhiteSpace(results[line]) && line < results.Length)
            {
                line++;
            }

            if (startLine.HasValue)
            {
                return new TestExpectedResult(results[startLine!.Value..line]);
            }

            line++;
        }

        throw new TestExpectedResultNotFoundException($"Could not find test expected result for {testName}.");
    }

    private static void SetProcessorState(Processor processor, TestInput input)
    {
        processor.State.Registers.WritePair(Register.AF, input.AF);

        processor.State.Registers.WritePair(Register.BC, input.BC);

        processor.State.Registers.WritePair(Register.DE, input.DE);

        processor.State.Registers.WritePair(Register.HL, input.HL);

        processor.State.Registers.WritePair(Register.AF1, input.AF_);

        processor.State.Registers.WritePair(Register.BC1, input.BC_);

        processor.State.Registers.WritePair(Register.DE1, input.DE_);

        processor.State.Registers.WritePair(Register.HL1, input.HL_);

        processor.State.Registers.WritePair(Register.IX, input.IX);

        processor.State.Registers.WritePair(Register.IY, input.IY);

        processor.State.ProgramCounter = input.PC;

        processor.State.StackPointer = input.SP;

        processor.State.Registers[Register.I] = input.I;

        processor.State.Registers[Register.R] = input.R;

        processor.State.InterruptFlipFlop1 = input.IFF1;

        processor.State.InterruptFlipFlop2 = input.IFF2;

        processor.State.InterruptMode = (InterruptMode) input.InterruptMode;

        processor.State.Halted = input.Halted;
    }

    private static void PopulateRam(Ram ram, TestInput input)
    {
        for (var i = 0; i < 0xFFFF; i++)
        {
            if (input.Ram[i].HasValue)
            {
                ram[i] = input.Ram[i]!.Value;
            }
        }
    }
}