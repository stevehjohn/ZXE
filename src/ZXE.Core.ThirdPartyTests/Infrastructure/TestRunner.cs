﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ZXE.Common.ConsoleHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Core.ThirdPartyTests.Models;
using ZXE.Core.Z80;

namespace ZXE.Core.ThirdPartyTests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestRunner
{
    public void RunAllTests()
    {
        var files = Directory.EnumerateFiles("TestDefinitions", "*.json");

        FormattedConsole.WriteLine(string.Empty);

        var total = 0;

        var passed = 0;

        foreach (var file in files)
        {
            var tests = JsonSerializer.Deserialize<TestDefinition[]>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tests == null)
            {
                continue;
            }

            foreach (var test in tests)
            {
                total++;

                if (RunTest(test))
                {
                    passed++;
                }

                Thread.Sleep(250);
            }
        }
    }

    private static bool RunTest(TestDefinition test)
    {
        FormattedConsole.Write($"  &Cyan;Test&White;: &Magenta;{test.Name, -12}  ");

        FormattedConsole.Write($"  &Cyan;RAM&White;: &Magenta;{test.Initial.Ram.Length, 3}B  ");

        var result = ExecuteTest(test);

        FormattedConsole.Write($"  &Cyan;Operations&White;: &Magenta;{result.Operations, 3}  ");

        FormattedConsole.Write("  &Cyan;Result&White; &White;[ ");

        if (result.Passed)
        {
            FormattedConsole.Write("&Green;PASS");
        }
        else
        {
            FormattedConsole.Write("&Red;FAIL");
        }

        FormattedConsole.WriteLine("&White; ]");

        FormattedConsole.WriteLine(string.Empty);

        return result.Passed;
    }

    private static (bool Passed, int Operations) ExecuteTest(TestDefinition test)
    {
        var ram = new Ram(Model.Spectrum48K);

        var processor = new Processor();

        var state = new State();

        processor.SetState(state);

        state.ProgramCounter = test.Initial.PC;
        state.StackPointer = test.Initial.SP;
        state.Registers[Register.A] = test.Initial.A;
        state.Registers[Register.B] = test.Initial.B;
        state.Registers[Register.C] = test.Initial.C;
        state.Registers[Register.D] = test.Initial.D;
        state.Registers[Register.E] = test.Initial.E;
        state.Registers[Register.H] = test.Initial.H;
        state.Registers[Register.L] = test.Initial.L;

        foreach (var pair in test.Initial.Ram)
        {
            ram[pair[0]] = (byte) pair[1];
        }

        var operations = CountOperations(test.Cycles);

        for (var i = 0; i < operations; i++)
        {
            processor.ProcessInstruction(ram);
        }

        var pass = state.ProgramCounter == test.Initial.PC
                   && state.StackPointer == test.Initial.SP
                   && state.Registers[Register.A] == test.Initial.A
                   && state.Registers[Register.B] == test.Initial.B
                   && state.Registers[Register.C] == test.Initial.C
                   && state.Registers[Register.D] == test.Initial.D
                   && state.Registers[Register.E] == test.Initial.E
                   && state.Registers[Register.H] == test.Initial.H
                   && state.Registers[Register.L] == test.Initial.L;

        foreach (var pair in test.Final.Ram)
        {
            pass = pass && ram[pair[0]] == ram[pair[1]];
        }

        return (pass, operations);
    }

    private static int CountOperations(object[][] cycles)
    {
        var last = int.MinValue;

        var count = 1;

        foreach (var cycle in cycles)
        {
            var addressBus = (int) ((JsonElement) cycle[0]).ValueKind;

            if (addressBus == last)
            {
                continue;
            }

            last = addressBus;

            count++;
        }

        return count;
    }
}