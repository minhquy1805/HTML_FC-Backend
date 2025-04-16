using LIBCORE.DataRepository.Helper;
using LIBCORE.Models;
using Microsoft.Data.SqlClient;
using System.Data;


namespace LIBCORE.DataRepository
{
    public partial class CertificateRepository : ICertificateRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal CertificateRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<DataTable> ICertificateRepository.SelectByPrimaryKeyAsync(int certificateId)
        {
            string storedProcedure = "[dbo].[Certificate_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateId", certificateId);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> ICertificateRepository.SelectAllAsync()
        {
            string storedProcedure = "[dbo].[Certificate_SelectAll]";
            return await this.SelectSharedAsync(storedProcedure, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> ICertificateRepository.SelectAllDynamicWhereAsync(int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            string storedProcedure = "[dbo].[Certificate_SelectAllDynamicWhere]";
            List<SqlParameter> sqlParamList = new();

            AddSearchCommandParamsShared(sqlParamList, certificateId, certificateTypeId, title, contentCert, dateCert, signCert, reasonCert, field1, field2, field3, field4, field5, flag);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> ICertificateRepository.SelectAllCertificateByCertificateTypeId(int certificateTypeId)
        {
            string storedProcedure = "[dbo].[Certificate_SelectAllByCerificateTypeId]";
            return await this.SelectSharedAsync(storedProcedure, "certificateTypeId", certificateTypeId, null!, null, null);
        }

        async Task ICertificateRepository.DeleteAsync(int certificateId)
        {
            string storedProcedure = "[dbo].[Certificate_Delete]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateId", certificateId);

            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        async Task<int> ICertificateRepository.InsertAsync(Certificate certificate)
        {
            return await this.InsertUpdateAsync(certificate, DatabaseOperationType.Create);
        }

        async Task ICertificateRepository.UpdateAsync(Certificate certificate)
        {
            await this.InsertUpdateAsync(certificate, DatabaseOperationType.Update);
        }

        private async Task<int> InsertUpdateAsync(Certificate certificate, DatabaseOperationType operationType) 
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedCertificateId = certificate.CertificateId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[Certificate_Update]";
            else
                storedProcedure = "[dbo].[Certificate_Insert]";

            object? certificateTypeId = certificate.CertificateTypeId;
            object? title = certificate.Title;
            object? contentCert = certificate.ContentCert;
            object? dateCert = certificate.DateCert;
            object? signCert = certificate.SignCert;
            object? reasonCert = certificate.ReasonCert;
            object? field1 = certificate.Field1;
            object? field2 = certificate.Field2;
            object? field3 = certificate.Field3;
            object? field4 = certificate.Field4;
            object? field5 = certificate.Field5;
            object? flag = certificate.Flag;

            if(certificate.CertificateTypeId is null)
                certificateTypeId = DBNull.Value;

            if(certificate.Title is null)
                title = DBNull.Value;

            if(certificate.ContentCert is null) 
                contentCert = DBNull.Value;

            if(certificate.DateCert is null)
                dateCert = DBNull.Value;

            if(certificate.SignCert is null)
                signCert = DBNull.Value;

            if(certificate.ReasonCert is null)
                reasonCert = DBNull.Value;

            if(certificate.Field1 is null)
                field1 = DBNull.Value;

            if(certificate.Field2 is null)
                field2 = DBNull.Value;

            if(certificate.Field3 is null)
                field3 = DBNull.Value;

            if(certificate.Field4 is null)
                field4 = DBNull.Value;

            if(certificate.Field5 is null)
                field5 = DBNull.Value;

            if(certificate.Flag is null)
                flag = DBNull.Value;

            if(operationType == DatabaseOperationType.Update)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateId", certificate.CertificateId);

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateTypeId!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@contentCert", contentCert!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@dateCert", dateCert!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@signCert", signCert!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@reasonCert", reasonCert!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag!);

            if (operationType == DatabaseOperationType.Update)
            {
                await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Update);
            }
            else
            {
                var result = await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
                newlyCreatedCertificateId = (int)(result ?? 0); // Default to 0 if null returned
            }
            return newlyCreatedCertificateId;
        }

        private async Task<DataTable> SelectSharedAsync(string storedProcName, string parameterName, object parameterValue, string sortByExpression, int? startRowIndex, int? rows)
        {
            List<SqlParameter> sqlParamList = new();

            // select, skip, take, sort sql parameters
            if (!String.IsNullOrEmpty(sortByExpression) && startRowIndex is not null && rows is not null)
                DatabaseFunctions.AddSelectSkipAndTakeParams(sqlParamList, sortByExpression, startRowIndex.Value, rows.Value);

            if (parameterName is not null)
                this.SetForeignKeySqlParameter(sqlParamList, parameterName, parameterValue);

            // get and return the data
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? certificateId, int? certificateTypeId, string title, string contentCert, DateTime? dateCert, string signCert, string reasonCert, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            if (certificateId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateId", certificateId.Value);

            if (certificateTypeId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateTypeId.Value);

            if (!String.IsNullOrEmpty(title))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title);

            if (!String.IsNullOrEmpty(contentCert))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@contentCert", contentCert);

            if (dateCert is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@dateCert", dateCert.Value);

            if (!String.IsNullOrEmpty(signCert))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@signCert", signCert);

            if (!String.IsNullOrEmpty(reasonCert))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@reasonCert", reasonCert);

            if (!String.IsNullOrEmpty(field1))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1);

            if (!String.IsNullOrEmpty(field2))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2);

            if (!String.IsNullOrEmpty(field3))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3);

            if (!String.IsNullOrEmpty(field4))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4);

            if (!String.IsNullOrEmpty(field5))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5);

            if (!String.IsNullOrEmpty(flag))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag);
        }

        private void SetForeignKeySqlParameter(List<SqlParameter> sqlParamList, string parameterName, object parameterValue)
        {
            // add the (foreign key) sql parameters
            if (!String.IsNullOrEmpty(parameterName))
            {
                if (parameterValue is null)
                    parameterValue = DBNull.Value;

                switch (parameterName)
                {
                    case "certificateTypeId":
                        DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", parameterValue);
                        break;
                    
                    default:
                        break;
                }
            }
        }
    }
}
