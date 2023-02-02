using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Action OnTick { get; init; }

    private Thread? _timer;

    public Timer(double speedHz)
    {
    }

    public void Start()
    {
        _timer = new Thread(TimerWorker)
                 {
                     Priority = ThreadPriority.Highest
                 };

        _timer.Start();
    }

    public void Dispose()
    {
    }

    private void TimerWorker()
    {
        while (true)
        {
            OnTick();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}