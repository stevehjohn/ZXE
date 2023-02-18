﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ZXE.Common.ConsoleHelpers;
using ZXE.Common.DebugHelpers;
using ZXE.Common.Extensions;
using ZXE.Core.Exceptions;
using ZXE.Core.Infrastructure;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.ThirdPartyTests.Models;
using ZXE.Core.Z80;

namespace ZXE.Core.ThirdPartyTests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestRunner
{
    public static void RunAllTests(bool dumpOnFail = false)
    {
        var files = Directory.EnumerateFiles("TestDefinitions", "*.json");

        FormattedConsole.WriteLine(string.Empty);

        var total = 0;

        var passed = 0;

        var notImplemented = 0;

        Console.CursorVisible = false;

        var stopwatch = Stopwatch.StartNew();

        var failedNames = new List<string>();

        foreach (var file in files)
        {
            // Skip a bunch of tests
            //if (Path.GetFileName(file).CompareTo("fd 18") < 0)
            //{
            //    continue;
            //}

            // End early
            //if (Path.GetFileName(file).CompareTo("dd 2f") > 0)
            //{
            //    continue;
            //}

            var tests = JsonSerializer.Deserialize<TestDefinition[]>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tests == null)
            {
                continue;
            }

            foreach (var test in tests)
            {
                total++;

                var skipRemainder = false;

                switch (RunTest(test))
                {
                    case TestResult.Pass:
                        passed++;

                        break;

                    case TestResult.Fail:
                        if (dumpOnFail)
                        {
                            failedNames.Add(test.Name);

                            DumpTest(test);
                        }

                        break;

                    case TestResult.NotImplemented:
                        notImplemented++;

                        skipRemainder = true;

                        break;
                }

                if (skipRemainder)
                {
                    break;
                }
            }
        }

        var failed = total - (passed + notImplemented);

        stopwatch.Stop();

        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.WriteLine($"  &Cyan;Testing complete. Time elapsed&White;: &Yellow;{stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}.{stopwatch.Elapsed.Milliseconds}");

        FormattedConsole.WriteLine($"\n  &Cyan;Tests Run&White;: &Yellow;{total:N0}    &Cyan;Tests Passed&White;: &Green;{passed:N0}    &Cyan;Not Implemented&White;: &Yellow;{notImplemented}");

        FormattedConsole.WriteLine($"\n  &Cyan;Tests Failed&White;: {(failed == 0 ? "&Green;" : "&Red;")}{failed:N0}    &Cyan;Percent Failed&White;: &Yellow;{(float) failed / total * 100:F2}%");

        FormattedConsole.WriteLine(string.Empty);

        if (failedNames.Count > 0)
        {
            FormattedConsole.WriteLine("  &Cyan;Press any key to see failed test names...\n");

            Console.ReadKey();

            var first = true;

            foreach (var name in failedNames)
            {
                if (first)
                {
                    FormattedConsole.Write($"&Red;{name}");

                    first = false;
                }
                else
                {
                    FormattedConsole.Write($"&White;, &Red;{name}");
                }
            }
        }

