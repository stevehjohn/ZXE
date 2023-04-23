namespace ZXE.Core.Z80;

public class State
{
    private int _programCounter;

    public int ProgramCounter
    {
        get => _programCounter;
        set => _programCounter = value & 0xFFFF;
    }

    private int _stackPointer;

    public int StackPointer
    {
        get => _stackPointer;
        // TODO: Detect SO?
        set =>_stackPointer = value & 0xFFFF;
    }

    public Registers Registers;

    public Flags Flags { get; set; }

    public ushort MemPtr { get; set; }

    public bool Halted { get; set; }

    public InterruptMode InterruptMode { get; set; } = InterruptMode.Mode0;

    public int OpcodePrefix { get; set; }

    public bool InterruptFlipFlop1 { get; set; }

    public bool InterruptFlipFlop2 { get; set; }

    public bool SkipInterrupt { get; set; }

    public InterruptType InterruptType { get; set; }

    public byte Q { get; set; }

    public byte Last7ffd { get; set; }

    public byte Last1ffd { get; set; }

    public State()
    {
        Registers = new Registers();

        Flags = new Flags();
    }

    public void PutFlagsInFRegister()
    {
        var asByte = Flags.ToByte();

        Registers[Register.F] = asByte;

        Q = asByte;
    }

    public void ResetQ()
    {
        Q = 0;
    }
}