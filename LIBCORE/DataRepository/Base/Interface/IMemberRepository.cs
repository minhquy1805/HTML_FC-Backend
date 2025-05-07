using LIBCORE.Models;
using System.Data;

namespace LIBCORE.DataRepository
{
    public partial interface IMemberRepository
    {
        internal Task<DataTable> SelectByPrimaryKeyAsync(int memberId);

        internal Task<DataTable> SelectAllAsync();

        internal Task DeleteAsync(int memberId);

        //Task<string?> LoginAsync(string username, string password);

        internal Task<int> InsertAsync(Member objMember);

        internal Task<DataTable> SelectAllDynamicWhereAsync(
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
            DateTime? refreshTokenExpiryTime         // ✅ thêm dòng này
        );

        internal Task UpdateAsync(Member objMember);

        Task<DataTable> SelectByEmailAsync(string email);
        Task<DataTable> SelectByUsernameAsync(string username);

        Task UpdateRefreshTokenAsync(int memberId, string? refreshToken, DateTime? expiryTime);

        Task RevokeRefreshTokenAsync(int memberId);
    }
}
