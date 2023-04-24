using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ZXE.Desktop.Host.Infrastructure.Menu;

public class FileSelect
{
    public Texture2D Menu { get; private set; }

    public FileSelect(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, Action<MenuResult, object> menuFinished)
    {
    }
}