using LIBCORE.DataRepository.Helper;
using LIBCORE.Helper;
using LIBCORE.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LIBCORE.DataRepository
{
    public partial class MemberRepository : IMemberRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly EmailService emailService;

        internal MemberRepository(string connectionString, JwtTokenGenerator jwtTokenGenerator, EmailService emailService)
        {
            _connectionString = connectionString;
            _jwtTokenGenerator = jwtTokenGenerator;
            this.emailService = emailService;
        }

        async Task<DataTable> IMemberRepository.SelectByPrimaryKeyAsync(int memberId)
        { 
            string storedProcedure = "[dbo].[Member_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", memberId);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> IMemberRepository.SelectAllAsync()
        {
            string storedProcedure = "[dbo].[Member_SelectAll]";
           
            return await this.SelectSharedAsync(storedProcedure, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> IMemberRepository.SelectAllDynamicWhereAsync(
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
            string refreshToken,
            DateTime? refreshTokenExpiryTime
        )
        {
            string storedProcName = "[dbo].[Member_SelectAllWhereDynamic]";
            List<SqlParameter> sqlParamList = new();

            this.AddSearchCommandParamsShared(
                sqlParamList,
                memberId,
                firstName,
                middleName,
                lastName,
                phone,
                email,
                facebook,
                address,
                type,
                avatar,
                numberPlayer,
                role,
                username,
                password,
                field1,
                field2,
                field3,
                field4,
                field5,
                createdAt,
                flag,
                refreshToken,
                refreshTokenExpiryTime
            );

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        async Task IMemberRepository.DeleteAsync(int memberId)
        {
            string storedProcedure = "[dbo].[Member_Delete]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", memberId);

            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        public async Task<int> InsertAsync(Member member)
        {
            return await this.InsertUpdateAsync(member, DatabaseOperationType.Create);
        }

        async Task IMemberRepository.UpdateAsync(Member member)
        {
            // ❌ KHÔNG xử lý hash tại đây nữa
            await this.InsertUpdateAsync(member, DatabaseOperationType.Update);
        }

        private async Task<DataTable> SelectSharedAsync(string storedProcName, string parameterName, object parameterValue, string sortByExpression, int? startRowIndex, int? rows)
        {
            List<SqlParameter> sqlParamList = new();

            // select, skip, take, sort sql parameters
            if (!String.IsNullOrEmpty(sortByExpression) && startRowIndex is not null && rows is not null)
                DatabaseFunctions.AddSelectSkipAndTakeParams(sqlParamList, sortByExpression, startRowIndex.Value, rows.Value);

            // get and return the data
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        public async Task<DataTable> SelectByUsernameAsync(string username)
        {
            
            string storedProcedure = "[dbo].[Member_SelectByUsername]";
            List<SqlParameter> sqlParamList = new();
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@username", username);

            var dataTable = await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);


            return dataTable;
        }

        public async Task<DataTable> SelectByEmailAsync(string email)
        {
            var sqlParams = new List<SqlParameter>();
            DatabaseFunctions.AddSqlParameter(sqlParams, "@Email", email);
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, "[dbo].[Member_SelectByEmail]", sqlParams, CommandType.StoredProcedure);
        }


        public async Task UpdateRefreshTokenAsync(int memberId, string? refreshToken, DateTime? expiryTime)
        {
            var sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@memberId", memberId),
                new SqlParameter("@refreshToken", (object?)refreshToken ?? DBNull.Value),
                new SqlParameter("@refreshTokenExpiry", (object?)expiryTime ?? DBNull.Value)
            };

            await DatabaseFunctions.ExecuteSqlCommandAsync(
                _connectionString,
                "[dbo].[Member_UpdateRefreshToken]",
                sqlParams,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update);
        }

        public async Task RevokeRefreshTokenAsync(int memberId)
        {
            // Chỉ cần gọi lại UpdateRefreshTokenAsync với null là xóa được
            await UpdateRefreshTokenAsync(memberId, null!, null);
        }

        // ✅ Refactored MemberRepository to remove password hashing from Repository Layer
        // Hashing should be handled in Business Layer only

        private async Task<int> InsertUpdateAsync(Member member, DatabaseOperationType operationType)
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType! Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedMemberId = member.MemberId;

            string storedProcedure = operationType == DatabaseOperationType.Update
                ? "[dbo].[Member_Update]"
                : "[dbo].[Member_Insert]";

            // ✅ NO password hashing here, all logic should be handled by Business Layer

            // Handle DBNull conversion
            object GetDbValue<T>(T value) => string.IsNullOrEmpty(value?.ToString()) ? DBNull.Value : value!;

            if (operationType == DatabaseOperationType.Update)
            {
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", GetDbValue(member.MemberId));
            }

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@firstName", GetDbValue(member.FirstName));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@middleName", GetDbValue(member.MiddleName));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@lastName", GetDbValue(member.LastName));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@phone", GetDbValue(member.Phone));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@email", GetDbValue(member.Email));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@facebook", GetDbValue(member.Facebook));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@address", GetDbValue(member.Address));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@type", GetDbValue(member.Type));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@avatar", GetDbValue(member.Avatar));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@numberPlayer", GetDbValue(member.NumberPlayer));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@role", GetDbValue(member.Role));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@username", GetDbValue(member.Username));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@password", GetDbValue(member.Password));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", GetDbValue(member.Field1));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", GetDbValue(member.Field2));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", GetDbValue(member.Field3));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", GetDbValue(member.Field4));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", GetDbValue(member.Field5));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", GetDbValue(member.CreatedAt ?? DateTime.Now));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", GetDbValue(member.Flag));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshToken", GetDbValue(member.RefreshToken));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshTokenExpiry", GetDbValue(member.RefreshTokenExpiryTime));

            if (operationType == DatabaseOperationType.Update)
            {
                await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Update);
            }
            else
            {
                var result = await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
                newlyCreatedMemberId = (int)(result ?? 0); // Default to 0 if null returned
            }

            return newlyCreatedMemberId;
        }


        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList,
            int? memberId, string firstName, string middleName, string lastName,
            string phone, string email, string facebook, string address, string type,
            string avatar, int? numberPlayer, string role, string username, string password,
            string field1, string field2, string field3, string field4, string field5,
            DateTime? createdAt, string flag,
            string refreshToken, DateTime? refreshTokenExpiryTime) // ✅ thêm ở đây) 
        {
            if (memberId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", memberId);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", System.DBNull.Value);

            if (!String.IsNullOrEmpty(firstName))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@firstName", firstName);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@firstName", System.DBNull.Value);

            if (!String.IsNullOrEmpty(middleName))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@middleName", middleName);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@middleName", System.DBNull.Value);

            if (!String.IsNullOrEmpty(lastName))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@lastName", lastName);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@lastName", System.DBNull.Value);

            if (!String.IsNullOrEmpty(phone))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@phone", phone);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@phone", System.DBNull.Value);

            if (!String.IsNullOrEmpty(email))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@email", email);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@email", System.DBNull.Value);

            if (!String.IsNullOrEmpty(facebook))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@facebook", facebook);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@facebook", System.DBNull.Value);

            if (!String.IsNullOrEmpty(address))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@address", address);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@address", System.DBNull.Value);

            if (!String.IsNullOrEmpty(type))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@type", type);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@type", System.DBNull.Value);

            if (!String.IsNullOrEmpty(avatar))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@avatar", avatar);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@avatar", System.DBNull.Value);

            if (numberPlayer is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@numberPlayer", numberPlayer);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@numberPlayer", System.DBNull.Value);

            if (!String.IsNullOrEmpty(role))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@role", role);  // Lỗi: nên là @role chứ không phải avatar
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@role", System.DBNull.Value);

            if (!String.IsNullOrEmpty(username))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@username", username);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@username", System.DBNull.Value);

            if (!String.IsNullOrEmpty(password))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@password", password);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@password", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field1))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field2))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field3))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field4))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field5))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", System.DBNull.Value);

            if (createdAt is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", System.DBNull.Value);

            if (!String.IsNullOrEmpty(flag))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", System.DBNull.Value);

            if (!string.IsNullOrEmpty(refreshToken))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshToken", refreshToken);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshToken", System.DBNull.Value);

            // ✅ Thêm xử lý cho RefreshTokenExpiryTime
            if (refreshTokenExpiryTime != null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshTokenExpiry", refreshTokenExpiryTime);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@refreshTokenExpiry", System.DBNull.Value);

        }

        private async Task<string> GetMemberPasswordById(int memberId)
        {
            string storedProcedure = "[dbo].[GetMemberPasswordById]";
            List<SqlParameter> sqlParamList = new();
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", memberId);

            var dataTable = await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);

            return (dataTable.Rows.Count > 0) ? dataTable.Rows[0]["Password"].ToString()! : string.Empty;
        }
    }
}
