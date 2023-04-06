//#define DELAY
// Use the above to pause boot to allow for recording.

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Core.System;
using ZXE.Windows.Host.Infrastructure;

namespace ZXE.Windows.Host.Display;

public class Monitor : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Motherboard _motherboard;

    private readonly VRamAdapter _vRamAdapter;

    private SpriteBatch _spriteBatch;

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

            //if (port.data != 31)
            //{
            //    Debugger.Log(0, "INFO", $"{port.Port:X2}: {port.data}\n");
            //}
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
            //var loader = new Z80FileLoader(_motherboard.Processor.State, _motherboard.Ram);

            ////loader.Load("..\\..\\..\\..\\..\\Game Images\\Horace Goes Skiing\\image-0.z80");
            ////loader.Load("..\\..\\..\\..\\..\\Game Images\\Treasure Island Dizzy\\image-0.z80");
            ////loader.Load("..\\..\\..\\..\\..\\Game Images\\Jet Pac\\image-0.z80");
            ////loader.Load("..\\..\\..\\..\\..\\Game Images\\Manic Miner\\image-0.z80");
            ////loader.Load("..\\..\\..\\..\\..\\Game Images\\Head Over Heels\\image-0.z80");
            ////loader.Load("..\\..\\..\\..\\..\\Other Images\\snaptest.v1.z80");
            //loader.Load("..\\..\\..\\..\\..\\Other Images\\snaptest.v1.z80");

            var data = File.ReadAllBytes("..\\..\\..\\..\\..\\Other Images\\zexdoc-spectrum.com");

            //_motherboard.Processor.Reset();

            _motherboard.Ram.Load(data, 0x8000);

            //_motherboard.Processor.State.ProgramCounter = 0x8000;
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);

        var screen = _vRamAdapter.GetDisplay();

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _spriteBatch.Draw(screen, new Rectangle(0, 0, 256 * 4, 192 * 4), new Rectangle(0, 0, 256, 192), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);

        screen.Dispose();
    }
}