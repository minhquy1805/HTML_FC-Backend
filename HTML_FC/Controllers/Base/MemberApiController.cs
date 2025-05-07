using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.ApiControllers
{
    [Route("api/v1/")]
    [ApiController]
    public partial class MemberApiController : ControllerBase
    {
        private Member _member;
        private readonly IMemberBusinessLayer _memberBusinessLayer;

        public MemberApiController(Member member,IMemberBusinessLayer memberBusinessLayer)
        {
            _member = member;
            _memberBusinessLayer = memberBusinessLayer;
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
    }
}
