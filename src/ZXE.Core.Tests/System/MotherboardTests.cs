using Xunit;
using ZXE.Core.Infrastructure;
using ZXE.Core.System;

namespace ZXE.Core.Tests.System;

public class MotherboardTests
{
    [Fact]
    public void Dumps_assembly_correctly()
    {
        var motherboard = new Motherboard(Model.Spectrum48K);

        // TODO: Endienness confirmation
        motherboard.Load(new byte[] 
                         { 
                             0x00,                   // NOP
                             0x01, 0x03, 0xFF,       // LD BC, 0x03FF
                             0x02,                   // LD (BC), A
                             0x03                    // INC BC
                         }, 0);

        var assembly = motherboard.DumpAssembly(0, 6);

        Assert.Equal("NOP", assembly[0]);
    }

    [Fact]
    public void Traces_routine_correctly()
    {
        var motherboard = new Motherboard(Model.Spectrum48K);

        // TODO: Endienness confirmation
        motherboard.Load(new byte[] 
                         { 
                             0x00,                   // NOP
                             0x01, 0x03, 0xFF,       // LD BC, 0x03FF
                             0x02,                   // LD (BC), A
                             0x03                    // INC BC
                         }, 0);

        var assembly = motherboard.DumpAssembly(0, 6);

        Assert.Equal("NOP", assembly[0]);
    }
}