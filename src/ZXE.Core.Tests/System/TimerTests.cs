﻿using Xunit;
using Timer = ZXE.Core.System.Timer;

namespace ZXE.Core.Tests.System;

public class TimerTests
{
    [Fact]
    public void Timer_fires_rapidly()
    {
        var callCount = 0L;

        using var timer = new Timer(3_500_000)
                          {
                              OnTick = () => callCount++
                          };

        timer.Start();

        Thread.Sleep(100);

        Assert.True(callCount > 10);
    }
}