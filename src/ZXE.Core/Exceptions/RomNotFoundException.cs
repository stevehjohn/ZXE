namespace ZXE.Core.Exceptions;

public class RomNotFoundException : Exception
{
    public RomNotFoundException(string message) : base(message)
    {
    }
}