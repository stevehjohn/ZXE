using System.Collections.Generic;

namespace ZXE.Windows.Host.Infrastructure.Menu;

public abstract class MenuBase
{
    public abstract List<Label> GetMenu();

    public abstract (MenuResult Result, MenuBase NewMenu, object Arguments) ItemSelected(int id);
}