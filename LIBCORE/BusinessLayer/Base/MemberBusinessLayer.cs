using Docker.DotNet.Models;
using LIBCORE.DataRepository;
using LIBCORE.Helper;
using LIBCORE.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Composition;
using System.Data;

namespace LIBCORE.BusinessLayer
{
    public partial class MemberBusinessLayer : IMemberBusinessLayer
    {
        private readonly IMemberRepository _memberRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly EmailService emailService;
        private readonly IMemberRefreshTokensBusinessLayer _memberRefreshTokensBusinessLayer;

        public MemberBusinessLayer(IMemberRepository memberRepository, JwtTokenGenerator jwtTokenGenerator, EmailService emailService, IMemberRefreshTokensBusinessLayer memberRefreshTokensBusinessLayer)
        {
            _memberRepository = memberRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            this.emailService = emailService;
            _memberRefreshTokensBusinessLayer = memberRefreshTokensBusinessLayer;
        }

        public async Task<Member> SelectByPrimaryKeyAsync(int memberId)
        {
            DataTable dt = await _memberRepository.SelectByPrimaryKeyAsync(memberId);

            // create ThiSinh
            if (dt is not null && dt.Rows.Count > 0)
                return this.CreateMemberFromDataRow(dt.Rows[0]);

            return null!;
        }

        public async Task<List<Member>> SelectAllAsync()
        {
            DataTable dt = await _memberRepository.SelectAllAsync();
            return this.GetListOfMember(dt);
        }

        public async Task<List<Member>> SelectAllDynamicWhereAsync(
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
            var dt = await _memberRepository.SelectAllDynamicWhereAsync(
                memberId, firstName, middleName, lastName, phone, email, facebook,
                address, type, avatar, numberPlayer, role, username, password,
                field1, field2, field3, field4, field5, createdAt, flag,
                refreshToken, refreshTokenExpiryTime
            );
            return this.GetListOfMember(dt);
        }


        public async Task<int> InsertAsync(Member member)
        {
            // Danh sách các thành viên có cùng Email hoặc Username
            var emailTable = await _memberRepository.SelectByEmailAsync(member.Email!);
            var usernameTable = await _memberRepository.SelectByUsernameAsync(member.Username!);

            bool emailExists = emailTable.Rows.Count > 0;
            bool usernameExists = usernameTable.Rows.Count > 0;

            if (emailExists && usernameExists)
                throw new Exception("❌ Email và Username đã được sử dụng.");

            if (emailExists)
                throw new Exception("❌ Email đã được sử dụng cho tài khoản khác.");

            if (usernameExists)
                throw new Exception("❌ Username đã được sử dụng.");

            return await _memberRepository.InsertAsync(member);
        }

        public async Task UpdateAsync(Member member)
        {
            await _memberRepository.UpdateAsync(member);
        }

        public async Task DeleteAsync(int memberId)
        {
            await _memberRepository.DeleteAsync(memberId);
        }

