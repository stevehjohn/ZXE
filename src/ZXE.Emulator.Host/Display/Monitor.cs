namespace ZXE.Emulator.Host.Display;

public class Monitor : Game
{
    GraphicsDeviceManager graphics;

    SpriteBatch spriteBatch;

    public Monitor()
    {
        graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "_Content";
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
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
