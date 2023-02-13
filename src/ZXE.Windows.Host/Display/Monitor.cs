using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZXE.Core.System;

namespace ZXE.Windows.Host.Display
{
    public class Monitor : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private readonly Motherboard _motherboard;

        private SpriteBatch _spriteBatch;

        public Monitor(Motherboard motherboard)
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "_Content";

            IsMouseVisible = true;

            _motherboard = motherboard;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}