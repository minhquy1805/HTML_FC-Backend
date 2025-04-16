using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/")]
    [ApiController]
    public class CertificateApiController : ControllerBase
    {
        private Certificate _certificate;
        private readonly ICertificateBusinessLayer _certificateBusinessLayer;

        public CertificateApiController(Certificate certificate,ICertificateBusinessLayer certificateBusinessLayer)
        {
            _certificate = certificate;
            _certificateBusinessLayer = certificateBusinessLayer;
        }

        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<Certificate> SelectByPrimaryKey(int id)
        {
            Certificate certificate = await _certificateBusinessLayer.SelectByPrimaryKeyAsync(id);
            return certificate;
        }

        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<Certificate>> SelectAll()
        {
            List<Certificate> certificates;

            certificates = await _certificateBusinessLayer.SelectAllAsync();

            return certificates;
        }

        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]
        public async Task<List<Certificate>> SelectAllDynamicWhereAsync(int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            List<Certificate> certificates = await _certificateBusinessLayer.SelectAllDynamicWhereAsync(certificateId, certificateTypeId, title, contentCert, dateCert, signCert, reasonCert, field1, field2, field3, field4, field5, flag);
            return certificates;
        }

        [Route("[controller]/selectallbycertificateTypeId")]
        [HttpGet]
        public async Task<List<Certificate>> SelectAllByCertificateTypeId(int certificateTypeId)
        {
            List<Certificate> certificates = await _certificateBusinessLayer.SelectAllByCertificateTypeIdAsync(certificateTypeId);
            return certificates;
        }
    }
}
