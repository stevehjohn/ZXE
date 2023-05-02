using Xunit;
using Timer = ZXE.Core.System.Timer;

namespace ZXE.Core.Tests.System;

public class TimerTests
{
    private int _callCount;

    [Fact]
    public void Timer_fires_rapidly()
    {
        _callCount = 0;

        using var timer = new Timer(3_500_000)
                          {
                              OnTick = OnTick,
                              HandleRefreshInterrupt = RefreshInterrupt,
                              FrameFinished = FrameFinished
                          };

        timer.Start();

        Thread.Sleep(200);

        Assert.True(_callCount > 5);
    }

    private void FrameFinished()
    {
        throw new NotImplementedException();
    }

    private int OnTick()
    {
        _callCount++;

        return 5;
    }

    private static void RefreshInterrupt()
    {
    }
}