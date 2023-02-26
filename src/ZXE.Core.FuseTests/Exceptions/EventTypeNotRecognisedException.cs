namespace ZXE.Core.FuseTests.Exceptions;

public class EventTypeNotRecognisedException : Exception
{
    public EventTypeNotRecognisedException(string message) : base(message)
    {
    }
}