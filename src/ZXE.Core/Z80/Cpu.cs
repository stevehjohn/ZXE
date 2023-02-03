using ZXE.Core.System;

namespace ZXE.Core.Z80;

public class Cpu
{
    private readonly State _state;

    private readonly Ram _ram;

    private readonly Processor _processor;

    public Cpu(State state, Ram ram, Processor processor)
    {
        _state = state;

        _ram = ram;

        _processor = processor;
    }

    public void Tick()
    {
        _processor.ProcessInstruction(_ram, _state);
    }
}