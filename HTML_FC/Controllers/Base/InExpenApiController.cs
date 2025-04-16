using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/")]
    [ApiController]
    public class InExpenApiController : ControllerBase
    {
        private InExpen _inExpen;
        private readonly IInExpenBusinessLayer _inExpenBusinessLayer;

        public InExpenApiController(InExpen inExpen, IInExpenBusinessLayer inExpenBusinessLayer)
        {
            _inExpen = inExpen;
            _inExpenBusinessLayer = inExpenBusinessLayer;
        }

        [Authorize(Roles = "User")]
        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<InExpen> SelectByPrimaryKey(int id)
        {
            InExpen inExpen = await _inExpenBusinessLayer.SelectByPrimaryKeyAsync(id);
            return inExpen;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<InExpen>> SelectAll()
        {
            List<InExpen> inExpens;

            inExpens = await _inExpenBusinessLayer.SelectAllAsync();

            return inExpens;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]
        public async Task<List<InExpen>> SelectAllDynamucWhereAsync(int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            List<InExpen> inExpens = await _inExpenBusinessLayer.SelectAllDynamicWhereAsync(inExpenId, memberId, transactionTime, moneyValue, type, description, createdAt, fileAttach, field1, field2, field3, field4, field5, flag);
            return inExpens;
        }

        [Route("[controller]/selectallbyMemberId")]
        [HttpGet]
        public async Task<List<InExpen>> SelectAllByMemberId(int memberId)
        {
            List<InExpen> inExpens = await _inExpenBusinessLayer.SelectAllByMemberId(memberId);
            return inExpens;
        }
    }
}