        // --------------------- THÊM LOGIN --------------------- 
        public async Task<string?> LoginAsync(string username, string password, string deviceInfo)
        {
            // Tìm thành viên theo username
            DataTable dt = await _memberRepository.SelectByUsernameAsync(username);

            if (dt is null || dt.Rows.Count == 0)
            {
                Console.WriteLine("❌ Không tìm thấy user trong DB!");
                return null;
            }

            // Lấy thông tin user
            Member member = this.CreateMemberFromDataRow(dt.Rows[0]);

            // ❗ Check xác thực email
            if (member.Flag != "T")
            {
                Console.WriteLine("⚠️ Tài khoản chưa xác thực email.");
                return null;
            }

            // 👉 Kiểm tra giới hạn login sai
            int failCount = int.TryParse(member.Field5, out var fc) ? fc : 0;
            DateTime.TryParse(member.Field4, out DateTime lastFailAt);

            if (failCount >= 5 && lastFailAt.AddMinutes(15) > DateTime.UtcNow)
            {
                Console.WriteLine("⛔ Tài khoản bị khóa tạm thời do nhập sai quá nhiều lần!");
                return null;
            }

            // ✅ Kiểm tra mật khẩu
            bool isMatch = PasswordHasher.VerifyPassword(password, member.Password!);
            Console.WriteLine($"🛠 Kết quả kiểm tra mật khẩu: {isMatch}");

            if (!isMatch)
            {
                // ❌ Sai → tăng bộ đếm + cập nhật thời gian
                failCount++;
                member.Field5 = failCount.ToString();
                member.Field4 = DateTime.UtcNow.ToString("o");
                await this.UpdateAsync(member);

                Console.WriteLine($"❌ Sai mật khẩu! Số lần sai: {failCount}");
                return null;
            }

            // ✅ Đúng → reset fail count
            member.Field5 = "0";
            member.Field4 = null;
            await this.UpdateAsync(member);

            // 🔑 Tạo Access Token
            string accessToken = _jwtTokenGenerator.GenerateToken(member.MemberId, member.Username!, member.Role!);

            // 🔁 Tạo Refresh Token
            string refreshToken = Guid.NewGuid().ToString();
            DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // ✅ Lưu refresh token vào database
            await _memberRepository.UpdateRefreshTokenAsync(member.MemberId, refreshToken, refreshTokenExpiry);

            // ➕ Lưu vào bảng MemberRefreshTokens
            await _memberRefreshTokensBusinessLayer.InsertAsync(new MemberRefreshToken
            {
                MemberId = member.MemberId,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry,
                DeviceInfo = deviceInfo,
                CreatedAt = DateTime.UtcNow,
                Flag = "A"
            });

            // ✅ Trả về cả access token và refresh token dưới dạng JSON string
            var result = new
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };

            return System.Text.Json.JsonSerializer.Serialize(result);
        }


        public async Task<bool> VerifyEmailAsync(string email, string inputCode)
        {
            var members = await _memberRepository.SelectAllDynamicWhereAsync(
                null, null!, null!, null!, null!, email,
                null!, null!, null!, null!, null,
                null!, null!, null!, null!, null!,
                null!, null!, null!, null!, null!,
                null!, null! // ✅ thêm 2 tham số: refreshToken, refreshTokenExpiryTime
            );

            if (members.Rows.Count == 0)
                return false;

            var row = members.Rows[0];
            string savedCode = row["Field1"]?.ToString() ?? "";
            string? expireAtStr = row["Field2"]?.ToString();

            if (inputCode != savedCode)
                return false;

            if (!DateTime.TryParse(expireAtStr, out DateTime expireAtUtc))
                return false; // Lỗi khi parse thời gian hết hạn

            if (DateTime.UtcNow > expireAtUtc)
                return false; // Hết hạn mã xác thực

            // Lấy lại full thông tin member
            var member = this.CreateMemberFromDataRow(row);
            member.Field1 = null;
            member.Field2 = null;
            member.Flag = "T";

            await this.UpdateAsync(member);
            return true;
        }

