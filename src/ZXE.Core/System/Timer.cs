using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Action OnTick { get; init; }

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CancellationToken _cancellationToken;

    private Task? _timer;

    public Timer(double speedHz)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;
    }

    public void Start()
    {
        _timer = Task.Run(TimerWorker, _cancellationToken);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();

        _cancellationTokenSource.Dispose();
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