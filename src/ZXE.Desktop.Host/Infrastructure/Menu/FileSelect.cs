using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZXE.Desktop.Host.Infrastructure.Menu;

public class FileSelect : CharacterOverlayBase
{
    public FileSelect(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        : base(background, graphicsDeviceManager, contentManager)
    {
    }
}