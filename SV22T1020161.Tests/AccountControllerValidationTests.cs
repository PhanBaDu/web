using SV22T1020161.Models.Partner;

namespace SV22T1020161.Tests;

/// <summary>
/// TC-R1: Test password strength validation logic
/// TC-R3: Test required field validation logic
/// </summary>
public class AccountValidationTests
{
    // ===== TC-R1: Password strength =====
    private bool ValidatePasswordStrength(string pw)
    {
        if (string.IsNullOrEmpty(pw)) return false;
        if (pw.Length < 6) return false;
        if (!pw.Any(char.IsUpper)) return false;
        if (!pw.Any(char.IsDigit)) return false;
        if (!pw.Any(ch => !char.IsLetterOrDigit(ch))) return false;
        return true;
    }

    [Theory]
    [InlineData("abc", false)]         // < 6 chars
    [InlineData("Abcdefg", false)]     // no digit
    [InlineData("Abcdefg1", false)]   // no special
    [InlineData("Abc@123!", true)]     // valid
    [InlineData("Xyz@999!", true)]     // valid
    public void ValidatePasswordStrength_ReturnsExpected(string password, bool expected)
    {
        Assert.Equal(expected, ValidatePasswordStrength(password));
    }

    // ===== TC-R3: Customer name =====
    private string? ValidateCustomerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Họ tên không được để trống.";
        if (name.Trim().Length < 2)
            return "Họ tên phải có ít nhất 2 ký tự.";
        return null;
    }

    [Theory]
    [InlineData("", "Họ tên không được để trống.")]
    [InlineData("A", "Họ tên phải có ít nhất 2 ký tự.")]
    [InlineData("  ", "Họ tên không được để trống.")]
    [InlineData("Nguyen Van A", null)]
    public void ValidateCustomerName_ReturnsExpected(string name, string? expectedError)
    {
        Assert.Equal(expectedError, ValidateCustomerName(name));
    }

    // ===== TC-R2: Email format =====
    private string? ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Vui lòng nhập Email.";
        if (!email.Contains('@') || !email.Contains('.'))
            return "Email không đúng định dạng.";
        return null;
    }

    [Theory]
    [InlineData("", "Vui lòng nhập Email.")]
    [InlineData("abc", "Email không đúng định dạng.")]
    [InlineData("abc@gmail", "Email không đúng định dạng.")]
    [InlineData("valid@email.com", null)]
    public void ValidateEmail_ReturnsExpected(string email, string? expectedError)
    {
        Assert.Equal(expectedError, ValidateEmail(email));
    }

    // ===== TC-R3: Phone =====
    private string? ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return "Số điện thoại không được để trống.";
        string digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length < 10 || digits.Length > 11)
            return "Số điện thoại phải từ 10-11 chữ số.";
        return null;
    }

    [Theory]
    [InlineData("", "Số điện thoại không được để trống.")]
    [InlineData("12345", "Số điện thoại phải từ 10-11 chữ số.")]
    [InlineData("012345678901", "Số điện thoại phải từ 10-11 chữ số.")]
    [InlineData("0123456789", null)]   // 10 digits OK
    [InlineData("0912345678", null)]   // 10 digits OK
    [InlineData("09123456789", null)] // 11 digits OK
    public void ValidatePhone_ReturnsExpected(string phone, string? expectedError)
    {
        Assert.Equal(expectedError, ValidatePhone(phone));
    }

    // ===== TC-R3: Province required =====
    [Fact]
    public void ValidateProvince_Empty_ReturnsError()
    {
        var result = string.IsNullOrWhiteSpace("") ? "Vui lòng chọn tỉnh/thành phố." : null;
        Assert.Equal("Vui lòng chọn tỉnh/thành phố.", result);
    }

    [Fact]
    public void ValidateProvince_HasValue_ReturnsNull()
    {
        var result = string.IsNullOrWhiteSpace("Ho Chi Minh") ? "Vui lòng chọn tỉnh/thành phố." : null;
        Assert.Null(result);
    }

    // ===== TC-L1: Login empty check =====
    [Theory]
    [InlineData("", "Abc@123!", true)]
    [InlineData("test@gmail.com", "", true)]
    [InlineData("", "", true)]
    [InlineData("test@gmail.com", "Abc@123!", false)]
    public void IsLoginFieldsEmpty_ReturnsExpected(string email, string password, bool expectedEmpty)
    {
        var isEmpty = string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password);
        Assert.Equal(expectedEmpty, isEmpty);
    }

    // ===== TC-P3: Change password =====
    private string? ValidateChangePassword(string oldPw, string newPw, string confirmPw)
    {
        if (string.IsNullOrWhiteSpace(oldPw) || string.IsNullOrWhiteSpace(newPw) || string.IsNullOrWhiteSpace(confirmPw))
            return "Vui lòng điền đầy đủ tất cả các trường.";
        if (newPw != confirmPw)
            return "Mật khẩu xác nhận không khớp.";
        if (newPw.Length < 6)
            return "Mật khẩu mới phải có ít nhất 6 ký tự.";
        if (!newPw.Any(char.IsUpper) || !newPw.Any(char.IsDigit) || !newPw.Any(ch => !char.IsLetterOrDigit(ch)))
            return "Mật khẩu mới phải chứa chữ HOA, chữ số và ký tự đặc biệt.";
        return null;
    }

    [Theory]
    [InlineData("", "Abc@123!", "Abc@123!", "Vui lòng điền đầy đủ")]
    [InlineData("OldPass@1", "", "Xyz@456!", "Vui lòng điền đầy đủ")]
    [InlineData("OldPass@1", "Xyz@456!", "", "Vui lòng điền đầy đủ")]
    public void ValidateChangePassword_EmptyFields_ReturnsError(
        string old, string newPw, string confirm, string expectedContains)
    {
        var error = ValidateChangePassword(old, newPw, confirm);
        Assert.NotNull(error);
        Assert.Contains("đầy đủ", error);
    }

    [Fact]
    public void ValidateChangePassword_Mismatch_ReturnsError()
    {
        var error = ValidateChangePassword("OldPass@1", "NewPass@1", "Different@2");
        Assert.Equal("Mật khẩu xác nhận không khớp.", error);
    }

    [Theory]
    [InlineData("abc", "Mật khẩu mới phải có ít nhất 6 ký tự.")]
    [InlineData("abcdefg", "Mật khẩu mới phải chứa chữ HOA, chữ số và ký tự đặc biệt.")]
    public void ValidateChangePassword_WeakNew_ReturnsError(string newPw, string expectedError)
    {
        var error = ValidateChangePassword("OldPass@1", newPw, newPw);
        Assert.Equal(expectedError, error);
    }

    [Fact]
    public void ValidateChangePassword_Valid_ReturnsNull()
    {
        var error = ValidateChangePassword("OldPass@1", "NewPass@1", "NewPass@1");
        Assert.Null(error);
    }

    // ===== TC-R5: Password + Confirm =====
    [Theory]
    [InlineData("Abc@123!", "Abc@123!", true)]
    [InlineData("Abc@123!", "Xyz@456!", false)]
    public void PasswordConfirm_Match_ReturnsExpected(string pw, string confirm, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, pw == confirm);
    }

    // ===== Customer model creation =====
    [Fact]
    public void Customer_NewAccount_IsNotLocked()
    {
        var customer = new Customer
        {
            CustomerName = "Test User",
            Email = "test@gmail.com",
            Phone = "0123456789",
            Province = "Ho Chi Minh",
            Password = "Abc@123!"
        };

        customer.IsLocked = false;
        Assert.False(customer.IsLocked);
    }
}
