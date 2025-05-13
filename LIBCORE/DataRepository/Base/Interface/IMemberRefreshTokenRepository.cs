using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial interface IMemberRefreshTokenRepository
    {
        // Thêm mới 1 bản ghi refresh token
        public Task<int> InsertAsync(MemberRefreshToken token); // MemberRefreshToken_Insert

        // Lấy tất cả token của một thành viên (theo memberId)
        public Task<DataTable> SelectByMemberIdAsync(int memberId); // MemberRefreshToken_SelectByMemberId

        // Tìm token cụ thể theo refreshToken (dùng để xác thực khi gọi API /refresh-token)
        public Task<DataTable> SelectByRefreshTokenAsync(string refreshToken); // MemberRefreshToken_SelectByRefreshToken

        // Tìm token theo thiết bị và người dùng
        public Task<DataTable> SelectByDeviceInfoAsync(int memberId, string deviceInfo); // MemberRefreshToken_SelectByDeviceInfo

        // Cập nhật thông tin token theo MemberRefreshTokensId
        public Task UpdateByMemberRefreshTokensIdAsync(MemberRefreshToken token); // MemberRefreshToken_UpdateByMemberRefreshTokensId

        // Xóa token theo khóa chính
        public Task DeleteByMemberRefreshTokensIdAsync(int memberRefreshTokensId); // MemberRefreshToken_DeleteByMemberRefreshTokensId

        // Xóa tất cả token của một thành viên (thường dùng khi logout toàn bộ thiết bị)
        public Task DeleteAllByMemberIdAsync(int memberId); // MemberRefreshToken_DeleteAllByMemberId

        // Tìm token theo khóa chính (dùng cho quản trị hoặc khi cần truy vết)
        public Task<DataTable> SelectByMemberRefreshTokensIdAsync(int memberRefreshTokensId); // MemberRefreshToken_SelectByMemberRefreshTokensId

    }
}
