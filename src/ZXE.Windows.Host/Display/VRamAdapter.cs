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

        var address = 0x4000;

        for (var i = 0; i < data.Length; i++)
        {
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

            address++;
        }

        texture.SetData(data);

        return texture;
    }
}