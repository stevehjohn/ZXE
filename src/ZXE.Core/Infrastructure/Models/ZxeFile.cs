using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Models;

public class ZxeFile
{
    public Model Model { get; set; }

    public State? State { get; set; }

    public Dictionary<int, int> PageConfiguration { get; set; } = new();

    public Dictionary<int, byte[]> RamBanks { get; set; } = new();

    public Dictionary<string, ushort> Registers { get; set; } = new();

    public byte[]? Rom { get; set; }
        
    public int RomNumber { get; set; }

    public string? RomTitle { get; set; }
}