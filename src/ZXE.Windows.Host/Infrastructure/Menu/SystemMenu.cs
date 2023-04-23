using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public class SystemMenu : MenuBase
{
    public override List<Label> GetMenu()
    {
        var items = new List<Label>
                    {
                        new(0, true, "ZXE - Select System", Color.White, 0, 0, null),
                        new(1, false, "[1] Spectrum 48K", Color.Yellow, 1, 3, Keys.D1, Color.LightGreen),
                        new(2, false, "[2] Spectrum 128", Color.Yellow, 1, 5, Keys.D2, Color.LightGreen),
                        new(3, false, "[3] Spectrum +2", Color.Yellow, 1, 7, Keys.D3, Color.LightGreen),
                        new(4, false, "[4] Spectrum +3", Color.Yellow, 1, 9, Keys.D4, Color.LightGreen),
                        new(5, true, "[ESC] Close Menu", Color.FromNonPremultiplied(255, 64, 64, 255), 0, 16, Keys.Escape, Color.LightGreen)
                    };

        return items;
    }

    public override MenuBase ItemSelected(int id)
    {
        return null;
    }
}