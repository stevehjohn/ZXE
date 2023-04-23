﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public class Label
{
    public bool Centered { get; private set; }

    public string Text { get; private set; }

    public Color Color { get; private set; }

    public Color? SelectedColor { get; private set; }

    public int X { get; private set; }

    public int Y { get; private set; }

    public Keys? SelectKey { get; private set; }

    public Label(bool centered, string text, Color color, int x, int y, Keys? selectKey, Color? selectedColor = null)
    {
        Centered = centered;

        Text = text;

        Color = color;

        X = x;

        Y = y;

        SelectKey = selectKey;

        SelectedColor = selectedColor;
    }
}