//#define DELAY
// Use the above to pause boot to allow for recording.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZXE.Core.Infrastructure;
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

            case MenuResult.LoadZ80SNA:
                LoadZ80Sna();

                break;

            case MenuResult.SpeedNormal:
                _motherboard.Fast = false;

                break;

            case MenuResult.SpeedFast:
                _motherboard.Fast = true;

                break;

            case MenuResult.SaveState:
                SaveState();

                break;

            case MenuResult.LoadState:
                LoadState();

                break;
        }

        _motherboard.Resume();
    }

    private void LoadState()
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

    private void SaveState()
    {
        _motherboard.Pause();

        var adapter = new ZxeFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZXE Snapshots", $"{_imageName} {DateTime.Now:yyyy-MM-dd HH-mm}.zxe.json");

        adapter.Save(path, _imageName, _motherboard.Model);

        _motherboard.Resume();
    }
    
    private void LoadZ80Sna()
    {

        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Filter = "Z80 (*.z80)|*.z80|Snapshot files(*.sna)|*.sna"
        };

        var result = dialog.ShowDialog();

        if (result != DialogResult.OK)
        {
            _motherboard.Resume();

            return;
        }

        var filename = dialog.FileName;

        IImageLoader loader;

        switch (Path.GetExtension(filename).ToLowerInvariant())
        {
            case ".z80":
                loader = new Z80FileLoader(_motherboard.Processor.State, _motherboard.Ram, _motherboard.Model);

                break;
            case ".sna":
                loader = new SnaFileAdapter(_motherboard.Processor.State, _motherboard.Ram);

                break;
            default:
                // TODO: Proper extension
                throw new Exception("Unsupported file format");
        }

        loader.Load(dialog.FileName);

        _imageName = dialog.FileName.Split('\\')[^2];

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