using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Interfaces;

public interface ITracer
{
    void TraceBefore(Instruction instruction, byte[] data, State state, Ram ram);
    
    void TraceAfter(Instruction instruction, byte[] data, State state, Ram ram);
}