        public async Task<bool> ResendVerificationCodeAsync(string email)
        {
            var members = await _memberRepository.SelectAllDynamicWhereAsync(
               null, null!, null!, null!, null!, email,
               null!, null!, null!, null!, null,
               null!, null!, null!, null!, null!,
               null!, null!, null!, null!, null!,
               null!, null! // ✅ thêm 2 tham số: refreshToken, refreshTokenExpiryTime
           );

            if (members.Rows.Count == 0)
                return false;

            var row = members.Rows[0];

            if (row["Flag"]?.ToString() == "T")
                return false; // Đã xác thực rồi thì không gửi lại nữa

            // Tạo mã mới
            var verificationService = new VerificationService();
            string newCode = verificationService.GenerateVerificationCode();
            string newExpireAt = DateTime.UtcNow.AddMinutes(10).ToString("o");

            // Gửi mail
            string userEmail = row["Email"].ToString()!;
            await emailService.SendVerificationEmailAsync(userEmail, newCode);

            // Cập nhật mã mới và thời gian hết hạn
            var member = this.CreateMemberFromDataRow(row);
            member.Field1 = newCode;
            member.Field2 = newExpireAt;

            await this.UpdateAsync(member);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string inputCode, string newPassword)
        {
            var members = await _memberRepository.SelectAllDynamicWhereAsync(
               null, null!, null!, null!, null!, email,
               null!, null!, null!, null!, null,
               null!, null!, null!, null!, null!,
               null!, null!, null!, null!, null!,
               null!, null! // ✅ thêm 2 tham số: refreshToken, refreshTokenExpiryTime
           );

            if (members.Rows.Count == 0)
                return false;

            var row = members.Rows[0];
            string savedCode = row["Field3"]?.ToString() ?? "";
            string? expireAtStr = row["Field4"]?.ToString();

            if (inputCode != savedCode)
                return false;

            if (!DateTime.TryParse(expireAtStr, out DateTime expireAt) || DateTime.UtcNow > expireAt)
                return false;

            // Cập nhật mật khẩu mới
            var member = this.CreateMemberFromDataRow(row);
            member.Password = newPassword;
            member.Field3 = null; // xoá mã
            member.Field4 = null; // xoá thời gian

            await this.UpdateAsync(member);
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var members = await _memberRepository.SelectAllDynamicWhereAsync(
                null, null!, null!, null!, null!, email,
                null!, null!, null!, null!, null,
                null!, null!, null!, null!, null!,
                null!, null!, null!, null!, null!,
                null!, null! // ✅ thêm 2 tham số: refreshToken, refreshTokenExpiryTime
            );

            if (members.Rows.Count == 0)
                return false;

            var row = members.Rows[0];

            var verificationService = new VerificationService();
            string resetCode = verificationService.GenerateVerificationCode();
            string expireAt = DateTime.UtcNow.AddMinutes(10).ToString("o");

            // Gửi email
            string userEmail = row["Email"].ToString()!;
            await emailService.SendResetPasswordEmailAsync(userEmail, resetCode);

            // Lưu mã khôi phục và thời gian hết hạn (Field3 & Field4)
            var member = this.CreateMemberFromDataRow(row);
            member.Field3 = resetCode;
            member.Field4 = expireAt;

            await this.UpdateAsync(member);
            return true;
        }

        public async Task<string?> RefreshTokenAsync(string refreshToken, string deviceInfo)
        {
            // Lấy token từ bảng MemberRefreshTokens
            var tokens = await _memberRefreshTokensBusinessLayer.SelectByRefreshTokenAsync(refreshToken);

            if (tokens.Rows.Count == 0)
            {
                Console.WriteLine("❌ Refresh token không tồn tại.");
                return null;
            }

            var row = tokens.Rows[0];

            // Nếu cần kiểm tra đúng device
            if (!string.IsNullOrEmpty(deviceInfo) && row["DeviceInfo"]?.ToString() != deviceInfo)
            {
                Console.WriteLine("❌ DeviceInfo không khớp.");
                return null;
            }

            int memberId = Convert.ToInt32(row["MemberId"]);
            DateTime expiry = Convert.ToDateTime(row["RefreshTokenExpiry"]);

            if (expiry < DateTime.UtcNow)
            {
                Console.WriteLine("❌ Refresh token đã hết hạn.");
                return null;
            }

            // Lấy thông tin member từ bảng Member
            var memberData = await _memberRepository.SelectByPrimaryKeyAsync(memberId);
            if (memberData.Rows.Count == 0)
            {
                Console.WriteLine("❌ Không tìm thấy thành viên.");
                return null;
            }

            var member = CreateMemberFromDataRow(memberData.Rows[0]);

            // Tạo mới access token
            string newAccessToken = _jwtTokenGenerator.GenerateToken(member.MemberId, member.Username!, member.Role!);
            Console.WriteLine("✅ Tạo mới access token từ bảng MemberRefreshTokens.");
            return newAccessToken;
        }

        public async Task LogoutAsync(int memberId)
        {
            await _memberRepository.RevokeRefreshTokenAsync(memberId);
        }

