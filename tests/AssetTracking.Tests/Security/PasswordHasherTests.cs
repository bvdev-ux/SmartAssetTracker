using AssetTracking.Infrastructure.Services;
using Xunit;

namespace AssetTracking.Tests.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ThenVerify_WithCorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        var result = _hasher.Verify("Sup3rSecret!", hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        var result = _hasher.Verify("WrongPassword", hash);

        Assert.False(result);
    }

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()
    {
        var first = _hasher.Hash("Sup3rSecret!");
        var second = _hasher.Hash("Sup3rSecret!");

        Assert.NotEqual(first, second);
    }
}
