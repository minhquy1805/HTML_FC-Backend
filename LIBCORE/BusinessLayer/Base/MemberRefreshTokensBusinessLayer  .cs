using LIBCORE.DataRepository;
using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.BusinessLayer
{
    public partial class MemberRefreshTokensBusinessLayer : IMemberRefreshTokensBusinessLayer
    {
        private readonly IMemberRefreshTokenRepository _repository;

        public MemberRefreshTokensBusinessLayer(IMemberRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> InsertAsync(MemberRefreshToken token)
        {
            return await _repository.InsertAsync(token);
        }

        public async Task<DataTable> SelectByMemberIdAsync(int memberId)
        {
            return await _repository.SelectByMemberIdAsync(memberId);
        }

        public async Task<DataTable> SelectByRefreshTokenAsync(string refreshToken)
        {
            return await _repository.SelectByRefreshTokenAsync(refreshToken);
        }

        public async Task<DataTable> SelectByDeviceInfoAsync(int memberId, string deviceInfo)
        {
            return await _repository.SelectByDeviceInfoAsync(memberId, deviceInfo);
        }

        public async Task<DataTable> SelectByMemberRefreshTokensIdAsync(int memberRefreshTokensId)
        {
            return await _repository.SelectByMemberRefreshTokensIdAsync(memberRefreshTokensId);
        }

        public async Task UpdateByMemberRefreshTokensIdAsync(MemberRefreshToken token)
        {
            await _repository.UpdateByMemberRefreshTokensIdAsync(token);
        }

        public async Task DeleteByMemberRefreshTokensIdAsync(int memberRefreshTokensId)
        {
            await _repository.DeleteByMemberRefreshTokensIdAsync(memberRefreshTokensId);
        }

        public async Task DeleteAllByMemberIdAsync(int memberId)
        {
            await _repository.DeleteAllByMemberIdAsync(memberId);
        }
    }
}
