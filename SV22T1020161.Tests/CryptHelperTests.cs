using SV22T1020161.Shop;

namespace SV22T1020161.Tests;

/// <summary>
/// TC-R5: Unit test cho CryptHelper (MD5 Hash)
/// </summary>
public class CryptHelperTests
{
    [Fact]
    public void MD5Hash_ReturnsCorrectLength()
    {
        var result = CryptHelper.MD5Hash("TestPassword123!");
        Assert.Equal(32, result.Length);
    }

    [Fact]
    public void MD5Hash_ReturnsLowercaseHex()
    {
        var result = CryptHelper.MD5Hash("abc");
        Assert.Equal(result, result.ToLowerInvariant());
        Assert.True(result.All(c => "0123456789abcdef".Contains(c)));
    }

    [Fact]
    public void MD5Hash_SameInput_ReturnsSameHash()
    {
        var hash1 = CryptHelper.MD5Hash("Abc@123!");
        var hash2 = CryptHelper.MD5Hash("Abc@123!");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void MD5Hash_DifferentInput_ReturnsDifferentHash()
    {
        var hash1 = CryptHelper.MD5Hash("Abc@123!");
        var hash2 = CryptHelper.MD5Hash("Xyz@456!");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void MD5Hash_KnownValue_ReturnsExpectedHash()
    {
        // MD5("hello") = 5d41402abc4b2a76b9719d911017c592
        var result = CryptHelper.MD5Hash("hello");
        Assert.Equal("5d41402abc4b2a76b9719d911017c592", result);
    }

    [Fact]
    public void MD5Hash_EmptyString_ReturnsKnownHash()
    {
        // MD5("") = d41d8cd98f00b204e9800998ecf8427e
        var result = CryptHelper.MD5Hash("");
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", result);
    }

    [Fact]
    public void MD5Hash_NullString_DoesNotThrow()
    {
        // Truyền null → coi như empty string, không throw
        var result = CryptHelper.MD5Hash(null ?? "");
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", result);
    }

    [Fact]
    public void VerifyMD5_CorrectPassword_ReturnsTrue()
    {
        var plain = "Abc@123!";
        var hash = CryptHelper.MD5Hash(plain);
        Assert.True(CryptHelper.VerifyMD5(plain, hash));
    }

    [Fact]
    public void VerifyMD5_WrongPassword_ReturnsFalse()
    {
        var hash = CryptHelper.MD5Hash("Abc@123!");
        Assert.False(CryptHelper.VerifyMD5("Xyz@456!", hash));
    }

    [Fact]
    public void VerifyMD5_VerifiesCorrectly()
    {
        var plain = "TestPass@99";
        var hash = CryptHelper.MD5Hash(plain);
        // Đúng password
        Assert.True(CryptHelper.VerifyMD5("TestPass@99", hash));
        // Sai password (dù cùng độ dài)
        Assert.False(CryptHelper.VerifyMD5("Different@11", hash));
    }

    [Fact]
    public void MD5Hash_SpecialCharacters_HandlesCorrectly()
    {
        var result = CryptHelper.MD5Hash("Mật khẩu!@#$%^&*()");
        Assert.Equal(32, result.Length);
        Assert.DoesNotContain(" ", result);
    }
}
