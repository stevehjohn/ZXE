using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Common;
using ZXE.Core.System;
using Color = Microsoft.Xna.Framework.Color;

namespace ZXE.Windows.Host.Display;

public class VRamAdapter
{
    private readonly Ram _ram;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    public VRamAdapter(Ram ram, GraphicsDeviceManager graphicsDeviceManager)
    {
        _ram = ram;

        _graphicsDeviceManager = graphicsDeviceManager;
    }

    public Texture2D GetDisplay()
    {
        var texture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, Constants.ScreenWidthPixels, Constants.ScreenHeightPixels);

        var data = new Color[Constants.ScreenWidthPixels * Constants.ScreenHeightPixels];

        var i = 0;

        for (var y = 0; y < Constants.ScreenHeightPixels; y++)
        {
            for (var x = 0; x < Constants.ScreenWidthBytes; x++)
            {
                var address = 0b0100_0000_0000_0000;

                address |= (y & 0b0000_0111) << 8;

                address |= (y & 0b1100_0000) << 5;

                address |= (y & 0b0011_1000) << 2;

                address |= x;

                var segment = _ram[address];

                var colours = GetColours(x, y);

                for (var b = 7; b >= 0; b--)
                {
                    if ((segment & (0x01 << b)) > 0)
                    {
                        data[i] = colours.Foreground;
                    }
                    else
                    {
                        data[i] = colours.Background;
                    }

                    i++;
                }
            }
        }

        texture.SetData(data);

        return texture;
    }

    private (Color Foreground, Color Background) GetColours(int x, int y)
    {
        var colourAddress = 0x5800;

        var offset = x + y / 8 * 32;

        colourAddress += offset;

        var data = _ram[colourAddress];

        var background = ((data & 0b00111000) >> 3) switch
        {
            0 => Color.Black,
            1 => Color.Blue,
            2 => Color.Red,
            3 => Color.Magenta,
            4 => Color.Green,
            5 => Color.Cyan,
            6 => Color.Yellow,
            7 => Color.White,
            _ => Color.Black
        };

        var foreground = (data & 0b00000111) switch
        {
            0 => Color.Black,
            1 => Color.Blue,
            2 => Color.Red,
            3 => Color.Magenta,
            4 => Color.Green,
            5 => Color.Cyan,
            6 => Color.Yellow,
            7 => Color.White,
            _ => Color.Black
        };

        return (foreground, background);
    }
}