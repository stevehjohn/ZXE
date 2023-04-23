using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZXE.Common;

namespace ZXE.Windows.Host.Infrastructure;

public class MenuSystem
{
    private const int ColorDecrement = 20;

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

        DrawString(data, " !\"#$%&'()*+,-./", 1, 1);
        DrawString(data, "0123456789:;<]>?", 1, 2);
        DrawString(data, "@ABCDEFGHIJKLMNO", 1, 3);
        DrawString(data, "PQRSTUVWXYZ[\\]^_", 1, 4);

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

    private void DrawString(Color[] data, string text, int x, int y)
    {
        for (var i = 0; i < text.Length; i++)
        {
            DrawMenuCharacter(data, text[i], x + i, y);
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

            textColor = Color.FromNonPremultiplied(textColor.R - ColorDecrement, textColor.G - ColorDecrement, textColor.B - ColorDecrement, textColor.A);
        }
    }

    private void InitialiseCharacterMap()
    {
        _characterMap.Add(' ', 0);
        _characterMap.Add('!', 8);
        _characterMap.Add('"', 16);
        _characterMap.Add('#', 24);
        _characterMap.Add('$', 32);
        _characterMap.Add('%', 40);
        _characterMap.Add('&', 48);
        _characterMap.Add('\'', 56);
        _characterMap.Add('(', 64);
        _characterMap.Add(')', 72);
        _characterMap.Add('*', 80);
        _characterMap.Add('+', 88);
        _characterMap.Add(',', 96);
        _characterMap.Add('-', 104);
        _characterMap.Add('.', 112);
        _characterMap.Add('/', 120);

        _characterMap.Add('0', 1024);
        _characterMap.Add('1', 1032);
        _characterMap.Add('2', 1040);
        _characterMap.Add('3', 1048);
        _characterMap.Add('4', 1056);
        _characterMap.Add('5', 1064);
        _characterMap.Add('6', 1072);
        _characterMap.Add('7', 1080);
        _characterMap.Add('8', 1088);
        _characterMap.Add('9', 1096);
        _characterMap.Add(':', 1104);
        _characterMap.Add(';', 1112);
        _characterMap.Add('<', 1120);
        _characterMap.Add('=', 1128);
        _characterMap.Add('>', 1136);
        _characterMap.Add('?', 1144);

        _characterMap.Add('@', 2048);
        _characterMap.Add('A', 2056);
        _characterMap.Add('B', 2064);
        _characterMap.Add('C', 2072);
        _characterMap.Add('D', 2080);
        _characterMap.Add('E', 2088);
        _characterMap.Add('F', 2096);
        _characterMap.Add('G', 2104);
        _characterMap.Add('H', 2112);
        _characterMap.Add('I', 2120);
        _characterMap.Add('J', 2128);
        _characterMap.Add('K', 2136);
        _characterMap.Add('L', 2144);
        _characterMap.Add('M', 2152);
        _characterMap.Add('N', 2160);
        _characterMap.Add('O', 2168);

        _characterMap.Add('P', 3072);
        _characterMap.Add('Q', 3080);
        _characterMap.Add('R', 3088);
        _characterMap.Add('S', 3096);
        _characterMap.Add('T', 3104);
        _characterMap.Add('U', 3112);
        _characterMap.Add('V', 3120);
        _characterMap.Add('W', 3128);
        _characterMap.Add('X', 3136);
        _characterMap.Add('Y', 3144);
        _characterMap.Add('Z', 3152);
        _characterMap.Add('[', 3160);
        _characterMap.Add('\\', 3168);
        _characterMap.Add(']', 3176);
        _characterMap.Add('^', 3184);
        _characterMap.Add('_', 3192);
    }
}