using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Common;

namespace ZXE.Windows.Host.Infrastructure;

public class MenuSystem
{
    private readonly Texture2D _background;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Color[] _characterSet;

    private readonly Dictionary<char, int> _characterMap = new();

    public Texture2D Menu { get; private set; }

    public MenuSystem(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
    {
        _background = background;

        _graphicsDeviceManager = graphicsDeviceManager;

        var characterSet = contentManager.Load<Texture2D>("character-set");

        _characterSet = new Color[7168];

        characterSet.GetData(_characterSet);

        InitialiseCharacterMap();
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

        DrawMenuCharacter(data, 'A', 1, 1);

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

    private void DrawMenuCharacter(Color[] data, char character, int x, int y)
    {
        var co = _characterMap[character];

        var textColor = Color.White;

        for (var iy = 0; iy < 8; iy++)
        {
            for (var ix = 0; ix < 8; ix++)
            {
                var color = _characterSet[iy * 128 + ix + co];

                if (color.A == 0)
                {
                    continue;
                }

                data[(3 + y) * 2048 + (x + 4) * 8 + ix + iy * 256] = textColor;
            }

            textColor = Color.FromNonPremultiplied(textColor.R - 20, textColor.G - 20, textColor.B - 20, textColor.A);
        }
    }

    private void InitialiseCharacterMap()
    {
        _characterMap.Add('@', 2048);
        _characterMap.Add('A', 2056);
    }
}