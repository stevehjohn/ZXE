namespace ZXE.Core.System.Interfaces;

public interface ITimer : IDisposable
{
    Action OnTick { init; }

    void Start();
}