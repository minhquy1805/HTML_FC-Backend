using LIBCORE.Models;

namespace LIBCORE.BusinessLayer
{
    public partial interface IMemberBusinessLayer
    {
        public Task<Member> SelectByPrimaryKeyAsync(int memberId);

        public Task<List<Member>> SelectAllAsync();

        public Task<List<Member>> SelectAllDynamicWhereAsync(
         int? memberId,
         string firstName,
         string middleName,
         string lastName,
         string phone,
         string email,
         string facebook,
         string address,
         string type,
         string avatar,
         int? numberPlayer,
         string role,
         string username,
         string password,
         string field1,
         string field2,
         string field3,
         string field4,
         string field5,
         DateTime? createdAt,
         string flag,
         string refreshToken,                     // ✅ thêm dòng này
         DateTime? refreshTokenExpiryTime        // ✅ và dòng này
     );

        public Task DeleteAsync(int memberId);


        public Task<int> InsertAsync(Member member);


        public Task UpdateAsync(Member member);

        public Task UpdateProfileAsync(Member member);

        // Thêm phương thức đăng nhập để trả về token
        public Task<string?> LoginAsync(string username, string password, string deviceInfo);

        public Task<bool> VerifyEmailAsync(string email, string inputCode);

        public Task<bool> ResendVerificationCodeAsync(string email);

        public Task<bool> ForgotPasswordAsync(string email);

        public Task<bool> ResetPasswordAsync(string email, string inputCode, string newPassword);

        public Task<string?> RefreshTokenAsync(string refreshToken, string deviceInfo);

        public Task LogoutAsync(int memberId);
    }
}