        private List<Member> GetListOfMember(DataTable dt)
        {
            List<Member> objMemberList = null!;

            // build the list of ThiSinhs
            if (dt != null && dt.Rows.Count > 0)
            {
                objMemberList = new List<Member>();

                foreach (DataRow dr in dt.Rows)
                {
                    Member member = this.CreateMemberFromDataRow(dr);
                    objMemberList.Add(member);
                }
            }

            return objMemberList;
        }

        private Member CreateMemberFromDataRow(DataRow dr)
        {
            Member member = new Member();
            
            member.MemberId = (int)dr["MemberId"];

            if (dr["FirstName"] != System.DBNull.Value)
                member.FirstName = dr["FirstName"].ToString();
            else
                member.FirstName = null;

            if (dr["MiddleName"] != System.DBNull.Value)
                member.MiddleName = dr["MiddleName"].ToString();
            else
                member.MiddleName = null;

            if (dr["LastName"] != System.DBNull.Value)
                member.LastName = dr["LastName"].ToString();
            else
                member.LastName = null;

            if (dr["Phone"] != System.DBNull.Value)
                member.Phone = dr["Phone"].ToString();
            else
                member.Phone = null;

            if (dr["Email"] != System.DBNull.Value)
                member.Email = dr["Email"].ToString();
            else
                member.Email = null;

            if(dr["Facebook"] != System.DBNull.Value)
                member.Facebook = dr["Facebook"].ToString();
            else
                member.Facebook = null;

            if (dr["Address"] != System.DBNull.Value)
                member.Address = dr["Address"].ToString();
            else
                member.Address = null;

            if (dr["Type"] != System.DBNull.Value)
                member.Type = dr["Type"].ToString();
            else
                member.Type = null;

            if (dr["Avatar"] != System.DBNull.Value)
                member.Avatar = dr["Avatar"].ToString();
            else
                member.Avatar = null;

            if (dr["NumberPlayer"] != System.DBNull.Value)
                member.NumberPlayer = (int)dr["NumberPlayer"];
            else
                member.NumberPlayer = null;

            if (dr["Role"] != System.DBNull.Value)
                member.Role = dr["Role"].ToString();
            else
                member.Role = null;

            if (dr["Username"] != System.DBNull.Value)
                member.Username = dr["Username"].ToString();
            else
                member.Username = null;

            if (dr["Password"] != System.DBNull.Value)
                member.Password = dr["Password"].ToString();
            else
                member.Password = null;

            if (dr["Field1"] != System.DBNull.Value)
                member.Field1 = dr["Field1"].ToString();
            else
                member.Field1 = null;

            if (dr["Field2"] != System.DBNull.Value)
                member.Field2 = dr["Field2"].ToString();
            else
                member.Field2 = null;

            if (dr["Field3"] != System.DBNull.Value)
                member.Field3 = dr["Field3"].ToString();
            else
                member.Field3 = null;

            if (dr["Field4"] != System.DBNull.Value)
                member.Field4 = dr["Field4"].ToString();
            else
                member.Field4 = null;

            if (dr["Field5"] != System.DBNull.Value)
                member.Field5 = dr["Field5"].ToString();
            else
                member.Field5 = null;


            if (dr["RefreshToken"] != System.DBNull.Value)
                member.RefreshToken = dr["RefreshToken"].ToString();
            else
                member.RefreshToken = null;

            if (dr["RefreshTokenExpiry"] != System.DBNull.Value)
                member.RefreshTokenExpiryTime = (DateTime)dr["RefreshTokenExpiry"];
            else
                member.RefreshTokenExpiryTime = null;

            if (dr["CreatedAt"] != System.DBNull.Value)
                member.CreatedAt = (DateTime)dr["CreatedAt"];
            else
                member.CreatedAt = null;

            if (dr["Flag"] != System.DBNull.Value)
                member.Flag = dr["Flag"].ToString();
            else
                member.Flag = null;

            return member;
        }
    }
}
