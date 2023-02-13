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
        var texture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, 256, 192);

        var data = new Color[256 * 192];

        for (var i = 0; i < data.Length; i++)
        {
        }

        texture.SetData(data);

        return texture;
    }
}