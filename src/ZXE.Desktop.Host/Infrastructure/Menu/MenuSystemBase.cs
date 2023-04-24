using Microsoft.Xna.Framework.Graphics;

namespace ZXE.Desktop.Host.Infrastructure.Menu;

public abstract class MenuSystemBase
{
    public abstract Texture2D Menu { get; }

    public abstract void Update();
}