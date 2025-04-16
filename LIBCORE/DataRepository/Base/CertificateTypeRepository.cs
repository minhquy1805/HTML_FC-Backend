using LIBCORE.DataRepository.Helper;
using LIBCORE.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LIBCORE.DataRepository
{
    public partial class CertificateTypeRepository : ICertificateTypeRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal CertificateTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<DataTable> ICertificateTypeRepository.SelectByPrimaryKeyAsync(int certificateTypeId)
        {
            string storedProcedure = "[dbo].[CertificateType_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateTypeId);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> ICertificateTypeRepository.SelectAllAsync()
        {
            string storedProcedure = "[dbo].[CertificateType_SelectAll]";
            return await this.SelectSharedAsync(storedProcedure, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> ICertificateTypeRepository.SelectAllDynamicWhereAsync(int? certificateTypeId, string certificateTitle)
        {
            string storedProcedure = "[dbo].[CertificateType_SelectAllDynamicWhere]";
            List<SqlParameter> sqlParamList = new();

            AddSearchCommandParamsShared(sqlParamList, certificateTypeId, certificateTitle);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task ICertificateTypeRepository.DeleteAsync(int certificateTypeId)
        {
            string storedProcedure = "[dbo].[CertificateType_Delete]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateTypeId);

            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        async Task<int> ICertificateTypeRepository.InsertAsync(CertificateType certificateType)
        {
            return await this.InsertUpdateAsync(certificateType, DatabaseOperationType.Create);
        }

        async Task ICertificateTypeRepository.UpdateAsync(CertificateType certificateType)
        {
             await this.InsertUpdateAsync(certificateType, DatabaseOperationType.Update);
        }

        private async Task<DataTable> SelectSharedAsync(string storedProcName, string parameterName, object parameterValue, string sortByExpression, int? startRowIndex, int? rows)
        {
            List<SqlParameter> sqlParamList = new();

            // select, skip, take, sort sql parameters
            if (!String.IsNullOrEmpty(sortByExpression) && startRowIndex is not null && rows is not null)
                DatabaseFunctions.AddSelectSkipAndTakeParams(sqlParamList, sortByExpression, startRowIndex.Value, rows.Value);

            // get and return the data
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        private async Task<int> InsertUpdateAsync(CertificateType certificateType, DatabaseOperationType operationType)
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();
            int newlyCreatedCertificateTypeId = certificateType.CertificateTypeId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[CertificateType_Update]";
            else
                storedProcedure = "[dbo].[CertificateType_Insert]";

            object? certificateTitle = certificateType.CertificateTitle;

            if(String.IsNullOrEmpty(certificateType.CertificateTitle))
                certificateTitle = DBNull.Value;


            if (operationType == DatabaseOperationType.Update)
            {
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateType.CertificateTypeId);
            }

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTitle", certificateTitle!);

            if (operationType == DatabaseOperationType.Update)
            {
                await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Update);
            }
            else
            {
                var result = await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
                newlyCreatedCertificateTypeId = (int)(result ?? 0); // Default to 0 if null returned
            }

            //Console.WriteLine($"certificateTypeId: {certificateType.CertificateTypeId}, certificateTitle: {certificateType.CertificateTitle}");
            return newlyCreatedCertificateTypeId;
        }

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? certificateTypeId, string certificateTitle)
        {
            if (certificateTypeId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTypeId", certificateTypeId.Value);

            if (!String.IsNullOrEmpty(certificateTitle))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@certificateTitle", certificateTitle);
        }
    }
}

