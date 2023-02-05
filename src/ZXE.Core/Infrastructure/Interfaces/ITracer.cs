using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Interfaces;

public interface ITracer
{
    void TraceBefore(string mnemonic, byte[] data, State state, Ram ram);
    
    void TraceAfter(string mnemonic, byte[] data, State state, Ram ram);
}