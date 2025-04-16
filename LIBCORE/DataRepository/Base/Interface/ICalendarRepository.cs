using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using LIBCORE.Models;

namespace LIBCORE.DataRepository
{
    public partial interface ICalendarRepository
    {
        internal Task<DataTable> SelectByPrimaryKeyAsync(int calendarId);

        internal Task<DataTable> SelectAllAsync();

        internal Task DeleteAsync(int calendarId);

        internal Task<int> InsertAsync(Calendar calendar);

        internal Task<DataTable> SelectAllDynamicWhereAsyn(int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag);

        internal Task UpdateAsync(Calendar calendar);
    }
}
