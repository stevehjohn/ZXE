﻿using System.Diagnostics;
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

        var testCount = 0;

        var passed = 0;

        var stopwatch = Stopwatch.StartNew();

        Console.CursorVisible = false;

        for (var i = 0; i < input.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(input[i]))
            {
                if (test.Count > 0)
                {
                    testCount++;

                    if (RunTest(new TestInput(test.ToArray())))
                    {
                        passed++;
                    }

                    test.Clear();
                }

                continue;
            }

            test.Add(input[i]);
        }

        FormattedConsole.WriteLine($"\n  &Cyan;Testing complete&White;. &Cyan;Elapsed&White;: &Yellow;{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds}.{stopwatch.Elapsed.Milliseconds}\n");

        FormattedConsole.WriteLine($"  &Cyan;Executed&White;: &Yellow;{testCount:N0}    &Cyan;Passed&White;: &Green;{passed:N0}   &Cyan;Failed&White;: &Red;{testCount - passed:N0}");

        FormattedConsole.WriteLine("&Green;");

        Console.CursorVisible = true;
    }

    private static bool RunTest(TestInput input)
    {
        FormattedConsole.Write($"\n  &Cyan;Test&White;: &Yellow;{input.Name}");

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

        return OutputResult(processor, expectedResult);
    }

    private static bool OutputResult(Processor processor, TestExpectedResult expectedResult)
    {
        if (processor.State.Registers.ReadPair(Register.AF) == expectedResult.ProcessorState.AF
            && processor.State.Registers.ReadPair(Register.BC) == expectedResult.ProcessorState.BC
            && processor.State.Registers.ReadPair(Register.DE) == expectedResult.ProcessorState.DE
            && processor.State.Registers.ReadPair(Register.HL) == expectedResult.ProcessorState.HL
            && processor.State.Registers.ReadPair(Register.AF_) == expectedResult.ProcessorState.AF_
            && processor.State.Registers.ReadPair(Register.BC_) == expectedResult.ProcessorState.BC_
            && processor.State.Registers.ReadPair(Register.DE_) == expectedResult.ProcessorState.DE_
            && processor.State.Registers.ReadPair(Register.HL_) == expectedResult.ProcessorState.HL_
            && processor.State.Registers.ReadPair(Register.IX) == expectedResult.ProcessorState.IX
            && processor.State.Registers.ReadPair(Register.IY) == expectedResult.ProcessorState.IY
            && processor.State.ProgramCounter == expectedResult.ProcessorState.PC
            && processor.State.StackPointer == expectedResult.ProcessorState.SP)
        {
            FormattedConsole.WriteLine(" &White;[&Green;PASS&White;]");

            return true;
        }

        FormattedConsole.WriteLine(" &White;[&Red;FAIL&White;]");

        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.WriteLine("         &Cyan;Expected         Actual");

        FormattedConsole.WriteLine($"    &Cyan;AF &White;: &Green;0x{expectedResult.ProcessorState.AF:X4}      &Cyan;AF &White;: {(expectedResult.ProcessorState.AF == processor.State.Registers.ReadPair(Register.AF) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.AF):X4}");
        FormattedConsole.WriteLine($"    &Cyan;BC &White;: &Green;0x{expectedResult.ProcessorState.BC:X4}      &Cyan;BC &White;: {(expectedResult.ProcessorState.BC == processor.State.Registers.ReadPair(Register.BC) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.BC):X4}");
        FormattedConsole.WriteLine($"    &Cyan;DE &White;: &Green;0x{expectedResult.ProcessorState.DE:X4}      &Cyan;DE &White;: {(expectedResult.ProcessorState.DE == processor.State.Registers.ReadPair(Register.DE) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.DE):X4}");
        FormattedConsole.WriteLine($"    &Cyan;HL &White;: &Green;0x{expectedResult.ProcessorState.HL:X4}      &Cyan;HL &White;: {(expectedResult.ProcessorState.HL == processor.State.Registers.ReadPair(Register.HL) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.HL):X4}");
        FormattedConsole.WriteLine($"    &Cyan;AF'&White;: &Green;0x{expectedResult.ProcessorState.AF_:X4}      &Cyan;AF'&White;: {(expectedResult.ProcessorState.AF_ == processor.State.Registers.ReadPair(Register.AF_) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.AF_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;BC'&White;: &Green;0x{expectedResult.ProcessorState.BC_:X4}      &Cyan;BC'&White;: {(expectedResult.ProcessorState.BC_ == processor.State.Registers.ReadPair(Register.BC_) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.BC_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;DE'&White;: &Green;0x{expectedResult.ProcessorState.DE_:X4}      &Cyan;DE'&White;: {(expectedResult.ProcessorState.DE_ == processor.State.Registers.ReadPair(Register.DE_) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.DE_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;HL'&White;: &Green;0x{expectedResult.ProcessorState.HL_:X4}      &Cyan;HL'&White;: {(expectedResult.ProcessorState.HL_ == processor.State.Registers.ReadPair(Register.HL_) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.HL_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;IX &White;: &Green;0x{expectedResult.ProcessorState.IX:X4}      &Cyan;IX &White;: {(expectedResult.ProcessorState.IX == processor.State.Registers.ReadPair(Register.IX) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.AF_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;IY &White;: &Green;0x{expectedResult.ProcessorState.IY:X4}      &Cyan;IY &White;: {(expectedResult.ProcessorState.IY == processor.State.Registers.ReadPair(Register.IY) ? "&Green;" : "&Red;")}0x{processor.State.Registers.ReadPair(Register.BC_):X4}");
        FormattedConsole.WriteLine($"    &Cyan;PC &White;: &Green;0x{expectedResult.ProcessorState.PC:X4}      &Cyan;PC &White;: {(expectedResult.ProcessorState.PC == processor.State.ProgramCounter ? "&Green;" : "&Red;")}0x{processor.State.ProgramCounter:X4}");
        FormattedConsole.WriteLine($"    &Cyan;SP &White;: &Green;0x{expectedResult.ProcessorState.SP:X4}      &Cyan;SP &White;: {(expectedResult.ProcessorState.SP == processor.State.StackPointer ? "&Green;" : "&Red;")}0x{processor.State.StackPointer:X4}");
        FormattedConsole.WriteLine($"    &Cyan;I  &White;: &Green;0x{expectedResult.ProcessorState.I:X2}        &Cyan;I  &White;: {(expectedResult.ProcessorState.I == processor.State.Registers[Register.I] ? "&Green;" : "&Red;")}0x{processor.State.Registers[Register.I]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;R  &White;: &Green;0x{expectedResult.ProcessorState.R:X2}        &Cyan;R  &White;: {(expectedResult.ProcessorState.R == processor.State.Registers[Register.R] ? "&Green;" : "&Red;")}0x{processor.State.Registers[Register.R]:X2}");

        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.Write($"    &Cyan;IFF1&White;: {(expectedResult.ProcessorState.IFF1 == processor.State.InterruptFlipFlop1 ? "&Green;" : "&Red;")}{processor.State.InterruptFlipFlop1.ToString().ToLower()}");
        FormattedConsole.Write($"      &Cyan;IFF2&White;: {(expectedResult.ProcessorState.IFF2 == processor.State.InterruptFlipFlop2 ? "&Green;" : "&Red;")}{processor.State.InterruptFlipFlop2.ToString().ToLower()}");
        FormattedConsole.Write($"      &Cyan;Mode&White;: {(expectedResult.ProcessorState.InterruptMode == (int) processor.State.InterruptMode ? "&Green;" : "&Red;")}{(int) processor.State.InterruptMode}");
        FormattedConsole.WriteLine($"      &Cyan;HALT&White;: {(expectedResult.ProcessorState.Halted == processor.State.Halted ? "&Green;" : "&Red;")}{processor.State.Halted.ToString().ToLower()}");

        return false;
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