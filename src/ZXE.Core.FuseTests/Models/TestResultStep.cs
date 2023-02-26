using System.Globalization;
using ZXE.Core.FuseTests.Exceptions;

namespace ZXE.Core.FuseTests.Models;

public class TestResultStep
{
    public int Time { get; }

    public EventType EventType { get; }

    public int Address { get; }

    public byte? Data { get; }

    public TestResultStep(string data)
    {
        var parts = data.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        Time = int.Parse(parts[0]);

        EventType = parts[1] switch
        {
            "MR" => EventType.MemoryRead,
            "MW" => EventType.MemoryWrite,
            "MC" => EventType.MemoryContend,
            "PR" => EventType.PortRead,
            "PW" => EventType.PortWrite,
            "PC" => EventType.PortContend,
            _ => throw new EventTypeNotRecognisedException(parts[1])
        };
        

        Address = int.Parse(parts[2], NumberStyles.HexNumber);

        if (parts.Length > 3)
        {
            Data = byte.Parse(parts[3], NumberStyles.HexNumber);
        }
    }
}