﻿namespace ZXE.Core.Extensions;

public static class ByteExtensions
{
    public static bool IsEvenParity(this byte value)
    {
        var bit = 1;

        var count = 0;

        for (var i = 0; i < 8; i++)
        {
            count += (value & bit) > 0 ? 1 : 0;

            bit <<= 1;
        }

        return count % 2 == 0;
    }
}