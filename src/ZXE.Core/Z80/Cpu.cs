namespace ZXE.Core.Z80;

public class Cpu
{
    private readonly State _state;

    private readonly Processor _processor;

    public Cpu(State state, Processor processor)
    {
        _state = state;

        _processor = processor;
    }
}