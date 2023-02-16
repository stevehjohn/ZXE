using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Windows.Host.Infrastructure;

public static class KeyboardMapper
{
    private static readonly int[] Ports = { 0xFEFE, 0xFDFE, 0xFBFE, 0xF7FE, 0xEFFE, 0xDFFE, 0xBFFE, 0x7FFE };

    public static List<(int Port, byte data)> MapKeyState(Keys[] keyboardState)
    {
        var portData = new List<(int Port, byte data)>();

        // Just assume up to 2 keys for now.
        var keys = keyboardState.Take(2).ToArray();

        foreach (var port in Ports)
        {
            var data = GetPortData(0xFEFE, keys[0]);

            if (keys.Length > 1)
            {
                data |= GetPortData(0xFEFE, keys[1]);
            }

            portData.Add((port, data));
        }

        return portData;
    }

    private static byte GetPortData(int port, Keys keys)
    {
        switch (port)
        {
            case 0xFDFE:
                return ScanForFDFEKeys(keys);

            default:
                // TODO: Should probably throw an exception.
                return 0;
        }
    }

    private static byte ScanForFDFEKeys(Keys keys)
    {
        if (keys == Keys.A)
        {
            return 0b11111110;
        }

        if (keys == Keys.S)
        {
            return 0b11111101;
        }

        if (keys == Keys.D)
        {
            return 0b11111011;
        }

        if (keys == Keys.F)
        {
            return 0b11110111;
        }

        if (keys == Keys.G)
        {
            return 0b11101111;
        }

        return 0;
    }

    private static byte ScanForFBFEKeys(Keys keys)
    {
        if (keys == Keys.Q)
        {
            return 0b11111110;
        }

        if (keys == Keys.W)
        {
            return 0b11111101;
        }

        if (keys == Keys.E)
        {
            return 0b11111011;
        }

        if (keys == Keys.R)
        {
            return 0b11110111;
        }

        if (keys == Keys.T)
        {
            return 0b11101111;
        }

        return 0;
    }
}