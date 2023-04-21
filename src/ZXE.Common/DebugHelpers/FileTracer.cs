using System.Text;
using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Common.DebugHelpers;

public class FileTracer : IDisposable, ITracer
{
    private readonly FileStream _file;

    public FileTracer()
    {
        if (File.Exists("trace.log"))
        {
            File.Delete("trace.log");

        }

        _file = File.OpenWrite("trace.log");
    }

    public void TraceBefore(Instruction instruction, byte[] data, State state, Ram ram)
    {
        var builder = new StringBuilder();

        builder.Append($"{instruction.Mnemonic,-15}");

        for (var i = 0; i < 7; i++)
        {
            if (data.Length < i - 1)
            {
                builder.Append($"{data[i]:X2} ");
            }
            else
            {
                builder.Append("   ");
            }
        }

        builder.Append($"PC: {state.ProgramCounter:X4} ");

        builder.Append($"SP: {state.StackPointer:X4} ");

        builder.Append(Environment.NewLine);

        _file.Write(Encoding.UTF8.GetBytes(builder.ToString()));
    }

    public void TraceAfter(Instruction instruction, byte[] data, State state, Ram ram)
    {
    }

    public List<string> GetTrace()
    {
        throw new NotImplementedException();
    }

    public void ClearTrace()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _file.Close();

        _file.Dispose();
    }
}