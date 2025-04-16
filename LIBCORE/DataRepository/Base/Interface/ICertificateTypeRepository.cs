using LIBCORE.Models;
using System.Data;

namespace LIBCORE.DataRepository
{
    public partial interface ICertificateTypeRepository
    {
        internal Task<DataTable> SelectByPrimaryKeyAsync(int certificateTypeId);

        internal Task<DataTable> SelectAllAsync();

        internal Task DeleteAsync(int certificateTypeId);

        internal Task<DataTable> SelectAllDynamicWhereAsync(int? certificateTypeId, string certificateTitle);

        internal Task<int> InsertAsync(CertificateType objCertificateType);

        internal Task UpdateAsync(CertificateType objCertificateType);
    }
}
