using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LIBCORE.Models;
using LIBCORE.BusinessLayer;
using Microsoft.AspNetCore.Authorization;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/")]
    [ApiController]
    public class CertificateTypeApiController : ControllerBase
    {
        private CertificateType _certificateType;
        private readonly ICertificateTypeBusinessLayer _certificateTypeBusinessLayer;

        public CertificateTypeApiController(CertificateType certificateType, ICertificateTypeBusinessLayer certificateTypeBusinessLayer)
        {
            _certificateType = certificateType;
            _certificateTypeBusinessLayer = certificateTypeBusinessLayer;
        }

        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<CertificateType> SelectByPrimaryKey(int id)
        {
            CertificateType certificateType = await _certificateTypeBusinessLayer.SelectByPrimaryKeyAsync(id);
            return certificateType;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<CertificateType>> SelectAll()
        {
            List<CertificateType> certificateTypes;

            certificateTypes = await _certificateTypeBusinessLayer.SelectAllAsync();

            return certificateTypes;
        }

        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]
        public async Task<List<CertificateType>> SelectAllDynamicWhereAsync(int? certificateTypeId, string certificateTitle)
        {
           List<CertificateType> certificateTypes = await _certificateTypeBusinessLayer.SelectAllDynamicWhereAsync(certificateTypeId, certificateTitle);
            return certificateTypes;
        }
    }
}
