using LIBCORE.DataRepository;
using LIBCORE.Models;
using System.Data;

namespace LIBCORE.BusinessLayer
{
    public partial class CertificateBusinessLayer : ICertificateBusinessLayer
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateTypeBusinessLayer _certificateTypeBusinessLayer;

        public CertificateBusinessLayer(ICertificateRepository certificateRepository, ICertificateTypeBusinessLayer certificateTypeBusinessLayer)
        {
            _certificateRepository = certificateRepository;
            _certificateTypeBusinessLayer = certificateTypeBusinessLayer;
        }

        public async Task<Certificate> SelectByPrimaryKeyAsync(int certificateId)
        {
            DataTable dt = await _certificateRepository.SelectByPrimaryKeyAsync(certificateId);

            // create Certificate
            if (dt is not null && dt.Rows.Count > 0)
                return await this.CreateCertificateFromDataRow(dt.Rows[0]);

            return null!;
        }

        public async Task<List<Certificate>> SelectAllAsync()
        {
            DataTable dt = await _certificateRepository.SelectAllAsync();
            return await this.GetListOfCertificate(dt);
        }

        public async Task<List<Certificate>> SelectAllDynamicWhereAsync(int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            DataTable dt = await _certificateRepository.SelectAllDynamicWhereAsync(certificateId, certificateTypeId, title, contentCert, dateCert, signCert, reasonCert, field1, field2, field3, field4, field5, flag);
            return await this.GetListOfCertificate(dt);
        }

        public async Task<List<Certificate>> SelectAllByCertificateTypeIdAsync(int certificateTypeId)
        {
            DataTable dt = await _certificateRepository.SelectAllCertificateByCertificateTypeId(certificateTypeId);
            return await this.GetListOfCertificate(dt);
        }

        public async Task<int> InsertAsync(Certificate certificate)
        {
            return await _certificateRepository.InsertAsync(certificate);
        }

        public async Task UpdateAsync(Certificate certificate)
        {
            await _certificateRepository.UpdateAsync(certificate);
        }

        public async Task DeleteAsync(int certificateId)
        {
            await _certificateRepository.DeleteAsync(certificateId);
        }

        private async Task<List<Certificate>> GetListOfCertificate(DataTable dt)
        {
            List<Certificate> certificates = null!;


            if (dt != null && dt.Rows.Count > 0)
            {
                certificates = new List<Certificate>();

                foreach (DataRow dr in dt.Rows)
                {
                    Certificate certificate = await this.CreateCertificateFromDataRow(dr);
                    certificates.Add(certificate);
                }
            }

            return certificates;
        }

        private async Task<Certificate> CreateCertificateFromDataRow(DataRow dr)
        {
            Certificate certificate = new();

            certificate.CertificateId = Convert.ToInt32(dr["CertificateId"]);

            if(dr["CertificateTypeId"] != DBNull.Value)
            {
                int certificateTypeId = (int)dr["CertificateTypeId"];
                certificate.CertificateTypeId = certificateTypeId;
                certificate.CertificateType = await _certificateTypeBusinessLayer.SelectByPrimaryKeyAsync(certificateTypeId);
            }
            else
            {
                certificate.CertificateTypeId = null;
                certificate.CertificateType = null;
            }

            if(dr["Title"] != DBNull.Value)
                certificate.Title = dr["Title"].ToString();
            else
                certificate.Title = null;

            if(dr["ContentCert"] != DBNull.Value)
                certificate.ContentCert = dr["ContentCert"].ToString();
            else
                certificate.ContentCert = null;

            if(dr["DateCert"] != DBNull.Value)
                certificate.DateCert = Convert.ToDateTime(dr["DateCert"]);
            else
                certificate.DateCert = null;

            if(dr["SignCert"] != DBNull.Value)
                certificate.SignCert = dr["SignCert"].ToString();
            else
                certificate.SignCert = null;

            if(dr["ReasonCert"] != DBNull.Value)
                certificate.ReasonCert = dr["ReasonCert"].ToString();
            else
                certificate.ReasonCert = null;

            if(dr["Field1"] != DBNull.Value)
                certificate.Field1 = dr["Field1"].ToString();
            else
                certificate.Field1 = null;

            if(dr["Field2"] != DBNull.Value)
                certificate.Field2 = dr["Field2"].ToString();
            else
                certificate.Field2 = null;

            if(dr["Field3"] != DBNull.Value)
                certificate.Field3 = dr["Field3"].ToString();
            else
                certificate.Field3 = null;

            if(dr["Field4"] != DBNull.Value)
                certificate.Field4 = dr["Field4"].ToString();
            else
                certificate.Field4 = null;

            if(dr["Field5"] != DBNull.Value)
                certificate.Field5 = dr["Field5"].ToString();
            else
                certificate.Field5 = null;

            if(dr["Flag"] != DBNull.Value)
                certificate.Flag = dr["Flag"].ToString();
            else
                certificate.Flag = null;

            return certificate;
        }
    }
}
