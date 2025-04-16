using LIBCORE.Models;
using System.Data;

namespace LIBCORE.BusinessLayer
{
    public partial interface ICertificateBusinessLayer
    {
        public Task<Certificate> SelectByPrimaryKeyAsync(int certificateId);

        public Task<List<Certificate>> SelectAllAsync();

        public Task<List<Certificate>> SelectAllDynamicWhereAsync(int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag);

        public Task<List<Certificate>> SelectAllByCertificateTypeIdAsync(int certificateTypeId);

        public Task<int> InsertAsync(Certificate certificate);

        public Task UpdateAsync(Certificate certificate);

        public Task DeleteAsync(int certificateId);

    }
}
