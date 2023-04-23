using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Common;

namespace ZXE.Windows.Host.Infrastructure;

public class MenuSystem
{
    private readonly Texture2D _background;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    public Texture2D Menu { get; private set; }

    public MenuSystem(Texture2D background, GraphicsDeviceManager graphicsDeviceManager)
    {
        _background = background;

        _graphicsDeviceManager = graphicsDeviceManager;
    }

    public void Update()
    {
        DrawMenu();
    }

    private void DrawMenu()
    {
        var data = new Color[Constants.ScreenWidthPixels * Constants.ScreenHeightPixels];

        _background.GetData(data);

        DrawWindow(data);

        var screen = new Texture2D(_graphicsDeviceManager.GraphicsDevice, Constants.ScreenWidthPixels, Constants.ScreenHeightPixels);

        screen.SetData(data);

        Menu = screen;
    }

    private static void DrawWindow(Color[] data)
    {
        for (var y = 16; y < 176; y++)
        {
            for (var x = 24; x < 232; x++)
            {
                data[y * Constants.ScreenWidthPixels + x] = Color.Black; //.Multiply(data[i], 0.5f);
            }
        }
    }
}