

using LIBCORE.DataRepository;
using LIBCORE.Models;
using System.Data;

namespace LIBCORE.BusinessLayer
{
    public partial class CertificateTypeBusinessLayer : ICertificateTypeBusinessLayer
    {
        private readonly ICertificateTypeRepository _certificateTypeRepository;

        public CertificateTypeBusinessLayer(ICertificateTypeRepository certificateTypeRepository)
        {
            _certificateTypeRepository = certificateTypeRepository;
        }

        public async Task<CertificateType> SelectByPrimaryKeyAsync(int certificateTypeId)
        {
            DataTable dt = await _certificateTypeRepository.SelectByPrimaryKeyAsync(certificateTypeId);

            // create ThiSinh
            if (dt is not null && dt.Rows.Count > 0)
                return this.CreateThiSinhFromDataRow(dt.Rows[0]);

            return null!;
        }

        public async Task<List<CertificateType>> SelectAllAsync()
        {
            DataTable dt = await _certificateTypeRepository.SelectAllAsync();
            return this.GetListOfThiSinh(dt);
        }

        public async Task<List<CertificateType>> SelectAllDynamicWhereAsync(int? certificateTypeId, string certificateTitle)
        {
            DataTable dt = await _certificateTypeRepository.SelectAllDynamicWhereAsync(certificateTypeId, certificateTitle);
            return this.GetListOfThiSinh(dt);
        }

        public async Task<int> InsertAsync(CertificateType certificateType)
        {
            return await _certificateTypeRepository.InsertAsync(certificateType);
        }

        public async Task UpdateAsync(CertificateType certificateType)
        {
            await _certificateTypeRepository.UpdateAsync(certificateType);
        }

        public async Task DeleteAsync(int certificateTypeId)
        {
            await _certificateTypeRepository.DeleteAsync(certificateTypeId);
        }

        private List<CertificateType>GetListOfThiSinh(DataTable dt)
        {
           List<CertificateType> certificateTypes = null!;

            // build the list of ThiSinhs
            if (dt != null && dt.Rows.Count > 0)
            {
                certificateTypes = new List<CertificateType>();

                foreach (DataRow dr in dt.Rows)
                {
                    CertificateType certificateType = this.CreateThiSinhFromDataRow(dr);
                    certificateTypes.Add(certificateType);
                }
            }

            return certificateTypes;
        }

        private CertificateType CreateThiSinhFromDataRow(DataRow dr)
        {
            CertificateType certificateType = new CertificateType();

            certificateType.CertificateTypeId = (int)dr["CertificateTypeId"];

            if (dr["CertificateTitle"] != DBNull.Value)
                certificateType.CertificateTitle = (string)dr["CertificateTitle"];
            else
                certificateType.CertificateTitle= null;

            return certificateType;
        }
    }
}
