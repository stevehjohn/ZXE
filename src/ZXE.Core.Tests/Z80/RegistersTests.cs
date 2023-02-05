using Xunit;
using ZXE.Core.Exceptions;
using ZXE.Core.Z80;

namespace ZXE.Core.Tests.Z80;

public class RegistersTests
{
    [Fact]
    public void ReadPair_throws_when_passed_a_single_register()
    {
        var registers = new Registers();

        Assert.Throws<RegisterAccessException>(() => registers.ReadPair(Register.A));
    }

    [Fact]
    public void WritePair_throws_when_passed_a_single_register()
    {
        var registers = new Registers();

        Assert.Throws<RegisterAccessException>(() => registers.WritePair(Register.A, 123));
    }
}