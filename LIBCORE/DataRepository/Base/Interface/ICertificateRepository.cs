using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial interface ICertificateRepository
    {
        internal Task<DataTable> SelectByPrimaryKeyAsync(int certificateId);

        internal Task<DataTable> SelectAllAsync();

        internal Task<DataTable> SelectAllDynamicWhereAsync(int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag);

        internal Task<DataTable> SelectAllCertificateByCertificateTypeId(int certificateTypeId);

        internal Task DeleteAsync(int certificateId);

        internal Task<int> InsertAsync(Certificate objCertificate);

        internal Task UpdateAsync(Certificate objCertificate);
    }
}
