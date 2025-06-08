using Docker.DotNet.Models;
using LIBCORE.DataRepository;
using LIBCORE.Helper;
using LIBCORE.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Composition;
using System.Data;
using System.Text.Json;

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
            // 1. Validate cơ bản
            MemberValidator.ValidatePassword(member.Password!);
            MemberValidator.ValidateEmail(member.Email!);
            MemberValidator.ValidateUsername(member.Username!);

            // 2. Check trùng email và username
            await MemberValidator.CheckDuplicateAsync(_memberRepository, member.Email!, member.Username!);

            // 3. Hash mật khẩu
            member.Password = PasswordHasher.HashPassword(member.Password!);

            // 4. Gán role mặc định
            member.Role = (member.Email == "minhquy073@gmail.com" || member.Email == "11giakhanh03@gmail.com")
                ? "Admin"
                : "User";

            // 5. Gán trạng thái chưa xác thực
            member.Flag = "F";

            // 6. Sinh mã xác thực email
            var verificationService = new VerificationService();
            string code = verificationService.GenerateVerificationCode();
            member.Field1 = code;
            member.Field2 = DateTime.UtcNow.AddMinutes(10).ToString("o");

            // 7. Gửi mail xác thực
            await emailService.SendVerificationEmailAsync(member.Email!, code);

            // 8. Ghi vào DB
            return await _memberRepository.InsertAsync(member);
        }

        public async Task UpdateAsync(Member member)
        {
            // 1. Lấy dữ liệu gốc từ DB
            var dt = await _memberRepository.SelectByPrimaryKeyAsync(member.MemberId);
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("Không tìm thấy người dùng để cập nhật.");

            var existing = this.CreateMemberFromDataRow(dt.Rows[0]);

            // 2. Hash mật khẩu nếu cần (trước khi merge)
            if (!string.IsNullOrWhiteSpace(member.Password) && !PasswordHasher.IsHashed(member.Password))
            {
                member.Password = PasswordHasher.HashPassword(member.Password);
            }

            // ✅ Log trước khi merge (optional)
            Console.WriteLine("📤 Trước merge:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(existing, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("📤 ✅ CI/CD test thành công!");

            // 3. Gộp dữ liệu từ member → existing
            MemberMerger.Merge(member, existing);

            // ✅ Log sau khi merge (để chắc chắn các field đã được giữ lại hoặc cập nhật đúng)
            Console.WriteLine("✅ Sau merge:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(existing, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

            // 4. Cập nhật DB
            await _memberRepository.UpdateAsync(existing);
        }

        public async Task UpdateProfileAsync (Member member)
        {
            // Lấy dữ liệu gốc từ DB
            var dt = await _memberRepository.SelectByPrimaryKeyAsync(member.MemberId);
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("Không tìm thấy người dùng để cập nhật.");

            var existing = this.CreateMemberFromDataRow(dt.Rows[0]);

            // 🔒 Không cho phép người dùng tự sửa các trường nhạy cảm
            member.Role = null;
            member.Username = null;
            member.Flag = null;
            member.Password = null; // Không cho phép thay mật khẩu ở đây
            member.RefreshToken = null;
            member.RefreshTokenExpiryTime = null;

            MemberMerger.Merge(member, existing);

            // ✅ Cập nhật DB
            await _memberRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int memberId)
        {
            await _memberRepository.DeleteAsync(memberId);
        }

        public async Task<string?> LoginAsync(string username, string password, string deviceInfo)
        {
            // 🔍 1. Tìm thành viên theo username
            var dt = await _memberRepository.SelectByUsernameAsync(username);
            if (dt is null || dt.Rows.Count == 0)
            {
                Console.WriteLine("❌ Không tìm thấy user trong DB!");
                return null;
            }

            var member = this.CreateMemberFromDataRow(dt.Rows[0]);

            // 📧 2. Kiểm tra xác thực email
            if (!LoginHelper.IsEmailVerified(member))
            {
                Console.WriteLine("⚠️ Tài khoản chưa xác thực email.");
                return null;
            }

            // 🔐 3. Kiểm tra bị khóa tạm thời
            if (LoginHelper.IsAccountLocked(member))
            {
                Console.WriteLine("⛔ Tài khoản bị khóa tạm thời do nhập sai quá nhiều lần!");
                return null;
            }

            // 🔑 4. Kiểm tra mật khẩu
            if (!LoginHelper.IsPasswordCorrect(password, member.Password!))
            {
                LoginHelper.IncreaseFailCount(member);
                await UpdateAsync(member);

                Console.WriteLine("❌ Sai mật khẩu!");
                return null;
            }

            // ✅ 5. Đăng nhập thành công → reset lỗi
            LoginHelper.ResetFailCount(member);
            await UpdateAsync(member);

            // 🎟️ 6. Tạo Access & Refresh Token
            string accessToken = _jwtTokenGenerator.GenerateToken(member.MemberId, member.Username!, member.Role!);
            string refreshToken = Guid.NewGuid().ToString();
            DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _memberRepository.UpdateRefreshTokenAsync(member.MemberId, refreshToken, refreshTokenExpiry);

            await _memberRefreshTokensBusinessLayer.InsertAsync(new MemberRefreshToken
            {
                MemberId = member.MemberId,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry,
                DeviceInfo = deviceInfo,
                CreatedAt = DateTime.UtcNow,
                Flag = "A"
            });

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                accessToken,
                refreshToken
            });
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
            // ✅ Cập nhật mật khẩu mới (HASH trước khi gửi repo)
            member.Password = PasswordHasher.HashPassword(newPassword);
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
