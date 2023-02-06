using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ZXE.Common.ConsoleHelpers;
using ZXE.Core.Infrastructure;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.ThirdPartyTests.Models;
using ZXE.Core.Z80;

namespace ZXE.Core.ThirdPartyTests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestRunner
{
    public void RunAllTests(bool dumpOnFail = false)
    {
        var files = Directory.EnumerateFiles("TestDefinitions", "*.json");

        FormattedConsole.WriteLine(string.Empty);

        var total = 0;

        var passed = 0;

        Console.CursorVisible = false;

        var stopwatch = Stopwatch.StartNew();

        foreach (var file in files)
        {
            var tests = JsonSerializer.Deserialize<TestDefinition[]>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tests == null)
            {
                continue;
            }

            foreach (var test in tests)
            {
                if (test.Name.StartsWith("A9", StringComparison.InvariantCultureIgnoreCase))
                {
                    goto done;
                }

                total++;

                if (RunTest(test))
                {
                    passed++;
                }
                else if (dumpOnFail)
                {
                    DumpTest(test);
                }
            }
        }

        stopwatch.Stop();

        done:
        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.WriteLine($"  &Cyan;Testing complete. Time elapsed&White;: &Yellow; {stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}.{stopwatch.Elapsed.Milliseconds}");

        FormattedConsole.WriteLine($"\n  &Cyan;Tests Run&White;: &Yellow;{total:N0}    &Cyan;Tests Passed&White;: &Green;{passed:N0}    ");

        FormattedConsole.WriteLine($"\n  &Cyan;Tests Failed&White;: &Red;{total - passed:N0}    &Cyan;Percent Failed&White;: &Yellow;{((float) total - passed) / total * 100:F2}%");

        FormattedConsole.WriteLine(string.Empty);

        Console.CursorVisible = true;
    }

    private static bool RunTest(TestDefinition test)
    {
        FormattedConsole.Write($"  &Cyan;Test&White;: &Magenta;{test.Name,-12}  ");

        FormattedConsole.Write($"  &Cyan;RAM&White;: &Magenta;{test.Initial.Ram.Length,3}B  ");

        var result = ExecuteTest(test);

        FormattedConsole.Write($"  &Cyan;Operations&White;: &Magenta;{result.Operations,6}  ");

        FormattedConsole.Write("  &Cyan;Result&White;: [ ");

        if (result.Passed)
        {
            FormattedConsole.Write("&Green;PASS");
        }
        else
        {
            FormattedConsole.Write("&Red;FAIL");
        }

        FormattedConsole.Write("&White; ]");

        FormattedConsole.WriteLine(string.Empty);

        return result.Passed;
    }

    private static (bool Passed, int Operations, State State) ExecuteTest(TestDefinition test, ITracer? tracer = null)
    {
        var ram = new Ram(Model.Spectrum48K);

        var processor = tracer != null
                            ? new Processor(tracer)
                            : new Processor();

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

        var operations = 0;

        try
        {
            while (state.ProgramCounter != test.Final.PC)
            {
                operations++;

                if (operations > 99_999)
                {
                    break;
                }

                processor.ProcessInstruction(ram);
            }
        }
        catch
        {
            return (false, operations, state);
        }

        var pass = state.ProgramCounter == test.Final.PC
                   && state.StackPointer == test.Final.SP
                   && state.Registers[Register.A] == test.Final.A
                   && state.Registers[Register.B] == test.Final.B
                   && state.Registers[Register.C] == test.Final.C
                   && state.Registers[Register.D] == test.Final.D
                   && state.Registers[Register.E] == test.Final.E
                   && state.Registers[Register.H] == test.Final.H
                   && state.Registers[Register.L] == test.Final.L;

        foreach (var pair in test.Final.Ram)
        {
            pass = pass && ram[pair[0]] == pair[1];
        }

        return (pass, operations, state);
    }

    private void DumpTest(TestDefinition test)
    {
        var tracer = new FormattingTracer();

        var result = ExecuteTest(test, tracer);

        FormattedConsole.WriteLine("\n&Cyan;        Expected    Actual");
        FormattedConsole.WriteLine($"    &Cyan;PC&White;: &Green;{test.Final.PC:X4}         {(test.Final.PC == result.State.ProgramCounter ? "&Green;" : "&Red;")}{result.State.ProgramCounter:X4}");

        var trace = tracer.GetTrace();

        for (var i = 0; i < 60; i++)
        {
            if (i >= trace.Count)
            {
                break;
            }

            FormattedConsole.WriteLine($"    {trace[i]}");
        }

        FormattedConsole.WriteLine("\n    &Cyan;Press any key to continue...\n");

        Console.ReadKey();
    }
}