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
    public class CertificateApiController : ControllerBase
    {
        private Certificate _Certificate;
        private readonly ICertificateBusinessLayer _CertificateBusinessLayer;

        public CertificateApiController(Certificate Certificate, ICertificateBusinessLayer CertificateBusinessLayer)
        {
            _Certificate = Certificate;
            _CertificateBusinessLayer = CertificateBusinessLayer;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Certificate certificate, bool isForListInlineOrListCrud = false)
        {
            return await AddEditAsync(certificate, CrudOperation.Add, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Certificate certificate, bool isForListInlineOrListCrud = false)
        {
            // update existing record
            return await this.AddEditAsync(certificate, CrudOperation.Update, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete a record based on id(s)
                await _CertificateBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        private async Task<IActionResult> AddEditAsync(Certificate certificate, CrudOperation operation, bool isForListInlineOrListCrud = false)
        { 
            try
            {
                if (operation == CrudOperation.Add)
                    _Certificate = new ();
                else
                    _Certificate = await _CertificateBusinessLayer.SelectByPrimaryKeyAsync(certificate.CertificateId);

                _Certificate.CertificateTypeId = certificate.CertificateTypeId;
                _Certificate.Title = certificate.Title;
                _Certificate.ContentCert = certificate.ContentCert;
                _Certificate.DateCert = certificate.DateCert;
                _Certificate.SignCert = certificate.SignCert;
                _Certificate.ReasonCert = certificate.ReasonCert;
                _Certificate.Field1 = certificate.Field1;
                _Certificate.Field2 = certificate.Field2;
                _Certificate.Field3 = certificate.Field3;
                _Certificate.Field4 = certificate.Field4;
                _Certificate.Field5 = certificate.Field5;
                _Certificate.Flag = certificate.Flag;

                if(operation == CrudOperation.Add)
                    await _CertificateBusinessLayer.InsertAsync(_Certificate);
                else
                    await _CertificateBusinessLayer.UpdateAsync(_Certificate);

                return Ok();

            } 
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
