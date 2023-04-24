using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text.Json;

namespace ZXE.Desktop.Host.Infrastructure.Settings;

public class AppSettings
{
    private static readonly Lazy<AppSettings> Lazy = new(GetAppSettings);

    public static AppSettings Instance => Lazy.Value;

    public string LastZ80SnaPath { get; set; }

    public Model SystemModel { get; set; }

    private static AppSettings GetAppSettings()
    {
        var json = File.ReadAllText("app-settings.json");

        return JsonSerializer.Deserialize<AppSettings>(json);
    }
}