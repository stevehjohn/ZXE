namespace ZXE.Core.FuseTests.Exceptions;

public class TestExpectedResultNotFoundException : Exception
{
    public TestExpectedResultNotFoundException(string message) : base(message)
    {
    }
}