using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Core.System;

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
        // TODO: Magic numbers...
        var texture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, 256, 192);

        var data = new Color[256 * 192];

        var i = 0;

        for (var y = 0; y < 192; y++)
        {
            for (var x = 0; x < 32; x++)
            {
                var address = 0b0100000000000000;

                address |= (y & 0b00000111) << 8;

                address |= (y & 0b11000000) << 11;

                address |= (y & 0b00111000) << 2;

                address |= x;

                var segment = _ram[address];

                for (var b = 0; b < 8; b++)
                {
                    if ((segment & (0x01 << b)) > 0)
                    {
                        data[i] = Color.Black;
                    }
                    else
                    {
                        data[i] = Color.White;
                    }
                }

                i++;
            }
        }

        texture.SetData(data);

        return texture;
    }
}