        Console.CursorVisible = true;
    }

    private static TestResult RunTest(TestDefinition test)
    {
        FormattedConsole.Write($"  &Cyan;Test&White;: &Magenta;{test.Name,-18}  ");

        FormattedConsole.Write($"  &Cyan;RAM&White;: &Magenta;{test.Initial.Ram.Length,3}B  ");

        var result = ExecuteTest(test);

        FormattedConsole.Write($"  &Cyan;Operations&White;: &Magenta;{result.Operations,6}  ");

        FormattedConsole.Write("  &Cyan;Result&White;: [ ");

        var testResult = TestResult.Fail;

        if (result.Passed)
        {
            FormattedConsole.Write("&Green;PASS");

            testResult = TestResult.Pass;
        }
        else
        {
            if (result.exception != null)
            {
                if (result.exception is OpcodeNotImplementedException)
                {
                    FormattedConsole.Write("&Yellow;NIMP");

                    testResult = TestResult.NotImplemented;
                }
                else
                {
                    FormattedConsole.Write("&Red;EXCP");
                }
            }
            else
            {
                FormattedConsole.Write("&Red;FAIL");
            }
        }

        FormattedConsole.Write("&White; ]");

        FormattedConsole.WriteLine(string.Empty);

        return testResult;
    }

    private static (bool Passed, int Operations, State State, Ram Ram, Exception? exception) ExecuteTest(TestDefinition test, ITracer? tracer = null)
    {
        var ram = new Ram(Model.Spectrum48K);

        var processor = tracer != null
                            ? new Processor(tracer)
                            : new Processor();

        var state = new State();

        var ports = new Ports();

        var bus = new Bus();

        if (test.Ports != null)
        {
            foreach (var port in test.Ports)
            {
                ports.WriteByte(((JsonElement) port[0]).GetInt32(), ((JsonElement) port[1]).GetByte());
            }
        }

        processor.SetState(state);

        state.ProgramCounter = test.Initial.PC;
        state.StackPointer = test.Initial.SP;
        state.Registers[Register.A] = test.Initial.A;
        state.Registers[Register.B] = test.Initial.B;
        state.Registers[Register.C] = test.Initial.C;
        state.Registers[Register.D] = test.Initial.D;
        state.Registers[Register.E] = test.Initial.E;
        state.Registers[Register.F] = test.Initial.F;
        state.Registers[Register.H] = test.Initial.H;
        state.Registers[Register.L] = test.Initial.L;
        state.Registers[Register.I] = test.Initial.I;
        state.Registers[Register.R] = test.Initial.R;
        state.Registers.WritePair(Register.IX, test.Initial.IX);
        state.Registers.WritePair(Register.IY, test.Initial.IY);

        state.Flags = Flags.FromByte(test.Initial.F);

        state.Registers[Register.A1] = (byte) ((test.Initial.AF_ & 0xFF00) >> 8);
        state.Registers[Register.B1] = (byte) ((test.Initial.BC_ & 0xFF00) >> 8);
        state.Registers[Register.C1] = (byte) (test.Initial.BC_ & 0x00FF);
        state.Registers[Register.D1] = (byte) ((test.Initial.DE_ & 0xFF00) >> 8);
        state.Registers[Register.E1] = (byte) (test.Initial.DE_ & 0x00FF);
        state.Registers[Register.F1] = (byte) (test.Initial.AF_ & 0x00FF);
        state.Registers[Register.H1] = (byte) ((test.Initial.HL_ & 0xFF00) >> 8);
        state.Registers[Register.L1] = (byte) (test.Initial.HL_ & 0x00FF);

        foreach (var pair in test.Initial.Ram)
        {
            ram[pair[0]] = (byte) pair[1];
        }

        var operations = 0;

        try
        {
            do
            {
                operations++;

                if (operations > 0xFFFF)
                {
                    break;
                }

                processor.ProcessInstruction(ram, ports, bus);

            } while (state.ProgramCounter != test.Final.PC);
        }
        catch (Exception exception)
        {
            return (false, operations, state, ram, exception);
        }

        // INFO: Edge case, hopefully fixed if this gets an answer: https://github.com/raddad772/jsmoo/issues/23
        //if (state.OpcodePrefix > 0)
        //{
        //    return (false, operations, state, ram, new OpcodeNotImplementedException("Not implemented"));
        //}

        var pass = state.ProgramCounter == test.Final.PC
                   && state.StackPointer == test.Final.SP
                   && state.Registers[Register.A] == test.Final.A
                   && state.Registers[Register.B] == test.Final.B
                   && state.Registers[Register.C] == test.Final.C
                   && state.Registers[Register.D] == test.Final.D
                   && state.Registers[Register.E] == test.Final.E
                   && state.Registers[Register.H] == test.Final.H
                   && state.Registers[Register.I] == test.Final.I
                   && state.Registers[Register.R] == test.Final.R
                   && state.Registers.ReadPair(Register.IX) == test.Final.IX
                   && state.Registers.ReadPair(Register.IY) == test.Final.IY; 
        // TODO: Alternate registers?

        foreach (var pair in test.Final.Ram)
        {
            pass = pass && ram[pair[0]] == pair[1];
        }

        return (pass, operations, state, ram, null);
    }

    private static void DumpTest(TestDefinition test)
    {
        var tracer = new FormattingTracer();

        var result = ExecuteTest(test, tracer);

        if (result.exception != null)
        {
            FormattedConsole.WriteLine($"\n    &Cyan;Exception&White;: &Red;{result.exception.GetType().Name}");
        }

        FormattedConsole.WriteLine("\n&Cyan;        Initial       Expected      Actual");

        FormattedConsole.WriteLine($"    &Cyan;PC&White;: &Green;0x{test.Initial.PC:X4}        &Cyan;PC&White;: &Green;0x{test.Final.PC:X4}        {(test.Final.PC == result.State.ProgramCounter ? "&Green;" : "&Red;")}0x{result.State.ProgramCounter:X4}");
        FormattedConsole.WriteLine($"    &Cyan;SP&White;: &Green;0x{test.Initial.SP:X4}        &Cyan;SP&White;: &Green;0x{test.Final.SP:X4}        {(test.Final.SP == result.State.StackPointer ? "&Green;" : "&Red;")}0x{result.State.StackPointer:X4}");

        FormattedConsole.WriteLine($"    &Cyan;A &White;: &Green;0x{test.Initial.A:X2}          &Cyan;A &White;: &Green;0x{test.Final.A:X2}          {(test.Final.A == result.State.Registers[Register.A] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.A]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;B &White;: &Green;0x{test.Initial.B:X2}          &Cyan;B &White;: &Green;0x{test.Final.B:X2}          {(test.Final.B == result.State.Registers[Register.B] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.B]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;C &White;: &Green;0x{test.Initial.C:X2}          &Cyan;C &White;: &Green;0x{test.Final.C:X2}          {(test.Final.C == result.State.Registers[Register.C] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.C]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;D &White;: &Green;0x{test.Initial.D:X2}          &Cyan;D &White;: &Green;0x{test.Final.D:X2}          {(test.Final.D == result.State.Registers[Register.D] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.D]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;E &White;: &Green;0x{test.Initial.E:X2}          &Cyan;E &White;: &Green;0x{test.Final.E:X2}          {(test.Final.E == result.State.Registers[Register.E] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.E]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;H &White;: &Green;0x{test.Initial.H:X2}          &Cyan;H &White;: &Green;0x{test.Final.H:X2}          {(test.Final.H == result.State.Registers[Register.H] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.H]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;L &White;: &Green;0x{test.Initial.L:X2}          &Cyan;L &White;: &Green;0x{test.Final.L:X2}          {(test.Final.L == result.State.Registers[Register.L] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.L]:X2}");

        FormattedConsole.WriteLine($"    &Cyan;I &White;: &Green;0x{test.Initial.I:X2}          &Cyan;I &White;: &Green;0x{test.Final.I:X2}          {(test.Final.I == result.State.Registers[Register.I] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.I]:X2}");
        FormattedConsole.WriteLine($"    &Cyan;R &White;: &Green;0x{test.Initial.R:X2}          &Cyan;R &White;: &Green;0x{test.Final.R:X2}          {(test.Final.R == result.State.Registers[Register.R] ? "&Green;" : "&Red;")}0x{result.State.Registers[Register.R]:X2}");

        FormattedConsole.WriteLine($"    &Cyan;IX&White;: &Green;0x{test.Initial.IX:X4}        &Cyan;IX&White;: &Green;0x{test.Final.IX:X4}        {(test.Final.IX == result.State.Registers.ReadPair(Register.IX) ? "&Green;" : "&Red;")}0x{result.State.Registers.ReadPair(Register.IX):X4}");
        FormattedConsole.WriteLine($"    &Cyan;IY&White;: &Green;0x{test.Initial.IY:X4}        &Cyan;IY&White;: &Green;0x{test.Final.IY:X4}        {(test.Final.IY == result.State.Registers.ReadPair(Register.IY) ? "&Green;" : "&Red;")}0x{result.State.Registers.ReadPair(Register.IY):X4}");
        
        var initialFlags = Flags.FromByte(test.Final.F);
        
        var expectedFlags = Flags.FromByte(test.Final.F);

        FormattedConsole.WriteLine($"\n    &Cyan;F &White;: &Green;{initialFlags.GetFlags()}      &Cyan;F &White;: &Green;{expectedFlags.GetFlags()}      {(test.Final.F == result.State.Registers[Register.F] ? "&Green;" : "&Red;")}{result.State.Flags.GetFlags()}");

        FormattedConsole.WriteLine(string.Empty);

        FormattedConsole.Write("    &Cyan;RAM differences&White;: ");

        var ramDifference = false;

        foreach (var entry in test.Final.Ram)
        {
            if (result.Ram[entry[0]] != entry[1])
            {
                ramDifference = true;

                break;
            }
        }

        if (! ramDifference)
        {
            FormattedConsole.WriteLine("&Green; NONE\n");
        }
        else
        {
            FormattedConsole.WriteLine("\n");
            
            FormattedConsole.WriteLine("&Cyan;              Expected    Actual");

            foreach (var entry in test.Final.Ram)
            {
                if (result.Ram[entry[0]] != entry[1])
                {
                    FormattedConsole.Write($"      &Cyan;0x{entry[0]:X4}&White;:");

                    FormattedConsole.Write($"     &Green;0x{entry[1]:X2}");

                    FormattedConsole.WriteLine($"      &Red;0x{result.Ram[entry[0]]:X2}");
                }
            }

            FormattedConsole.WriteLine(string.Empty);
        }

        var trace = tracer.GetTrace();

        for (var i = 0; i < 60; i++)
        {
            if (i >= trace.Count)
            {
                break;
            }

            FormattedConsole.WriteLine($"    {trace[i]}");
        }

        //FormattedConsole.WriteLine("\n    &Cyan;Press any key to continue...\n");

        //Console.ReadKey();
    }
}