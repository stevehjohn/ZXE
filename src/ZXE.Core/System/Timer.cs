using System.Diagnostics;
using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Func<int> OnTick { get; init; }

    public required Action HandleRefreshInterrupt { get; init; }

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CancellationToken _cancellationToken;

    private readonly double _microsecondsPerCycle;

    public Timer(double speedHz)
    {
        _microsecondsPerCycle = 1.0f / speedHz * Stopwatch.Frequency;

        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;
    }

    public void Start()
    {
        Task.Run(TimerWorker, _cancellationToken);

        Task.Run(InterruptWorker, _cancellationToken);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();

        _cancellationTokenSource.Dispose();
    }

    private void TimerWorker()
    {
        var stopwatch = new Stopwatch();

        var cycles = 0;

        while (true)
        {
            //stopwatch.Restart();

            //while (stopwatch.ElapsedTicks < cycles * _microsecondsPerCycle)
            //{
            //}

            cycles = OnTick();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private void InterruptWorker()
    {
        var stopwatch = new Stopwatch();

        while (true)
        {
            stopwatch.Restart();

            while (stopwatch.ElapsedMilliseconds < 50)
            {
            }

            HandleRefreshInterrupt();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}