using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Models;

public class ZxeFile
{
    public State? State { get; set; }

    public Dictionary<int, int> PageConfiguration { get; set; } = new();

    public Dictionary<int, byte[]> RamBanks { get; set; } = new();

    public Dictionary<string, ushort> Registers { get; set; } = new();

    public byte[]? Rom { get; set; }

    public string? RomTitle { get; set; }
}