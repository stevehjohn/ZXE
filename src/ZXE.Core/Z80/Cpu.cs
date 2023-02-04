namespace ZXE.Core.Z80;

public class Cpu
{
    private readonly State _state;

    private readonly Processor _processor;

    public Cpu()
    {
        _state = new State();

        _processor = new Processor();
    }
}