using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Common;

namespace ZXE.Windows.Host.Infrastructure;

public class MenuSystem
{
    private readonly Texture2D _background;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private Texture2D _characterSet;

    public Texture2D Menu { get; private set; }

    public MenuSystem(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
    {
        _background = background;

        _graphicsDeviceManager = graphicsDeviceManager;

        _characterSet = contentManager.Load<Texture2D>("character-set");
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
                var i = y * Constants.ScreenWidthPixels + x;

                var color = data[i];

                color = y < 18 || y > 173 || x < 26 || x > 229 ? Color.White : Color.FromNonPremultiplied(color.R / 5, color.G / 5, color.B / 5, color.A);

                data[i] = color;
            }
        }
    }
}