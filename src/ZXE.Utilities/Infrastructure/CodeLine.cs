namespace ZXE.Utilities.Infrastructure;

public class CodeLine
{
    public int Address { get; }

    public string Mnemonic { get; }

    public byte[]? Parameters { get; }

    public CodeLine(int address, string mnemonic, byte[]? parameters = null)
    {
        Address = address;

        Mnemonic = mnemonic;

        Parameters = parameters;
    }
}