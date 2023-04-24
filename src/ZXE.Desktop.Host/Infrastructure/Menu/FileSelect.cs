using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZXE.Common;

namespace ZXE.Desktop.Host.Infrastructure.Menu;

public class FileSelect : CharacterOverlayBase
{
    private const int SelectDelayFramesFast = 5;

    private const int SelectDelayFramesSlow = 10;

    private const int SelectDelayFramesVerySlow = 24;

    private const int FileRows = 12;

    private string _path;

    private readonly List<(string FullPath, string Display, bool IsDirectory)> _files = new();

    private int _selected;

    private int _selectDelay;

    private bool _cancelled;

    private bool _fileSelected;

    private readonly Action<string> _menuDone;

    public FileSelect(Texture2D background, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, Action<string> menuDone)
        : base(background, graphicsDeviceManager, contentManager)
    {
        _path = Directory.GetCurrentDirectory();

        GetFiles();

        _menuDone = menuDone;
    }

    public void Update()
    {
        DrawFileSelect();

        UpdateTextAnimation();

        if (! _cancelled && ! _fileSelected)
        {
            CheckKeys();
        }
        else
        {
            _selectDelay--;

            if (_selectDelay == 0)
            {
                _menuDone(_cancelled ? null : _files[_selected].FullPath);
            }
        }
    }

    private void CheckKeys()
    {
        if (_selectDelay != 0)
        {
            _selectDelay--;

            return;
        }

        var keys = Keyboard.GetState();

        if (keys.IsKeyDown(Keys.Up) && _selected > 0)
        {
            _selected--;

            _selectDelay = SelectDelayFramesFast;

            return;
        }

        if (keys.IsKeyDown(Keys.Down) && _selected < _files.Count - 1)
        {
            _selected++;

            _selectDelay = SelectDelayFramesFast;

            return;
        }

        if (keys.IsKeyDown(Keys.Enter))
        {
            if (_files[_selected].IsDirectory)
            {
                _path = _files[_selected].FullPath;

                _selected = 0;

                _selectDelay = SelectDelayFramesSlow;

                GetFiles();
            }
            else
            {
                _selectDelay = SelectDelayFramesVerySlow;

                _fileSelected = true;
            }
        }

        if (keys.IsKeyDown(Keys.Escape))
        {
            _selectDelay = SelectDelayFramesVerySlow;

            _cancelled = true;
        }
    }

    private void DrawFileSelect()
    {
        var data = new Color[Constants.ScreenWidthPixels * Constants.ScreenHeightPixels];

        Background.GetData(data);

        DrawWindow(data);

        DrawStaticItems(data);

        DrawFileList(data);

        var screen = new Texture2D(GraphicsDeviceManager.GraphicsDevice, Constants.ScreenWidthPixels, Constants.ScreenHeightPixels);

        screen.SetData(data);

        Menu = screen;
    }

    private void GetFiles()
    {
        _files.Clear();

        if (string.IsNullOrWhiteSpace(_path))
        {
            var drives = Directory.GetLogicalDrives().ToList();

            drives.ForEach(d => _files.Add((d, d, true)));

            return;
        }

        var parent = Directory.GetParent(_path);

        if (parent != null)
        {
            _files.Add((parent.FullName, "..", true));
        }
        else
        {
            _files.Add((string.Empty, "..", true));
        }

        var directories = Directory.EnumerateDirectories(_path).OrderBy(d => d).ToList();

        directories.ForEach(d => _files.Add((d, Path.GetFileName(d), true)));

        var files = Directory.EnumerateFiles(_path).Where(f => Path.GetExtension(f).ToLowerInvariant() is ".z80" or ".sna").OrderBy(d => d).ToList();

        files.ForEach(f => _files.Add((f, Path.GetFileName(f), false)));
    }

    private void DrawFileList(Color[] data)
    {
        var i = 0;

        var y = 0;

        if (_selected >= FileRows)
        {
            i = _selected - FileRows + 1;
        }

        while (i < _files.Count && y < FileRows)
        {
            DrawString(data, TruncateFileName(_files[i].Display), 0, y + 3, _fileSelected ? Color.Yellow : Color.LightGreen, false, i == _selected);

            i++;

            y++;
        }
    }

    private static string TruncateFileName(string filename)
    {
        if (filename.Length < 25)
        {
            return filename;
        }

        return $"{filename[..17]}..{Path.GetExtension(filename)}";
    }

    private void DrawStaticItems(Color[] data)
    {
        DrawString(data, "ZXE - Load Z80/SNA", 0, 0, Color.White, true);

        DrawString(data, "[ESC] Close Menu", 0, 17, _cancelled ? Color.LightGreen : Color.FromNonPremultiplied(255, 64, 64, 255), true);

        for (var y = 38; y < 40; y++)
        {
            for (var x = 24; x < 232; x++)
            {
                data[y * Constants.ScreenWidthPixels + x] = Color.White;

                data[(y + 114) * Constants.ScreenWidthPixels + x] = Color.White;
            }
        }
    }
}