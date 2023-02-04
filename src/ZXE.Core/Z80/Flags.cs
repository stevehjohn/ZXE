namespace ZXE.Core.Z80;

public class Flags
{
    public bool Carry { get; set; }
    
    public bool AddSubtract { get; set; }

    public bool ParityOverflow { get; set; }
    
    public bool X1 { get; set; }
    
    public bool HalfCarry { get; set; }
    
    public bool X2 { get; set; }
    
    public bool Zero { get; set; }
    
    public bool Sign { get; set; }

    public byte ToByte()
    {
        return (byte) ((Carry ? 0x01 : 0x00)
                       | (AddSubtract ? 0x02 : 0x00)
                       | (ParityOverflow ? 0x04 : 0x00)
                       | (X1 ? 0x08 : 0x00)
                       | (HalfCarry ? 0x10 : 0x00)
                       | (X2 ? 0x20 : 0x00)
                       | (Zero ? 0x40 : 0x00)
                       | (Sign ? 0x80 : 0x00));
    }
}