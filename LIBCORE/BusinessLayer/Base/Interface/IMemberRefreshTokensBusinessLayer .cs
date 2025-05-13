using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.BusinessLayer
{
    public partial interface IMemberRefreshTokensBusinessLayer
    {
        public Task<int> InsertAsync(MemberRefreshToken token);

        public Task<DataTable> SelectByMemberIdAsync(int memberId);

        public Task<DataTable> SelectByRefreshTokenAsync(string refreshToken);

        public Task<DataTable> SelectByDeviceInfoAsync(int memberId, string deviceInfo);

        public Task<DataTable> SelectByMemberRefreshTokensIdAsync(int memberRefreshTokensId);

        public Task UpdateByMemberRefreshTokensIdAsync(MemberRefreshToken token);

        public Task DeleteByMemberRefreshTokensIdAsync(int memberRefreshTokensId);

        public Task DeleteAllByMemberIdAsync(int memberId);
    }
}
