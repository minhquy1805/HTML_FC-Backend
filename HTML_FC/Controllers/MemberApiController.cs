using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using LIBCORE.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HTML_FC.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public partial class MemberApiController : ControllerBase
    {
        private Member _Member;
        private readonly IMemberBusinessLayer _MemberBusinessLayer;

        public MemberApiController(Member Member, IMemberBusinessLayer MemberBusinessLayer)
        {
            _Member = Member;
            _MemberBusinessLayer = MemberBusinessLayer;
        }

        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Member model, bool isForListInlineOrListCrud = false)
        {
            try
            {
                var result = await AddEditAsync(model, CrudOperation.Add, isForListInlineOrListCrud);
                return result;
            }
            catch (Exception ex)
            {
                // Trả lỗi rõ ràng nếu là trùng Email hoặc Username
                if (ex.Message.Contains("Email") || ex.Message.Contains("Username"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                // Các lỗi khác
                return StatusCode(500, new
                {
                    message = "🚨 Có lỗi xảy ra trong quá trình xử lý.",
                    detail = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Member model, bool isForListInlineOrListCrud = false)
        {
            // update existing record
            return await this.AddEditAsync(model, CrudOperation.Update, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "User")]
        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Member model)
        {
            await _MemberBusinessLayer.UpdateProfileAsync(model);
            return Ok("✅ Cập nhật thông tin cá nhân thành công.");
        }


        [Authorize(Roles = "Admin")]
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete a record based on id(s)
                await _MemberBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            Console.WriteLine("✅ API nhận request login cho username: " + request.Username);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                Console.WriteLine("❌ Thiếu username hoặc password!");
                return BadRequest(new { message = "Username and password are required" });
            }

            string? jsonResult = await _MemberBusinessLayer.LoginAsync(request.Username, request.Password, request.DeviceInfo!);

            if (jsonResult == null)
            {
                Console.WriteLine("❌ Sai username hoặc password!");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // ✅ Parse chuỗi JSON trả về từ BusinessLayer
            var tokens = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResult);

            Console.WriteLine("✅ Đăng nhập thành công! Trả về token và refresh token.");
            return Ok(tokens); // Trả về { token: "...", refreshToken: "..." }
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Missing refresh token" });
            }

            string? newAccessToken = await _MemberBusinessLayer.RefreshTokenAsync(request.RefreshToken, request.DeviceInfo!);

          

            if (newAccessToken == null)
            {
                return Unauthorized(new { message = "Refresh token is invalid or expired" });
            }

            return Ok(new { accessToken = newAccessToken });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            bool result = await _MemberBusinessLayer.VerifyEmailAsync(request.Email, request.Code);

            if (result)
                return Ok("✅ Xác thực thành công!");

            return BadRequest("❌ Mã xác thực không đúng hoặc email không tồn tại.");
        }

        [HttpPost("resend-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("❌ Email không được để trống.");

            bool result = await _MemberBusinessLayer.ResendVerificationCodeAsync(email);

            if (result)
                return Ok("✅ Mã xác thực mới đã được gửi tới email.");
            else
                return BadRequest("❌ Email không tồn tại hoặc đã được xác thực.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var success = await _MemberBusinessLayer.ForgotPasswordAsync(email);
            if (!success) return BadRequest("Email không tồn tại.");
            return Ok("Mã đặt lại mật khẩu đã được gửi đến email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var success = await _MemberBusinessLayer.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
            if (!success) return BadRequest("Mã không đúng hoặc đã hết hạn.");
            return Ok("Mật khẩu đã được cập nhật thành công.");
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var memberId))
                return BadRequest(new { message = "Invalid token" });

            await _MemberBusinessLayer.LogoutAsync(memberId);
            return Ok(new { message = "Logged out successfully" });
        }

        private async Task<IActionResult> AddEditAsync(Member model, CrudOperation operation, bool isForListInlineOrListCrud = false)
        {
            try
            {
                if (operation == CrudOperation.Add)
                    _Member = new();
                else
                    _Member = await _MemberBusinessLayer.SelectByPrimaryKeyAsync(model.MemberId);

                _Member.FirstName = model.FirstName;
                _Member.MiddleName = model.MiddleName;
                _Member.LastName = model.LastName;

                _Member.Phone = model.Phone;
                _Member.Email = model.Email;
                _Member.Facebook = model.Facebook;
                _Member.Address = model.Address;
                _Member.Type = model.Type;
                _Member.Avatar = model.Avatar;
                _Member.NumberPlayer = model.NumberPlayer;
                _Member.Role = model.Role;
                _Member.Username = model.Username;
                _Member.Password = model.Password;
                _Member.Field1 = model.Field1;
                _Member.Field2 = model.Field2;
                _Member.Field3 = model.Field3;
                _Member.Field4 = model.Field4;
                _Member.Field5 = model.Field5;
                _Member.CreatedAt = model.CreatedAt;
                _Member.Flag = model.Flag;

                if (operation == CrudOperation.Add)
                    await _MemberBusinessLayer.InsertAsync(_Member);
                else
                    await _MemberBusinessLayer.UpdateAsync(_Member);


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
