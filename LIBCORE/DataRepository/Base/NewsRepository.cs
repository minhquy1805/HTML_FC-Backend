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
    public partial class NewsRepository : INewsRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal NewsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<DataTable> INewsRepository.SelectByPrimaryKeyAsync(int newsId)
        {
            string storedProcName = "[dbo].[News_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            // add parameters to the sqlParams
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@newsId", newsId);

            // get and return the data
            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        async Task<DataTable> INewsRepository.SelectAllAsync()
        {
            string storedProcName = "[dbo].[News_SelectAll]";
            return await SelectSharedAsync(storedProcName, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> INewsRepository.SelectAllDynamicWhereAsync(int? newsId, string title, string lead, string contentNew, string image, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            string storedProcName = "[dbo].[News_SelectAllDynamicWhere]";
            List<SqlParameter> sqlParamList = new();

            AddSearchCommandParamsShared(sqlParamList, newsId, title, lead, contentNew, image, field1, field2, field3, field4, field5, createdAt, flag);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        async Task INewsRepository.DeleteAsync(int newsId)
        {
            string storedProcName = "[dbo].[News_Delete]";
            List<SqlParameter> sqlParamList = new();

            // add parameters to the sqlParams
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@newsId", newsId);

            // get and return the data
            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcName, sqlParamList, _commandType, DatabaseOperationType.Delete, false);
        }

        async Task<int> INewsRepository.InsertAsync(News news)
        {
            return await InsertUpdateAsync(news, DatabaseOperationType.Create);
        }

        async Task INewsRepository.UpdateAsync(News news)
        {
            await InsertUpdateAsync(news, DatabaseOperationType.Update);
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


        private async Task<int> InsertUpdateAsync(News news, DatabaseOperationType operationType) 
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedNewsId = news.NewsId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[News_Update]";
            else
                storedProcedure = "[dbo].[News_Insert]";

            object? title = news.Title;
            object? lead = news.Lead;
            object? contentNew = news.ContentNew;
            object? image = news.Image;
            object? field1 = news.Field1;
            object? field2 = news.Field2;
            object? field3 = news.Field3;
            object? field4 = news.Field4;
            object? field5 = news.Field5;
            object? createdAt = news.CreatedAt;
            object? flag = news.Flag;

           if(String.IsNullOrEmpty(news.Title))
                title = DBNull.Value;

           if (String.IsNullOrEmpty(news.Lead))
                lead = DBNull.Value;

           if (String.IsNullOrEmpty(news.ContentNew))
                contentNew = DBNull.Value;

           if (String.IsNullOrEmpty(news.Image))
                image = DBNull.Value;

           if (String.IsNullOrEmpty(news.Field1))
                field1 = DBNull.Value;

           if (String.IsNullOrEmpty(news.Field2))
                field2 = DBNull.Value;

           if (String.IsNullOrEmpty(news.Field3))
                field3 = DBNull.Value;

           if (String.IsNullOrEmpty(news.Field4))
                field4 = DBNull.Value;

           if (String.IsNullOrEmpty(news.Field5))
                field5 = DBNull.Value;

            if (String.IsNullOrEmpty(news.Flag))
                flag = System.DBNull.Value;

            if (operationType == DatabaseOperationType.Update) 
            {
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@newsId", news.NewsId);
            }

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@lead", lead!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@contentNew", contentNew!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@image", image!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag!);

            if (operationType == DatabaseOperationType.Update)
            {
                await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
            }
            else
            {
                var result = await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
                newlyCreatedNewsId = (int)(result ?? 0); // Default to 0 if null returned
            }
            return newlyCreatedNewsId;
        }

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? newsId, string title, string lead, string contentNew, string image, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            if (newsId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@newsId", newsId);

            if (!String.IsNullOrEmpty(title))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title);

            if (!String.IsNullOrEmpty(lead))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@lead", lead);

            if (!String.IsNullOrEmpty(contentNew))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@contentNew", contentNew);

            if (!String.IsNullOrEmpty(image))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@image", image);

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

            if (createdAt is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt);

            if (!String.IsNullOrEmpty(flag))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag);
        }
    }
}
