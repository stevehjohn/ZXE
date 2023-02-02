using Xunit;
using Timer = ZXE.Core.System.Timer;

namespace ZXE.Core.Tests.System;

public class TimerTests
{
    [Fact]
    public void Timer_raises_repeated_events()
    {
        var callCount = 0L;

        using var timer = new Timer(3_500_000)
                          {
                              OnTick = () => callCount++
                          };

        timer.Start();

        Thread.Sleep(2000);

        Assert.True(callCount > 2);
    }
}