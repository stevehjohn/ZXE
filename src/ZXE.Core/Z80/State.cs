namespace ZXE.Core.Z80;

public class State
{
    public int ProgramCounter = 0;

    public int StackPointer = 0;

    public Registers Registers;

    public Flags Flags { get; set; }

    public State()
    {
        Registers = new Registers();
    }
}