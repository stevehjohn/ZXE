﻿using System;
using System.IO;
using System.Text.Json;
using ZXE.Core.Infrastructure;

namespace ZXE.Desktop.Host.Infrastructure.Settings;

public class AppSettings
{
    private const string SettingsFile = "app-settings.json";

    private static readonly Lazy<AppSettings> Lazy = new(GetAppSettings);

    public static AppSettings Instance => Lazy.Value;

    public string LastZ80SnaPath { get; set; }

    public Model SystemModel { get; set; }

    public int ScaleFactor { get; set; }

    private static AppSettings GetAppSettings()
    {
        var json = File.ReadAllText(SettingsFile);

        return JsonSerializer.Deserialize<AppSettings>(json);
    }

    public void Save()
    {
        File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this));
    }
}