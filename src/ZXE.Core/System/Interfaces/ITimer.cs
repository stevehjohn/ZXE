namespace ZXE.Core.System.Interfaces;

public interface ITimer : IDisposable
{
    Func<int> OnTick { init; }

    void Start();

    void Stop();

    void Pause();

    void Resume();

    bool Fast { get; set; }
}