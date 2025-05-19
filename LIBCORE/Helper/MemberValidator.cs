using LIBCORE.DataRepository;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LIBCORE.Helper
{
    public static class MemberValidator
    {
        /// ✅ Kiểm tra độ mạnh của mật khẩu
        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new Exception("❌ Mật khẩu phải dài tối thiểu 8 ký tự.");

            var weakPasswords = new[] { "12345678", "password", "abcdefg", "123456789", "qwerty" };
            if (weakPasswords.Contains(password.ToLower()))
                throw new Exception("❌ Mật khẩu quá đơn giản. Vui lòng chọn mật khẩu mạnh hơn.");

            // (Tuỳ chọn nâng cao)
            // Regex bắt buộc có ít nhất 1 chữ hoa, 1 số, 1 ký tự đặc biệt
            var strongPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$";
            if (!Regex.IsMatch(password, strongPattern))
                throw new Exception("❌ Mật khẩu cần ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt.");

        }

        /// ✅ Kiểm tra định dạng email đơn giản
        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
                throw new Exception("❌ Email không hợp lệ.");
        }

        /// ✅ Kiểm tra username
        public static void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
                throw new Exception("❌ Username phải dài ít nhất 4 ký tự.");
            if (username.Contains(" "))
                throw new Exception("❌ Username không được chứa khoảng trắng.");
        }

        /// ✅ Kiểm tra xem email hoặc username đã tồn tại chưa
        public static async Task CheckDuplicateAsync(IMemberRepository repo, string email, string username)
        {
            var emailTable = await repo.SelectByEmailAsync(email);
            var usernameTable = await repo.SelectByUsernameAsync(username);

            if (emailTable.Rows.Count > 0 && usernameTable.Rows.Count > 0)
                throw new Exception("❌ Email và Username đã được sử dụng.");

            if (emailTable.Rows.Count > 0)
                throw new Exception("❌ Email đã được sử dụng cho tài khoản khác.");

            if (usernameTable.Rows.Count > 0)
                throw new Exception("❌ Username đã được sử dụng.");
        }
    }
}
