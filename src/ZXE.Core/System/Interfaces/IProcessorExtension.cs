using ZXE.Core.Z80;

namespace ZXE.Core.System.Interfaces;

public interface IProcessorExtension
{
    bool InterceptInstruction(State state, Ram ram);
}