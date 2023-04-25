using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ZXE.Desktop.Host.Infrastructure.Menu;

public class ScaleMenu : MenuBase
{
    public override List<Label> GetMenu()
    {
        var items = new List<Label>
                    {
                        new(0, true, "ZXE - Display Scale", Color.White, 0, 0, null),
                        new(1, false, "[1] 2x", Color.Yellow, 1, 3, Keys.D1, Color.LightGreen),
                        new(2, false, "[2] 4x", Color.Yellow, 1, 5, Keys.D2, Color.LightGreen),
                        new(2, false, "[3] 6x", Color.Yellow, 1, 7, Keys.D3, Color.LightGreen),
                        new(2, false, "[4] 8x", Color.Yellow, 1, 9, Keys.D4, Color.LightGreen),
                        new(99, true, "[ESC] Close Menu", Color.FromNonPremultiplied(255, 64, 64, 255), 0, 16, Keys.Escape, Color.LightGreen)
                    };

        return items;
    }

    public override (MenuResult Result, MenuBase NewMenu, object Arguments) ItemSelected(int id)
    {
        throw new System.NotImplementedException();
    }
}