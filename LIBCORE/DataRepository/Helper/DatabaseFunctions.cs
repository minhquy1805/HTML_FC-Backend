using Microsoft.Data.SqlClient;
using System.Data;

namespace LIBCORE.DataRepository.Helper
{
    internal static class DatabaseFunctions
    {
        private static MsSqlDatabase GetMsSqlDatabase(string connectionString, string sql, List<SqlParameter> sqlParamList, CommandType commandType) 
        {
            if(commandType == CommandType.TableDirect)
            {
                throw new ArgumentException("Invalid CommandType!  Acceptable types are CommandType.StoredProcedure or CommandType.Text only.");
            }

            // instantiate sqlConnection
            SqlConnection sqlConnection = new(connectionString);

            // instantiate sqlCommand and assign SqlCommand properties
            SqlCommand sqlCommand = new(sql, sqlConnection);
            sqlCommand.CommandType = commandType;

            foreach (var param in sqlParamList)
            {
                sqlCommand.Parameters.AddWithValue(param.ParameterName, param.Value);
            }

            // instatntiate sqlDataAdapter
            SqlDataAdapter sqlDataAdapter = new(sqlCommand);

            // instantiate msSqlDatabase and return it
            MsSqlDatabase msSqlDatabase = new(sqlConnection, sqlCommand, sqlDataAdapter);
            return msSqlDatabase;
        }

        internal static async Task<DataTable> GetDataTableAsync(string connectionString, string sql, List<SqlParameter> sqlParamList, CommandType commandType)
        {
            // make sure command type is valid
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("Invalid CommandType!  Acceptable types are CommandType.StoredProcedure or CommandType.Text only.");

            DataTable dt;
            MsSqlDatabase msSqlDatabase = GetMsSqlDatabase(connectionString, sql, sqlParamList, commandType);

            using (msSqlDatabase.SqlConnection)
            {
                // open SqlConnection to the database
                await msSqlDatabase.SqlConnection.OpenAsync();

                // execute transact-sql command against the database
                using (msSqlDatabase.SqlCommand)
                {
                    // fill the datatable using SqlDataAdapter
                    using (msSqlDatabase.SqlDataAdapter)
                    {
                        dt = new DataTable();
                        msSqlDatabase.SqlDataAdapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        internal static async Task<object?> ExecuteSqlCommandAsync(string connectionString, string sql, List<SqlParameter> sqlParamList, CommandType commandType, DatabaseOperationType operationType, bool? isPrimaryKeyGuid = null)
        {
            // make sure command type is valid
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("Invalid CommandType!  Acceptable types are CommandType.StoredProcedure or CommandType.Text only.");

            if (operationType == DatabaseOperationType.RetrieveDataTable)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create, Update, and Delete only.", "operationType: " + operationType.ToString());

            if (operationType == DatabaseOperationType.Create && isPrimaryKeyGuid == null)
                throw new ArgumentException("Invalid isPrimaryKeyGuid! isPrimaryKeyGuid cannot be null when the operation type is DatabaseOperationType.Create, must be True or False.", "isPrimaryKeyGuid is null.");

            MsSqlDatabase msSqlDatabase = GetMsSqlDatabase(connectionString, sql, sqlParamList, commandType);

            using (msSqlDatabase.SqlConnection)
            {
                // open SqlConnection to the database
                await msSqlDatabase.SqlConnection.OpenAsync();

                // execute transact-sql command against the database
                using (msSqlDatabase.SqlCommand)
                {
                    if (operationType == DatabaseOperationType.Update || operationType == DatabaseOperationType.Delete)
                    {
                        // update or delete record
                        await msSqlDatabase.SqlCommand.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // create new record
                        if (isPrimaryKeyGuid!.Value)
                            return msSqlDatabase.SqlCommand.ExecuteScalar()?.ToString() ?? string.Empty;

                        var result = await msSqlDatabase.SqlCommand.ExecuteScalarAsync();
                        return result ?? DBNull.Value;
                    }
                }
            }

            return null;
        }

        internal static void AddSelectSkipAndTakeParams(List<SqlParameter> sqlParamList, string sortByExpression, int startRowIndex, int rows)
        {
            AddSqlParameter(sqlParamList, "@start", startRowIndex);
            AddSqlParameter(sqlParamList, "@numberOfRows", rows);
            AddSqlParameter(sqlParamList, "@sortByExpression", sortByExpression);
        }

        internal static void AddSqlParameter(List<SqlParameter> sqlParamList, string parameterName, object parameterValue)
        {
            if (!String.IsNullOrEmpty(parameterName))
            {
                SqlParameter sqlParam = new(parameterName, parameterValue);
                sqlParamList.Add(sqlParam);
            }
        }
    }
}
