namespace ZXE.Core.Z80;

public class Flags
{
    public bool Carry { get; set; }          // C
    
    public bool AddSubtract { get; set; }    // N

    public bool ParityOverflow { get; set; } // P/V
    
    public bool X1 { get; set; }             // Supposedly unused
     
    public bool HalfCarry { get; set; }      // H
    
    public bool X2 { get; set; }             // Supposedly unused
    
    public bool Zero { get; set; }           // Z
    
    public bool Sign { get; set; }           // S

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

    public static Flags FromByte(byte data)
    {
        return new Flags
               {
                   Carry = (data & 0x01) > 0,
                   AddSubtract = (data & 0x02) > 0,
                   ParityOverflow = (data & 0x04) > 0,
                   X1 = (data & 0x08) > 0,
                   HalfCarry = (data & 0x10) > 0,
                   X2 = (data & 0x20) > 0,
                   Zero = (data & 0x40) > 0,
                   Sign = (data & 0x80) > 0
               };
    }
}