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

        async Task<DataTable> IMemberRepository.SelectAllDynamicWhereAsync(int? memberId, string firstName, string middleName, string lastName, string phone, string email, string facebook, string address, string type, string avatar, int? numberPlayer, string role, string username, string password, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            string storedProcName = "[dbo].[Member_SelectAllWhereDynamic]";
            List<SqlParameter> sqlParamList = new();


            this.AddSearchCommandParamsShared(sqlParamList, memberId, firstName, middleName, lastName, phone, email, facebook, address, type, avatar, numberPlayer, role, username, password, field1, field2, field3, field4, field5, createdAt, flag);


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
            // Gán Role
            if (member.Email == "minhquy073@gmail.com" || member.Email == "11giakhanh03@gmail.com")
            {
                member.Role = "Admin";
            }
            else
            {
                member.Role = "User";
            }

            // Gán Flag chưa xác thực
            member.Flag = "F";

            // Sinh mã xác thực
            var verificationService = new VerificationService();
            string code = verificationService.GenerateVerificationCode();

            // Gán mã xác thực vào Field1
            member.Field1 = code;
            member.Field2 = DateTime.UtcNow.AddMinutes(10).ToString("o"); // ISO 8601

            // Gửi email xác nhận
            await emailService.SendVerificationEmailAsync(member.Email!, code);

            // Insert dữ liệu
            return await this.InsertUpdateAsync(member, DatabaseOperationType.Create);
        }

        //async Task IMemberRepository.UpdateAsync(Member member)
        //{
        //    // Lấy mật khẩu cũ từ database
        //    string oldPassword = await GetMemberPasswordById(member.MemberId);

        //    // Nếu người dùng nhập mật khẩu mới thì mã hóa, ngược lại giữ nguyên mật khẩu cũ
        //    if (!string.IsNullOrEmpty(member.Password))
        //    {
        //        member.Password = PasswordHasher.HashPassword(member.Password);
        //    }
        //    else
        //    {
        //        member.Password = oldPassword;
        //    }

        //    await this.InsertUpdateAsync(member, DatabaseOperationType.Update);
        //}


        async Task IMemberRepository.UpdateAsync(Member member)
        {
            string oldPassword = await GetMemberPasswordById(member.MemberId);

            if (!string.IsNullOrEmpty(member.Password))
            {
                if (!PasswordHasher.IsHashed(member.Password))  // ✅ Chỉ hash nếu chưa mã hóa
                {
                    member.Password = PasswordHasher.HashPassword(member.Password);
                }
            }
            else
            {
                member.Password = oldPassword;
            }

            await this.InsertUpdateAsync(member, DatabaseOperationType.Update);
        }

        //public async Task<string?> LoginAsync(string username, string password)
        //{
        //    string storedProcedure = "[dbo].[Member_Login]";
        //    List<SqlParameter> sqlParamList = new();
        //    DatabaseFunctions.AddSqlParameter(sqlParamList, "@username", username);

        //    var dataTable = await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);

        //    if (dataTable.Rows.Count == 0)
        //    {
        //        Console.WriteLine("Không tìm thấy tài khoản: " + username);
        //        return null;
        //    }

        //    var row = dataTable.Rows[0];

        //    if (row["Password"] == DBNull.Value || row["Password"] is null)
        //    {
        //        Console.WriteLine($"🔴 Lỗi: Tài khoản {username} không có mật khẩu trong DB!");
        //        return null;
        //    }

        //    string hashedPassword = row["Password"].ToString()!;

        //    Console.WriteLine("Mật khẩu nhập vào: " + password);
        //    Console.WriteLine("Mật khẩu từ DB: " + hashedPassword);

        //    // Kiểm tra mật khẩu với BCrypt
        //    bool isMatch = PasswordHasher.VerifyPassword(password, hashedPassword);
        //    Console.WriteLine("Kết quả kiểm tra BCrypt: " + isMatch);

        //    if (!isMatch)
        //    {
        //        Console.WriteLine("Sai mật khẩu!");
        //        return null;
        //    }

        //    int userId = row["MemberId"] != DBNull.Value ? Convert.ToInt32(row["MemberId"]) : 0;
        //    string role = row["Role"] != DBNull.Value ? row["Role"].ToString()! : "User";

        //    Console.WriteLine($"✅ Đăng nhập thành công! UserId: {userId}, Role: {role}");

        //    // Tạo JWT token
        //    string token = _jwtTokenGenerator.GenerateToken(userId, username, role);
        //    return token;
        //}

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

        private async Task<int> InsertUpdateAsync(Member member, DatabaseOperationType operationType)
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedMemberId = member.MemberId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[Member_Update]";
            else
                storedProcedure = "[dbo].[Member_Insert]";

            // Nếu là Create, mã hóa mật khẩu trước khi lưu
            if (operationType == DatabaseOperationType.Create)
            {
                if (!string.IsNullOrEmpty(member.Password))
                {
                    member.Password = PasswordHasher.HashPassword(member.Password);
                }
            }
            else if (operationType == DatabaseOperationType.Update)
            {
                // Nếu mật khẩu mới không nhập, giữ nguyên mật khẩu cũ từ database
                string oldPassword = await GetMemberPasswordById(member.MemberId);
                if (string.IsNullOrEmpty(member.Password))
                {
                    member.Password = oldPassword;
                }
                else if (member.Password != oldPassword)
                {
                    member.Password = PasswordHasher.HashPassword(member.Password);
                }
            }

            // Xử lý DBNull gọn hơn bằng phương thức tiện ích
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
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@password", GetDbValue(member.Password)); // Mật khẩu đã xử lý trước đó
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", GetDbValue(member.Field1));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", GetDbValue(member.Field2));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", GetDbValue(member.Field3));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", GetDbValue(member.Field4));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", GetDbValue(member.Field5));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", GetDbValue(member.CreatedAt ?? DateTime.Now));
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", GetDbValue(member.Flag));

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

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? memberId, string firstName, string middleName, string lastName, string phone, string email, string facebook, string address, string type, string avatar, int? numberPlayer, string role, string username, string password, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
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
