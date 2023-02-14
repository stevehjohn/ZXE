namespace ZXE.Core.Z80;

public class State
{
    public int ProgramCounter = 0;

    public int StackPointer = 0;

    public Registers Registers;

    public Flags Flags { get; set; }

    public bool Halted { get; set; }

    public InterruptMode InterruptMode { get; set; } = InterruptMode.Mode0;

    public int OpcodePrefix { get; set; }

    public bool InterruptFlipFlop1 { get; set; }

    public bool InterruptFlipFlop2 { get; set; }

    public State()
    {
        Registers = new Registers();

        Flags = new Flags();
    }
}