namespace ZXE.Core.Tests.System;

public class MotherboardTests
{
    //[Fact]
    //public void Dumps_assembly_correctly()
    //{
    //    var motherboard = new Motherboard(Model.Spectrum48K);

    //    motherboard.Load(new byte[]
    //                     {
    //                         0x00,                   // NOP
    //                         0x01, 0x34, 0x12,       // LD BC, 0x1234
    //                         0x02,                   // LD (BC), A
    //                         0x03,                   // INC BC
    //                         0x76                    // HALT
    //                     }, 0);

    //    var assembly = motherboard.DumpAssembly(0, 7);

    //    Assert.Equal("NOP", assembly[0]);
    //    Assert.Equal("LD BC, nn", assembly[1]);
    //    Assert.Equal("LD (BC), A", assembly[2]);
    //    Assert.Equal("INC BC", assembly[3]);
    //    Assert.Equal("HALT", assembly[4]);
    //}

    //[Fact]
    //public void Traces_routine_correctly()
    //{
    //    var motherboard = new Motherboard(Model.Spectrum48K);

    //    // TODO: Endienness confirmation
    //    motherboard.Load(new byte[] 
    //                     { 
    //                         // Something like:
    //                         // LD A, 0
    //                         // INC A
    //                         // JNE A, 5, -1
    //                         // RET NZ
    //                     }, 0);

    //    var assembly = motherboard.DumpAssembly(0, 6);

    //    Assert.Equal("NOP", assembly[0]);
    //}
}