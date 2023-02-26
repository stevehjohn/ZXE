﻿using ZXE.Common.ConsoleHelpers;
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

        SetProcessorState(processor, input.ProcessorState);

        var ram = new Ram(Model.Spectrum48K);

        PopulateRam(ram, input);

        var ports = new Ports();

        var bus = new Bus();

        var tStates = 0;

        while (tStates < input.ProcessorState.TStates)
        {
            tStates += processor.ProcessInstruction(ram, ports, bus);
        }

        var expectedResult = LoadExpectedResult(input.Name);

        OutputResult(processor, expectedResult);
    }

    private static void OutputResult(Processor processor, TestExpectedResult expectedResult)
    {
        if (processor.State.Registers.ReadPair(Register.AF) == expectedResult.ProcessorState.AF
            && processor.State.Registers.ReadPair(Register.BC) == expectedResult.ProcessorState.BC
            && processor.State.Registers.ReadPair(Register.DE) == expectedResult.ProcessorState.DE
            && processor.State.Registers.ReadPair(Register.HL) == expectedResult.ProcessorState.HL)
        {
            return;
        }

        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.WriteLine("        &Cyan;Expected    Actual");

        FormattedConsole.WriteLine($"    &Cyan;AF&White;: &Green;0x{expectedResult.ProcessorState.AF:X4}      &Cyan;AF&White;: {(expectedResult.ProcessorState.AF == processor.State.Registers.ReadPair(Register.AF) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.AF):X4}");
        FormattedConsole.WriteLine($"    &Cyan;BC&White;: &Green;0x{expectedResult.ProcessorState.BC:X4}      &Cyan;BC&White;: {(expectedResult.ProcessorState.BC == processor.State.Registers.ReadPair(Register.BC) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.BC):X4}");
        FormattedConsole.WriteLine($"    &Cyan;DE&White;: &Green;0x{expectedResult.ProcessorState.DE:X4}      &Cyan;DE&White;: {(expectedResult.ProcessorState.DE == processor.State.Registers.ReadPair(Register.DE) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.DE):X4}");
        FormattedConsole.WriteLine($"    &Cyan;HL&White;: &Green;0x{expectedResult.ProcessorState.HL:X4}      &Cyan;HL&White;: {(expectedResult.ProcessorState.HL == processor.State.Registers.ReadPair(Register.HL) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.HL):X4}");
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

    private static void SetProcessorState(Processor processor, ProcessorState processorState)
    {
        processor.State.Registers.WritePair(Register.AF, processorState.AF);

        processor.State.Registers.WritePair(Register.BC, processorState.BC);

        processor.State.Registers.WritePair(Register.DE, processorState.DE);

        processor.State.Registers.WritePair(Register.HL, processorState.HL);

        processor.State.Registers.WritePair(Register.AF_, processorState.AF_);

        processor.State.Registers.WritePair(Register.BC_, processorState.BC_);

        processor.State.Registers.WritePair(Register.DE_, processorState.DE_);

        processor.State.Registers.WritePair(Register.HL_, processorState.HL_);

        processor.State.Registers.WritePair(Register.IX, processorState.IX);

        processor.State.Registers.WritePair(Register.IY, processorState.IY);

        processor.State.ProgramCounter = processorState.PC;

        processor.State.StackPointer = processorState.SP;

        processor.State.Registers[Register.I] = processorState.I;

        processor.State.Registers[Register.R] = processorState.R;

        processor.State.InterruptFlipFlop1 = processorState.IFF1;

        processor.State.InterruptFlipFlop2 = processorState.IFF2;

        processor.State.InterruptMode = (InterruptMode) processorState.InterruptMode;

        processor.State.Halted = processorState.Halted;
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