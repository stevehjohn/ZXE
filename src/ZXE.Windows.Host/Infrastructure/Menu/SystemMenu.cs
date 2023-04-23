﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public class SystemMenu : MenuBase
{
    public override List<Label> GetMenu()
    {
        var items = new List<Label>
                    {
                        new(true, "ZXE - Select System", Color.White, 0, 0, null),
                        new(false, "[1] Spectrum 48K", Color.Yellow, 1, 3, Keys.D1, Color.Green),
                        new(true, "[ESC] Close Menu", Color.FromNonPremultiplied(255, 64, 64, 255), 0, 16, Keys.Escape)
                    };

        return items;
    }
}