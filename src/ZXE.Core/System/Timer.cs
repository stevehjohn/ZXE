using System.Timers;
using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Action OnTick { get; init; }

    private readonly global::System.Timers.Timer _timer = new();

    public Timer(double speedHz)
    {
        _timer.Interval = 1 / speedHz / 1_000d;

        _timer.Elapsed += Tick;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    private void Tick(object? source, ElapsedEventArgs arguments)
    {
        OnTick();
    }
}