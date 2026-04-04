using System.Security.Cryptography;
using System.Text;

namespace SV22T1020161.Shop
{
    /// <summary>
    /// Helper mã hóa / giải mã dùng trong Shop
    /// </summary>
    public static class CryptHelper
    {
        /// <summary>
        /// Mã hóa chuỗi thành MD5 hash (32 ký tự hex, viết thường)
        /// Dùng để lưu/so sánh mật khẩu với SQL Server:
        /// SUBSTRING(master.dbo.fn_varbintohexstr(HASHBYTES('MD5', @Password)), 3, 32)
        /// </summary>
        public static string MD5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder(32);
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// So sánh chuỗi plain text với chuỗi MD5 hash
        /// </summary>
        public static bool VerifyMD5(string plainText, string md5Hash)
        {
            return string.Equals(MD5Hash(plainText), md5Hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
