using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HTML_FC.ApiControllers
{
    [Route("api/v1/")]
    [ApiController]
    public partial class MemberApiController : ControllerBase
    {
        private Member _member;
        private readonly IMemberBusinessLayer _memberBusinessLayer;
        private readonly IMemberRefreshTokensBusinessLayer _memberRefreshTokensBusinessLayer;

        public MemberApiController(Member member,IMemberBusinessLayer memberBusinessLayer, IMemberRefreshTokensBusinessLayer memberRefreshTokensBusinessLayer)
        {
            _member = member;
            _memberBusinessLayer = memberBusinessLayer;
            _memberRefreshTokensBusinessLayer = memberRefreshTokensBusinessLayer;
        }

        [Authorize(Roles = "Admin,User")]
        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<Member> SelectByPrimaryKey(int id)
        {
            Member member = await _memberBusinessLayer.SelectByPrimaryKeyAsync(id);
            return member;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<Member>> SelectAll()
        {
            List<Member> members;

            members = await _memberBusinessLayer.SelectAllAsync();

            return members;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]
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
             string refreshToken,                 // ✅ thêm
             DateTime? refreshTokenExpiryTime    // ✅ thêm
         )
        {
            List<Member> members = await _memberBusinessLayer.SelectAllDynamicWhereAsync(
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
                refreshToken,               // ✅ thêm
                refreshTokenExpiryTime      // ✅ thêm
            );

            return members;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("devices")]
        public async Task<IActionResult> GetAllDevices([FromQuery] int memberId)
        {
            if (memberId <= 0)
                return BadRequest(new { message = "memberId là bắt buộc." });

            var dt = await _memberRefreshTokensBusinessLayer.SelectByMemberIdAsync(memberId);

            if (dt.Rows.Count == 0)
                return NotFound(new { message = "Không có thiết bị nào." });

            var devices = dt.AsEnumerable()
                .Select(row => new
                {
                    memberRefreshTokensId = row["MemberRefreshTokensId"],
                    deviceInfo = row["DeviceInfo"]?.ToString(),
                    refreshToken = row["RefreshToken"]?.ToString(),
                    refreshTokenExpiry = row["RefreshTokenExpiry"]?.ToString(),
                    createdAt = row["CreatedAt"]?.ToString(),
                    flag = row["Flag"]?.ToString()
                }).ToList();

            return Ok(devices);
        }
    }
}
