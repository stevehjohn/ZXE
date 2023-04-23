using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public class MainMenu : MenuBase
{
    public override List<Label> GetMenu()
    {
        var items = new List<Label>
                    {
                        new(0, true, "ZXE - Menu", Color.White, 0, 0, null),
                        new(1, false, "[1] Select System", Color.Yellow, 1, 3, Keys.D1, Color.LightGreen),
                        new(2, true, "[ESC] Close Menu", Color.FromNonPremultiplied(255, 64, 64, 255), 0, 16, Keys.Escape)
                    };

        return items;
    }

    public override MenuBase ItemSelected(int id)
    {
        switch (id)
        {
            case 1:
                return new SystemMenu();

            default:
                // TODO: Proper exception?
                throw new Exception("Invalid item");
        }
    }
}