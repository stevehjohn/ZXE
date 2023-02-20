// #define DELAY
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

        if (keys.Length > 0)
        {
            var portData = KeyboardMapper.MapKeyState(keys);

            foreach (var port in portData)
            {
                _motherboard.Ports.WriteByte(port.Port, port.data);
            }
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
            //var data = File.ReadAllBytes("..\\..\\..\\..\\..\\Game Images\\Treasure Island Dizzy\\load-screen.bin");

            //_motherboard.LoadData(data, 0x4000);

            var data = File.ReadAllBytes("..\\..\\..\\..\\..\\Game Images\\Dizzy\\image-0.z80");

            _motherboard.LoadData(data[0xB7..], 0x4000);

            _motherboard.Processor.State.OpcodePrefix = 0;

            _motherboard.Processor.State.ProgramCounter = 0x5B00;
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