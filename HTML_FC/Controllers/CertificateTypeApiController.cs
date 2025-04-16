using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using LIBCORE.Domain;
using Microsoft.AspNetCore.Authorization;


namespace HTML_FC.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class CertificateTypeApiController : ControllerBase
    {
        private CertificateType _CertificateType;
        private readonly ICertificateTypeBusinessLayer _CertificateTypeBusinessLayer;

        public CertificateTypeApiController(CertificateType CertificateType, ICertificateTypeBusinessLayer CertificateTypeBusinessLayer)
        {
            _CertificateType = CertificateType;
            _CertificateTypeBusinessLayer = CertificateTypeBusinessLayer;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] CertificateType model, bool isForListInlineOrListCrud = false)
        {
            return await AddEditAsync(model, CrudOperation.Add, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] CertificateType model, bool isForListInlineOrListCrud = false)
        {
            // update existing record
            return await this.AddEditAsync(model, CrudOperation.Update, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete a record based on id(s)
                await _CertificateTypeBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        private async Task<IActionResult> AddEditAsync(CertificateType model, CrudOperation operation, bool isForListInlineOrListCrud = false)
        {
            try
            {
                if (operation == CrudOperation.Add)
                    _CertificateType = new();
                else
                    _CertificateType = await _CertificateTypeBusinessLayer.SelectByPrimaryKeyAsync(model.CertificateTypeId);

                _CertificateType.CertificateTitle = model.CertificateTitle;

                if (operation == CrudOperation.Add)
                    await _CertificateTypeBusinessLayer.InsertAsync(_CertificateType);
                else
                    await _CertificateTypeBusinessLayer.UpdateAsync(_CertificateType);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
