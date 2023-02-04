namespace ZXE.Core.Z80;

public class State
{
    public int ProgramCounter = 0;

    public Registers Registers;

    public State()
    {
        Registers = new Registers();
    }
}