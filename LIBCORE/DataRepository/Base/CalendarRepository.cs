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
    public partial class CalendarRepository : ICalendarRepository
    {
        private const CommandType _commandType = CommandType.StoredProcedure;
        private readonly string _connectionString;

        internal CalendarRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<DataTable> ICalendarRepository.SelectByPrimaryKeyAsync(int calendarId)
        {
            string storedProcedure = "[dbo].[Calendar_SelectByPrimaryKey]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarId", calendarId);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcedure, sqlParamList, _commandType);
        }

        async Task<DataTable> ICalendarRepository.SelectAllAsync()
        {
            string storedProcedure = "[dbo].[Calendar_SelectAll]";
            return await SelectSharedAsync(storedProcedure, String.Empty, null!, null!, null, null);
        }

        async Task<DataTable> ICalendarRepository.SelectAllDynamicWhereAsyn(int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            string storedProcName = "[dbo].[Calendar_SelectAllDynamicWhere]";
            List<SqlParameter> sqlParamList = new();

            this.AddSearchCommandParamsShared(sqlParamList, calendarId, title, eventCalendar, calendarTime, field1, field2, field3, field4, field5, createdAt, flag);

            return await DatabaseFunctions.GetDataTableAsync(_connectionString, storedProcName, sqlParamList, _commandType);
        }

        async Task ICalendarRepository.DeleteAsync(int calendarId)
        {
            string storedProcedure = "[dbo].[Calendar_Delete]";
            List<SqlParameter> sqlParamList = new();

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarId", calendarId);

            await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Delete);
        }

        async Task<int> ICalendarRepository.InsertAsync(Calendar calendar)
        {
            return await InsertUpdateAsync(calendar, DatabaseOperationType.Create);
        }

        async Task ICalendarRepository.UpdateAsync(Calendar calendar)
        {
            await InsertUpdateAsync(calendar, DatabaseOperationType.Update);
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

        private async Task<int> InsertUpdateAsync(Calendar calendar, DatabaseOperationType operationType)
        {
            if (operationType == DatabaseOperationType.RetrieveDataTable || operationType == DatabaseOperationType.Delete)
                throw new ArgumentException("Invalid DatabaseOperationType!  Acceptable operation types are: Create or Update only.", "operationType: " + operationType.ToString());

            List<SqlParameter> sqlParamList = new();

            int newlyCreatedCalendarId = calendar.CalendarId;

            string storedProcedure;

            if (operationType == DatabaseOperationType.Update)
                storedProcedure = "[dbo].[Calendar_Update]";
            else
                storedProcedure = "[dbo].[Calendar_Insert]";

            object? title = calendar.Title;
            object? eventCalendar = calendar.Event;
            object? calendarTime = calendar.CalendarTime;
            object? field1 = calendar.Field1;
            object? field2 = calendar.Field2;
            object? field3 = calendar.Field3;
            object? field4 = calendar.Field4;
            object? field5 = calendar.Field5;
            object? createdAt = calendar.CreatedAt;
            object? flag = calendar.Flag;

            if (String.IsNullOrEmpty(calendar.Title))
                title = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Event))
                eventCalendar = DBNull.Value;

            if (calendar.CalendarTime is null)
                calendarTime = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Field1))
                field1 = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Field2))
                field2 = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Field3))
                field3 = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Field4))
                field4 = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Field5))
                field5 = DBNull.Value;

            if (calendar.CreatedAt is null)
                createdAt = DBNull.Value;

            if (String.IsNullOrEmpty(calendar.Flag))
                flag = DBNull.Value;

            if (operationType == DatabaseOperationType.Update)
            {
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarId", calendar.CalendarId);
            }

            DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@event", eventCalendar!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarTime", calendarTime!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt!);
            DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag!);

            if (operationType == DatabaseOperationType.Update)
            {
                await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Update);
            }
            else
            {
                var result = await DatabaseFunctions.ExecuteSqlCommandAsync(_connectionString, storedProcedure, sqlParamList, _commandType, DatabaseOperationType.Create, false);
                newlyCreatedCalendarId = (int)(result ?? 0); // Default to 0 if null returned
            }

            return newlyCreatedCalendarId;
        }

        private void AddSearchCommandParamsShared(List<SqlParameter> sqlParamList, int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            if(calendarId is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarId", calendarId.Value);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarId", System.DBNull.Value);

            if (!String.IsNullOrEmpty(title))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", title);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@title", System.DBNull.Value);

            if (!String.IsNullOrEmpty(eventCalendar))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@event", eventCalendar);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@event", System.DBNull.Value);

            if (calendarTime is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarTime", calendarTime.Value);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@calendarTime", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field1))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", field1);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field1", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field2))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", field2);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field2", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field3))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", field3);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field3", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field4))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", field4);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field4", System.DBNull.Value);

            if (!String.IsNullOrEmpty(field5))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", field5);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@field5", System.DBNull.Value);

            if (createdAt is not null)
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", createdAt.Value);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@createdAt", System.DBNull.Value);

            if(!String.IsNullOrEmpty(flag))
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", flag);
            else
                DatabaseFunctions.AddSqlParameter(sqlParamList, "@flag", System.DBNull.Value);
        }
    }
}

