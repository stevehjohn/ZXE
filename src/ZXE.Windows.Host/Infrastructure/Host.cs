//#define DELAY
// Use the above to pause boot to allow for recording.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;
using ZXE.Windows.Host.Infrastructure.Menu;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Model = ZXE.Core.Infrastructure.Model;

namespace ZXE.Windows.Host.Infrastructure;

public class Host : Game
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private VRamAdapter _vRamAdapter;

    private Motherboard _motherboard;

    private SpriteBatch _spriteBatch;

    private string _imageName = "Standard ROM";

#if DELAY
    private int _count = 0;
#endif

    private MenuSystem _menuSystem;

    public Host()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 256 * 4,
            PreferredBackBufferHeight = 192 * 4
        };

        Content.RootDirectory = "_Content";

        IsMouseVisible = true;

        SetMotherboard(Model.SpectrumPlus3);
    }

    private void SetMotherboard(Model model)
    {
        _motherboard = new Motherboard(model);

        _vRamAdapter = new VRamAdapter(_motherboard.Ram, _graphicsDeviceManager);

#if ! DELAY
        _motherboard.Start();
#endif
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

    private void MenuFinished(MenuResult result, object arguments)
    {
        _menuSystem = null;

        switch (result)
        {
            case MenuResult.Restart:
                SetMotherboard((Model) arguments);

                return;
        }

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