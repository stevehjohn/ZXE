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
                        new(2, false, "[2] Load Z80/SNA File", Color.Yellow, 1, 5, Keys.D2, Color.LightGreen),
                        new(3, false, "[3] Load/Save State", Color.Yellow, 1, 7, Keys.D3, Color.LightGreen),
                        new(4, false, "[4] Emulator Speed", Color.Yellow, 1, 9, Keys.D4, Color.LightGreen),
                        new(99, true, "[ESC] Close Menu", Color.FromNonPremultiplied(255, 64, 64, 255), 0, 16, Keys.Escape, Color.LightGreen)
                    };

        return items;
    }

    public override (MenuResult Result, MenuBase NewMenu, object Arguments) ItemSelected(int id)
    {
        switch (id)
        {
            case 1:
                return (MenuResult.NewMenu, new SystemMenu(), null);

            case 2:
                return (MenuResult.LoadZ80SNA, null, null);

            case 3:
                return (MenuResult.NewMenu, new StateMenu(), null);

            case 4:
                return (MenuResult.NewMenu, new SpeedMenu(), null);

            default:
                return (MenuResult.Exit, null, null);
        }
    }
}