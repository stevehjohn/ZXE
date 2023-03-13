namespace ZXE.Core.System.Interfaces;

public interface ITimer : IDisposable
{
    Func<int> OnTick { init; }

    void Start();

    void Stop();
}