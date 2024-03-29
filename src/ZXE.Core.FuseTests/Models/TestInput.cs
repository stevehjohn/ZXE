﻿using System.Globalization;

namespace ZXE.Core.FuseTests.Models;

// ReSharper disable InconsistentNaming

public class TestInput
{
    public string Name { get; }

    public ProcessorState ProcessorState { get; }

    public byte?[] Ram { get; }

    public TestInput(string[] testData)
    {
        Name = testData[0];

        ProcessorState = new ProcessorState(testData[1..3]);

        Ram = new byte?[0xFFFF];

        var line = 3;

        while (testData[line] != "-1")
        {
            var parts = testData[line].Split(' ');

            var address = ushort.Parse(parts[0], NumberStyles.HexNumber);

            var part = 1;

            while (parts[part] != "-1")
            {
                Ram[address] = byte.Parse(parts[part], NumberStyles.HexNumber);

                address++;

                part++;
            }

            line++;
        }
    }
}