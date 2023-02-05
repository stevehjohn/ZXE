using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Interfaces;

public interface ITracer
{
    void Trace(string mnemonic, State state, Ram ram);
}