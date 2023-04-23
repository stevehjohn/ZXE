﻿//#define DELAY
// Use the above to pause boot to allow for recording.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;
using ZXE.Windows.Host.Display;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
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

            var dialog = new OpenFileDialog
            {
                DefaultExt = "z80"
            };

            var result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                _motherboard.Resume();

                return;
            }

            var adapter = new Z80FileLoader(_motherboard.Processor.State, _motherboard.Ram, _motherboard.Model);

            adapter.Load(dialog.FileName);

            Thread.Sleep(250); // Prevent multiple loads, hopefully.

            _imageName = dialog.FileName.Split('\\')[^2];

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

            var file = "..\\..\\..\\..\\..\\Other Images\\memtest+3.z80";

            var adapter = new Z80FileLoader(_motherboard.Processor.State, _motherboard.Ram, _motherboard.Model);

            adapter.Load(file);

            _motherboard.Reset();

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F9))
        {
            _motherboard.Fast = !_motherboard.Fast;

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

            adapter.Save(path, _imageName);

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F7))
        {
            _motherboard.Pause();

            _motherboard.Reset();

            var adapter = new ZxeFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZXE Snapshots");

            var directoryInfo = new DirectoryInfo(path);

            var file = directoryInfo.EnumerateFiles("*").MaxBy(f => f.CreationTimeUtc);

            if (file != null)
            {
                _imageName = adapter.Load(file.FullName);
            }

            _motherboard.Resume();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F1))
        {
            _motherboard.Pause();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.F2))
        {
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