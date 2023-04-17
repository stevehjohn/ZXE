//#define DELAY
// Use the above to pause boot to allow for recording.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Infrastructure;

namespace ZXE.Windows.Host.Display;

public class Monitor : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Motherboard _motherboard;

    private readonly VRamAdapter _vRamAdapter;

    private SpriteBatch _spriteBatch;

    private string _imageName = "Standard ROM";

#if DELAY
    private int _count = 0;
#endif

    public Monitor(Motherboard motherboard)
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

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        var keys = Keyboard.GetState().GetPressedKeys();

        var portData = KeyboardMapper.MapKeyState(keys);

        foreach (var port in portData)
        {
            _motherboard.Ports.WriteByte(port.Port, port.data);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F11))
        {
            _motherboard.SetTraceState(true);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F12))
        {
            _motherboard.SetTraceState(false);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F10))
        {
            _motherboard.Pause();

            var file = "..\\..\\..\\..\\..\\Game Images\\Dizzy\\image-0.z80";

            //var adapter = new SnaFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

            var adapter = new Z80FileLoader(_motherboard.Processor.State, _motherboard.Ram);

            adapter.Load(file);

            _imageName = file.Split('\\')[^2];

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F6))
        {
            _motherboard.Pause();

            var file = "..\\..\\..\\..\\..\\Other Images\\zexall-spectrum.com";

            _imageName = file.Split('\\')[^2];

            _motherboard.Ram.Load(File.ReadAllBytes(file), 0x8000);

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F5))
        {
            _motherboard.Pause();

            var file = "..\\..\\..\\..\\..\\Other Images\\memptr-flags-tests.sna";

            var adapter = new SnaFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

            adapter.Load(file);

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F9))
        {
            _motherboard.Fast = ! _motherboard.Fast;

            if (_motherboard.Fast)
            {
                Window.Title = "ZXE - Fast";
            }
            else
            {
                Window.Title = "ZXE";
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F8))
        {
            _motherboard.Pause();

            var adapter = new ZxeFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZXE Snapshots", $"{_imageName} {DateTime.Now:yyyy-MM-dd HH-mm}.zxe.json");

            // TODO: This is me-specific.
            path = path.Replace("OneDrive\\", string.Empty);

            adapter.Save(path, _imageName);

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F7))
        {
            _motherboard.Pause();

            _motherboard.Reset();

            var adapter = new ZxeFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZXE Snapshots");

            // TODO: This is me-specific.
            path = path.Replace("OneDrive\\", string.Empty);

            var directoryInfo = new DirectoryInfo(path);

            var file = directoryInfo.EnumerateFiles("*").MaxBy(f => f.CreationTimeUtc);

            if (file != null)
            {
                _imageName = adapter.Load(file.FullName);
            }

            _motherboard.Resume();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);

        // TODO: Call from motherboard at appropriate point.
        _vRamAdapter.RenderDisplay();

        var screen = _vRamAdapter.Display;

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _spriteBatch.Draw(screen, new Rectangle(0, 0, 256 * 4, 192 * 4), new Rectangle(0, 0, 256, 192), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);

        screen.Dispose();
    }
}