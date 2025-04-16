using LIBCORE.BusinessLayer;
using LIBCORE.Domain;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class InExpenApiController : ControllerBase
    {
        private InExpen _InExpen;
        private readonly IInExpenBusinessLayer _InExpenBusinessLayer;

        public InExpenApiController(InExpen InExpen, IInExpenBusinessLayer InExpenBusinessLayer)
        {
            _InExpen = InExpen;
            _InExpenBusinessLayer = InExpenBusinessLayer;
        }

        [Authorize(Roles = "User")]
        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] InExpen inExpen, bool isForListInlineOrListCrud = false)
        {
            return await AddEditAsync(inExpen, CrudOperation.Add, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "User")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] InExpen inExpen, bool isForListInlineOrListCrud = false)
        {
            // update existing record
            return await this.AddEditAsync(inExpen, CrudOperation.Update, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "User")]
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete a record based on id(s)
                await _InExpenBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        private async Task<IActionResult> AddEditAsync(InExpen inExpen, CrudOperation operation, bool isForListInlineOrListCrud = false)
        {
            try
            {
                if (operation == CrudOperation.Add)
                    _InExpen = new();
                else
                    _InExpen = await _InExpenBusinessLayer.SelectByPrimaryKeyAsync(inExpen.InExpenId);

                _InExpen.MemberId = inExpen.MemberId;
                _InExpen.TransactionTime = inExpen.TransactionTime;
                _InExpen.MoneyValue = inExpen.MoneyValue;
                _InExpen.Type = inExpen.Type;
                _InExpen.Description = inExpen.Description;
                _InExpen.CreatedAt = inExpen.CreatedAt;
                _InExpen.FileAttach = inExpen.FileAttach;
                _InExpen.Field1 = inExpen.Field1;
                _InExpen.Field2 = inExpen.Field2;
                _InExpen.Field3 = inExpen.Field3;
                _InExpen.Field4 = inExpen.Field4;
                _InExpen.Field5 = inExpen.Field5;
                _InExpen.Flag = inExpen.Flag;

                if (operation == CrudOperation.Add)
                    await _InExpenBusinessLayer.InsertAsync(_InExpen);
                else
                    await _InExpenBusinessLayer.UpdateAsync(_InExpen);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
