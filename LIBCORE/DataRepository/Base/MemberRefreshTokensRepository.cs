using LIBCORE.DataRepository.Helper;
using LIBCORE.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial class MemberRefreshTokensRepository : IMemberRefreshTokenRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal MemberRefreshTokensRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertAsync(MemberRefreshToken token)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_Insert]";
            var sqlParamList = new List<SqlParameter>
        {
            new SqlParameter("@memberId", token.MemberId),
            new SqlParameter("@refreshToken", token.RefreshToken ?? (object)DBNull.Value),
            new SqlParameter("@refreshTokenExpiry", token.RefreshTokenExpiry ?? (object)DBNull.Value),
            new SqlParameter("@deviceInfo", token.DeviceInfo ?? (object)DBNull.Value),
            new SqlParameter("@createdAt", token.CreatedAt ?? DateTime.UtcNow),
            new SqlParameter("@field1", token.Field1 ?? (object)DBNull.Value),
            new SqlParameter("@field2", token.Field2 ?? (object)DBNull.Value),
            new SqlParameter("@field3", token.Field3 ?? (object)DBNull.Value),
            new SqlParameter("@field4", token.Field4 ?? (object)DBNull.Value),
            new SqlParameter("@field5", token.Field5 ?? (object)DBNull.Value),
            new SqlParameter("@flag", token.Flag ?? (object)DBNull.Value)
        };

            var result = await DatabaseFunctions.ExecuteSqlCommandAsync(
                _connectionString, storedProcedure, sqlParamList, _commandType,
                DatabaseOperationType.Create, false);

            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToInt32(result);
        }

        public async Task<DataTable> SelectByMemberIdAsync(int memberId)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_SelectByMemberId]";
            var sqlParamList = new List<SqlParameter> { new SqlParameter("@memberId", memberId) };
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        public async Task<DataTable> SelectByRefreshTokenAsync(string refreshToken)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_SelectByRefreshToken]";
            var sqlParamList = new List<SqlParameter> { new SqlParameter("@refreshToken", refreshToken) };
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        public async Task<DataTable> SelectByDeviceInfoAsync(int memberId, string deviceInfo)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_SelectByDeviceInfo]";
            var sqlParamList = new List<SqlParameter>
            {
                new SqlParameter("@memberId", memberId),
                new SqlParameter("@deviceInfo", deviceInfo)
            };
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        public async Task UpdateByMemberRefreshTokensIdAsync(MemberRefreshToken token)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_UpdateByMemberRefreshTokensId]";
            var sqlParamList = new List<SqlParameter>
            {
                new SqlParameter("@memberRefreshTokensId", token.MemberRefreshTokensId),
                new SqlParameter("@refreshToken", token.RefreshToken ?? (object)DBNull.Value),
                new SqlParameter("@refreshTokenExpiry", token.RefreshTokenExpiry ?? (object)DBNull.Value),
                new SqlParameter("@deviceInfo", token.DeviceInfo ?? (object)DBNull.Value),
                new SqlParameter("@field1", token.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@field2", token.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@field3", token.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@field4", token.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@field5", token.Field5 ?? (object)DBNull.Value),
                new SqlParameter("@flag", token.Flag ?? (object)DBNull.Value)
            };
            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Update);
        }

        public async Task DeleteByMemberRefreshTokensIdAsync(int memberRefreshTokensId)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_DeleteByMemberRefreshTokensId]";
            var sqlParamList = new List<SqlParameter> { new SqlParameter("@memberRefreshTokensId", memberRefreshTokensId) };
            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        public async Task DeleteAllByMemberIdAsync(int memberId)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_DeleteAllByMemberId]";
            var sqlParamList = new List<SqlParameter> { new SqlParameter("@memberId", memberId) };
            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        public async Task<DataTable> SelectByMemberRefreshTokensIdAsync(int memberRefreshTokensId)
        {
            string storedProcedure = "[dbo].[MemberRefreshToken_SelectByMemberRefreshTokensId]";
            var sqlParamList = new List<SqlParameter> { new SqlParameter("@memberRefreshTokensId", memberRefreshTokensId) };
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }
    }
}
