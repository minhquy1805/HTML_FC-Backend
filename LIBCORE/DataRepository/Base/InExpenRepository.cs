using LIBCORE.DataRepository.Helper;
using LIBCORE.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial class InExpenRepository : IInExpenRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal InExpenRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<DataTable> IInExpenRepository.SelectByPrimaryKeyAsync(int inExpenId)
        {
            string storedProcedure = "[dbo].[InExpen_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@inExpenId", inExpenId);
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> IInExpenRepository.SelectAllAsync()
        {
            string storedProcedure = "[dbo].[InExpen_SelectAll]";
            return await this.SelectSharedAsync(storedProcedure, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> IInExpenRepository.SelectAllDynamicWhereAsync(int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            string storedProcedure = "[dbo].[InExpen_SelectAllWhereDynamic]";
            List<SqlParameter> sqlParamList = new();

            AddSearchCommandParamsShared(sqlParamList, inExpenId, memberId, transactionTime, moneyValue, type, description, createdAt, fileAttach, field1, field2, field3, field4, field5, flag);
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> IInExpenRepository.SelectAllByMemberId(int memberId)
        {
            string storedProcedure = "[dbo].[InExpen_SelectAllByMemberId]";
            return await this.SelectSharedAsync(storedProcedure, "memberId", memberId, null!, null, null);
        }

        async Task IInExpenRepository.DeleteAsync(int inExpenId)
        {
            string storedProcedure = "[dbo].[InExpen_Delete]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@inExpenId", inExpenId);

            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        async Task<int> IInExpenRepository.InsertAsync(InExpen inExpen)
        {
            return await this.InsertUpdateAsync(inExpen, DatabaseOperationType.Create);
        }

        async Task IInExpenRepository.UpdateAsync(InExpen inExpen)
        {
            await this.InsertUpdateAsync(inExpen, DatabaseOperationType.Update);
        }

        private async Task<int> InsertUpdateAsync(InExpen inExpen, DatabaseOperationType operationType)
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedInExpenId = inExpen.InExpenId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[InExpen_Update]";
            else
                storedProcedure = "[dbo].[InExpen_Insert]";

            object? memberId = inExpen.MemberId;
            object? transactionTime = inExpen.TransactionTime;
            object? moneyValue = inExpen.MoneyValue;
            object? type = inExpen.Type;
            object? description = inExpen.Description;
            object? createdAt = inExpen.CreatedAt;
            object? fileAttach = inExpen.FileAttach;
            object? field1 = inExpen.Field1;
            object? field2 = inExpen.Field2;
            object? field3 = inExpen.Field3;
            object? field4 = inExpen.Field4;
            object? field5 = inExpen.Field5;
            object? flag = inExpen.Flag;

            if(inExpen.MemberId is null)
                memberId = DBNull.Value;

            if(inExpen.TransactionTime is null)
                transactionTime = DBNull.Value;

            if(inExpen.MoneyValue is null)
                moneyValue = DBNull.Value;

            if(inExpen.Type is null)
                type = DBNull.Value;

            if (inExpen.Description is null)
                description = DBNull.Value;

            if(inExpen.CreatedAt is null)
                createdAt = DBNull.Value;

            if(inExpen.FileAttach is null)
                fileAttach = DBNull.Value;

            if (inExpen.Field1 is null)
                field1 = DBNull.Value;

            if (inExpen.Field2 is null)
                field2 = DBNull.Value;

            if (inExpen.Field3 is null)
                field3 = DBNull.Value;

            if (inExpen.Field4 is null)
                field4 = DBNull.Value;

            if (inExpen.Field5 is null)
                field5 = DBNull.Value;

            if (inExpen.Flag is null)
                flag = DBNull.Value;

            if(operationType == DatabaseOperationType.Update)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@inExpenId", inExpen.InExpenId);

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@memberId", memberId!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@transactionTime", transactionTime!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@moneyValue", moneyValue!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@type", type!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@description", description!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@fileAttach", fileAttach!);
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
                newlyCreatedInExpenId = (int)(result ?? 0); // Default to 0 if null returned
            }
            return newlyCreatedInExpenId;
        }

        private async Task<DataTable> SelectSharedAsync(string storedProcName, string parameterName, object parameterValue, string sortByExpression, int? startRowIndex, int? rows)
        {
            List<SqlParameter> sqlParamList = new();

            if (!String.IsNullOrEmpty(sortByExpression) && startRowIndex is not null && rows is not null)
                DatabaseFunctions.AddSelectSkipAndTakeParams(sqlParamList, sortByExpression, startRowIndex.Value, rows.Value);

            if (parameterName is not null)
                this.SetForeignKeySqlParameter(sqlParamList, parameterName, parameterValue);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            if (inExpenId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "inExpenId", inExpenId.Value);

            if (memberId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "memberId", memberId.Value);

            if (transactionTime is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "transactionTime", transactionTime.Value);

            if (moneyValue is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "moneyValue", moneyValue.Value);

            if(!String.IsNullOrEmpty(type))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "type", type);

            if (!String.IsNullOrEmpty(description))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "description", description);

            if (createdAt is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "createdAt", createdAt);

            if (!String.IsNullOrEmpty(fileAttach))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "fileAttach", fileAttach);

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
            if(!String.IsNullOrEmpty(parameterName))
            { 
                if(parameterValue is null)
                    parameterValue = DBNull.Value;

                switch (parameterName)
                {
                    case "memberId":
                        DatabaseFunctions.AddSqlParameter(sqlParamList, "memberId", parameterValue);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
