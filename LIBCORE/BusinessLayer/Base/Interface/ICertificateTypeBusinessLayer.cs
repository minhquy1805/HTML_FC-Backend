using LIBCORE.Models;

namespace LIBCORE.BusinessLayer
{
    public partial interface ICertificateTypeBusinessLayer
    {
        public Task<CertificateType> SelectByPrimaryKeyAsync(int certificateTypeId);
        public Task<List<CertificateType>> SelectAllAsync();

        public Task<List<CertificateType>> SelectAllDynamicWhereAsync(int? certificateTypeId, string certificateTitle);

        public Task DeleteAsync(int certificateTypeId);


        public Task<int> InsertAsync(CertificateType certificateType);


        public Task UpdateAsync(CertificateType certificateType);
    }
}
