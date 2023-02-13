using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Core.System;

namespace ZXE.Windows.Host.Display
{
    public class Monitor : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private readonly Motherboard _motherboard;

        private SpriteBatch _spriteBatch;

        public Monitor(Motherboard motherboard)
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
                                     {
                                         PreferredBackBufferWidth = 256,
                                         PreferredBackBufferHeight = 192
                                     };

            Content.RootDirectory = "_Content";

            IsMouseVisible = true;

            _motherboard = motherboard;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}