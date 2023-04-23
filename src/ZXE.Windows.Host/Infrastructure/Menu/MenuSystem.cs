using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Common;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public class MenuSystem
{
    private const int ColorDecrement = 20;

    private const int SelectionFrameDelay = 24;

    private readonly Texture2D _background;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Color[] _characterSet;

    private readonly Dictionary<char, int> _characterMap = new();

    private readonly Action _menuFinished;

    private int _colorOffset;

    private int _colorFrame;

    private int _selectedItem = -1;

    private int _selectionDelay = SelectionFrameDelay;

    private MenuBase _menu;

    public Texture2D Menu { get; private set; }

    public MenuSystem(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, Action menuFinished)
    {
        _background = background;

        _graphicsDeviceManager = graphicsDeviceManager;

        _menuFinished = menuFinished;

        var characterSet = contentManager.Load<Texture2D>("character-set");

        _characterSet = new Color[7168];

        characterSet.GetData(_characterSet);

        InitialiseCharacterMap();

        _menu = new MainMenu();
    }

    public void Update()
    {
        DrawMenu();

        UpdateTextAnimation();

        CheckSelection();

        ActionSelection();
    }

    public void ActionSelection()
    {
        if (_selectedItem == -1)
        {
            return;
        }

        _selectionDelay--;

        if (_selectionDelay > 0)
        {
            return;
        }

        var result = _menu.ItemSelected(_selectedItem);

        switch (result.Result)
        {
            case MenuResult.NewMenu:
                _menu = result.NewMenu;

                _selectedItem = -1;

                _selectionDelay = SelectionFrameDelay;

                break;
            case MenuResult.Exit:
                _menuFinished();

                break;
        }
    }

    private void CheckSelection()
    {
        if (_selectedItem > -1)
        {
            return;
        }

        var keys = Keyboard.GetState();

        foreach (var item in _menu.GetMenu())
        {
            if (item.Id == 0 || ! item.SelectKey.HasValue)
            {
                continue;
            }

            if (keys.IsKeyDown(item.SelectKey!.Value))
            {
                _selectedItem = item.Id;

                break;
            }
        }
    }

    private void UpdateTextAnimation()
    {
        _colorFrame++;

        if (_colorFrame < 5)
        {
            return;
        }

        _colorFrame = 0;

        _colorOffset--;

        if (_colorOffset < 0)
        {
            _colorOffset = 7;
        }
    }

    private void DrawMenu()
    {
        var data = new Color[Constants.ScreenWidthPixels * Constants.ScreenHeightPixels];

        _background.GetData(data);

        DrawWindow(data);

        var items = _menu.GetMenu();

        foreach (var item in items)
        {
            DrawString(data, item.Text, item.X, item.Y, _selectedItem == item.Id ? item.SelectedColor!.Value : item.Color, item.Centered);
        }

        //DrawString(data, "[2] Load Z80/SNA File", 1, 5, Color.FromNonPremultiplied(80, 80, 80, 255));

        //DrawString(data, "[3] Save System State", 1, 7, Color.FromNonPremultiplied(80, 80, 80, 255));

        //DrawString(data, "[4] Load System State", 1, 9, Color.FromNonPremultiplied(80, 80, 80, 255));

        //DrawString(data, "[5] Emulation Speed", 1, 11, Color.FromNonPremultiplied(80, 80, 80, 255));

        //DrawString(data, "[6] Debugging Options", 1, 13, Color.FromNonPremultiplied(80, 80, 80, 255));

        //DrawString(data, "[ESC] Close Menu", 4, 16, Color.FromNonPremultiplied(255, 64, 64, 255));

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

    private void DrawString(Color[] data, string text, int x, int y, Color color, bool centered)
    {
        var xOffset = 0;

        if (centered)
        {
            x = 12 - text.Length / 2;

            if (text.Length % 2 == 1)
            {
                xOffset = -4;
            }
        }

        for (var i = 0; i < text.Length; i++)
        {
            DrawMenuCharacter(data, text[i], x + i, y, color, xOffset);
        }
    }

    private void DrawMenuCharacter(Color[] data, char character, int x, int y, Color color, int xOffset)
    {
        var co = _characterMap[character];

        var offset = _colorOffset;

        for (var iy = 0; iy < 8; iy++)
        {
            var decrement = ColorDecrement * offset;

            var textColor = Color.FromNonPremultiplied(color.R - decrement, color.G - decrement, color.B - decrement, color.A);

            offset++;

            if (offset > 7)
            {
                offset = 0;
            }

            for (var ix = 0; ix < 8; ix++)
            {
                if (_characterSet[iy * 128 + ix + co].A == 0)
                {
                    continue;
                }

                data[(3 + y) * 2048 + (x + 4) * 8 + ix + iy * 256 + xOffset] = textColor;
            }
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

        _characterMap.Add('£', 4096);
        _characterMap.Add('a', 4104);
        _characterMap.Add('b', 4112);
        _characterMap.Add('c', 4120);
        _characterMap.Add('d', 4128);
        _characterMap.Add('e', 4136);
        _characterMap.Add('f', 4144);
        _characterMap.Add('g', 4152);
        _characterMap.Add('h', 4160);
        _characterMap.Add('i', 4168);
        _characterMap.Add('j', 4176);
        _characterMap.Add('k', 4184);
        _characterMap.Add('l', 4192);
        _characterMap.Add('m', 4200);
        _characterMap.Add('n', 4208);
        _characterMap.Add('o', 4216);

        _characterMap.Add('p', 5120);
        _characterMap.Add('q', 5128);
        _characterMap.Add('r', 5136);
        _characterMap.Add('s', 5144);
        _characterMap.Add('t', 5152);
        _characterMap.Add('u', 5160);
        _characterMap.Add('v', 5168);
        _characterMap.Add('w', 5176);
        _characterMap.Add('x', 5184);
        _characterMap.Add('y', 5192);
        _characterMap.Add('z', 5200);
        _characterMap.Add('{', 5208);
        _characterMap.Add('|', 5216);
        _characterMap.Add('}', 5224);
        _characterMap.Add('~', 5232);
        _characterMap.Add('©', 5240);
    }
}