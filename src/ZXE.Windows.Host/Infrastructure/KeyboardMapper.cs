using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace ZXE.Windows.Host.Infrastructure;

public static class KeyboardMapper
{
    private static readonly ushort[] Ports = { 0xFEFE, 0xFDFE, 0xFBFE, 0xF7FE, 0xEFFE, 0xDFFE, 0xBFFE, 0x7FFE };

    public static List<(ushort Port, byte data)> MapKeyState(Keys[] keyboardState)
    {
        var portData = new List<(ushort Port, byte data)>();

        // Just assume up to 2 keys for now.
        var keys = keyboardState.Take(2).ToArray();

        foreach (var port in Ports)
        {
            if (keys.Length == 0)
            {
                portData.Add((port, 0b11111111));

                continue;
            }

            var data = GetPortData(port, keys[0]);

            if (keys.Length > 1)
            {
                data &= GetPortData(port, keys[1]);
            }

            portData.Add((port, data));
        }

        return portData;
    }

    private static byte GetPortData(int port, Keys keys)
    {
        switch (port)
        {
            case 0xFEFE:
                return ScanForFEFEKeys(keys);

            case 0xFDFE:
                return ScanForFDFEKeys(keys);

            case 0xFBFE:
                return ScanForFBFEKeys(keys);

            case 0xF7FE:
                return ScanForF7FEKeys(keys);

            case 0xEFFE:
                return ScanForEFFEKeys(keys);

            case 0xDFFE:
                return ScanForDFFEKeys(keys);

            case 0xBFFE:
                return ScanForBFFEKeys(keys);

            case 0x7FFE:
                return ScanFor7FFEKeys(keys);

            default:
                // TODO: Should probably throw an exception.
                return 0b11111111;
        }
    }

    private static byte ScanForFEFEKeys(Keys keys)
    {
        var data = (byte) 0b1111_1111;

        // Caps shift
        if (keys == Keys.LeftShift || keys == Keys.RightShift)
        {
            data = (byte) (data & 0b1111_1110);
        }

        // Account for PC arrow keys (caps + 5, 6, 7 or 8 on the speccy)
        if (keys == Keys.Up || keys == Keys.Down || keys == Keys.Left || keys == Keys.Right)
        {
            data = (byte) (data & 0b1111_1110);
        }

        if (keys == Keys.Z)
        {
            data = (byte) (data & 0b1111_1101);
        }

        if (keys == Keys.X)
        {
            data = (byte) (data & 0b1111_1011);
        }

        if (keys == Keys.C)
        {
            data = (byte) (data & 0b1111_0111);
        }

        if (keys == Keys.V)
        {
            data = (byte) (data & 0b1110_1111);
        }

        return data;
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

        return 0b11111111;
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

        return 0b11111111;
    }

    private static byte ScanForF7FEKeys(Keys keys)
    {
        var data = (byte) 0b1111_1111;

        // Numeric 1 - 5
        if (keys == Keys.D1)
        {
            data = (byte) (data & 0b1111_1110);
        }

        if (keys == Keys.D2)
        {
            data = (byte) (data & 0b1111_1101);
        }

        if (keys == Keys.D3)
        {
            data = (byte) (data & 0b1111_1011);
        }

        if (keys == Keys.D4)
        {
            data = (byte) (data & 0b1111_0111);
        }

        if (keys == Keys.D5)
        {
            data = (byte) (data & 0b1110_1111);
        }

        // Windows left arrow
        if (keys == Keys.Left)
        {
            data = (byte) (data & 0b1110_1111);
        }

        return data;
    }

    private static byte ScanForEFFEKeys(Keys keys)
    {
        // Numeric 0, 6 - 9
        if (keys == Keys.D0)
        {
            return 0b11111110;
        }

        if (keys == Keys.D9)
        {
            return 0b11111101;
        }

        if (keys == Keys.D8)
        {
            return 0b11111011;
        }

        if (keys == Keys.D7)
        {
            return 0b11110111;
        }

        if (keys == Keys.D6)
        {
            return 0b11101111;
        }

        return 0b11111111;
    }

    private static byte ScanForDFFEKeys(Keys keys)
    {
        if (keys == Keys.P)
        {
            return 0b11111110;
        }

        if (keys == Keys.O)
        {
            return 0b11111101;
        }

        if (keys == Keys.I)
        {
            return 0b11111011;
        }

        if (keys == Keys.U)
        {
            return 0b11110111;
        }

        if (keys == Keys.Y)
        {
            return 0b11101111;
        }

        return 0b11111111;
    }

    private static byte ScanForBFFEKeys(Keys keys)
    {
        if (keys == Keys.Enter)
        {
            return 0b11111110;
        }

        if (keys == Keys.L)
        {
            return 0b11111101;
        }

        if (keys == Keys.K)
        {
            return 0b11111011;
        }

        if (keys == Keys.J)
        {
            return 0b11110111;
        }

        if (keys == Keys.H)
        {
            return 0b11101111;
        }

        return 0b11111111;
    }

    private static byte ScanFor7FFEKeys(Keys keys)
    {
        if (keys == Keys.Space)
        {
            return 0b11111110;
        }

        // Symbol shift
        if (keys == Keys.LeftAlt)
        {
            return 0b11111101;
        }

        if (keys == Keys.M)
        {
            return 0b11111011;
        }

        if (keys == Keys.N)
        {
            return 0b11110111;
        }

        if (keys == Keys.B)
        {
            return 0b11101111;
        }

        return 0b11111111;
    }
}