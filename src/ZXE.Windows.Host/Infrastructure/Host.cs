//#define DELAY
// Use the above to pause boot to allow for recording.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;
using ZXE.Windows.Host.Infrastructure.Menu;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ZXE.Windows.Host.Infrastructure;

public class Host : Game
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Motherboard _motherboard;

    private readonly VRamAdapter _vRamAdapter;

    private SpriteBatch _spriteBatch;

    private string _imageName = "Standard ROM";

#if DELAY
    private int _count = 0;
#endif

    private MenuSystem _menuSystem;

    public Host(Motherboard motherboard)
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 256 * 4,
            PreferredBackBufferHeight = 192 * 4
        };

        Content.RootDirectory = "_Content";

        IsMouseVisible = true;

        _motherboard = motherboard;

        _vRamAdapter = new VRamAdapter(_motherboard.Ram, _graphicsDeviceManager);

#if ! DELAY
        _motherboard.Start();
#endif

        Content.RootDirectory = "_Content";
    }

    protected override void Initialize()
    {
        Window.Title = "ZXE";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
#if DELAY
        _count++;

        if (_count == 400)
        {
            _motherboard.Start();
        }
#endif

        var keys = Keyboard.GetState().GetPressedKeys();

        var portData = KeyboardMapper.MapKeyState(keys);

        foreach (var port in portData)
        {
            _motherboard.Ports.WriteByte(port.Port, port.data);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F10) && _menuSystem == null)
        {
            _motherboard.Pause();

            _vRamAdapter.RenderDisplay();
            
            var screen = _vRamAdapter.Display;

            _menuSystem = new MenuSystem(screen, _graphicsDeviceManager, Content, MenuFinished);
        }

        if (_menuSystem != null)
        {
            _menuSystem.Update();
        }

        base.Update(gameTime);
    }

    private void MenuFinished()
    {
        _menuSystem = null;

        _motherboard.Resume();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);

        Texture2D screen;

        if (_menuSystem != null)
        {
            screen = _menuSystem.Menu;
        }
        else
        {
            // TODO: Call from motherboard at appropriate point.
            _vRamAdapter.RenderDisplay();
            
            screen = _vRamAdapter.Display;
        }

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _spriteBatch.Draw(screen, new Rectangle(0, 0, 256 * 4, 192 * 4), new Rectangle(0, 0, 256, 192), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);

        screen.Dispose();
    }
}