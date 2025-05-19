using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Helper
{
    public static class LoginHelper
    {
        public static bool IsEmailVerified(Member member)
        {
            return member.Flag == "T";
        }

        public static bool IsAccountLocked(Member member, int maxFail = 5, int lockMinutes = 15)
        {
            // 👉 Chuyển Field5 (dạng chuỗi) thành số nguyên failCount.
            // Nếu Field5 không phải số hợp lệ (null, "", "abc"...), thì mặc định failCount = 0
            int failCount = int.TryParse(member.Field5, out var fc) ? fc : 0;
            DateTime.TryParse(member.Field4, out DateTime lastFailAt);

            return failCount >= maxFail && lastFailAt.AddMinutes(lockMinutes) > DateTime.UtcNow;
        }

        public static void IncreaseFailCount(Member member)
        {
            int failCount = int.TryParse(member.Field5, out var fc) ? fc : 0;
            failCount++;
            member.Field5 = failCount.ToString();
            member.Field4 = DateTime.UtcNow.ToString("o");
        }

        public static void ResetFailCount(Member member)
        {
            member.Field5 = "0";
            member.Field4 = null;
        }

        public static bool IsPasswordCorrect(string inputPassword, string hashedPassword)
        {
            return PasswordHasher.VerifyPassword(inputPassword, hashedPassword);
        }
    }
}
