using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure.Models;

public class ZxeFile
{
    public State? State { get; set; }

    public byte[]? Ram { get; set; }

    public byte[]? Registers { get; set; }

    public string? RomTitle { get; set; }
}