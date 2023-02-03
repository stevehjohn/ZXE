using ZXE.Core.System;

namespace ZXE.Core.Z80;

public class Cpu
{
    private readonly State _state;

    private readonly Ram _ram;

    private readonly Processor _processor;

    public Cpu(Ram ram)
    {
        _ram = ram;

        _state = new State();

        _processor = new Processor();
    }

    public void Tick()
    {
        _processor.ProcessInstruction(_ram, _state);
    }
}