using ZXE.Core.Z80;

namespace ZXE.Core.System.Interfaces;

public interface IProcessorExtension
{
    void InterceptInstruction(State state, Ram ram);
}