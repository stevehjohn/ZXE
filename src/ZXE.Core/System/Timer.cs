using System.Diagnostics;
using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Func<int> OnTick { get; init; }

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
            stopwatch.Restart();

            while (stopwatch.ElapsedTicks < cycles * _microsecondsPerCycle)
            {
            }

            cycles = OnTick();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}