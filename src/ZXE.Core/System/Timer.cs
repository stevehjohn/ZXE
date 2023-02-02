using ZXE.Core.System.Interfaces;

namespace ZXE.Core.System;

public class Timer : ITimer
{
    public required Action OnTick { get; init; }

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CancellationToken _cancellationToken;

    private Task? _timer;

    // ReSharper disable once UnusedParameter.Local - Will be used eventually.
    public Timer(double speedHz)
    {
        // TODO: Gonna need to use speedHz to slow the timer down when in Release mode I think.

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

        _timer = null;
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