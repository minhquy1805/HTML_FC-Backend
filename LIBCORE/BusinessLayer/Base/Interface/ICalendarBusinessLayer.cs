using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.BusinessLayer
{
    public partial interface ICalendarBusinessLayer
    {
        public Task<Calendar> SelectByPrimaryKeyAsync(int calendarId);

        public Task<List<Calendar>> SelectAllAsync();

        public Task<List<Calendar>> SelectAllDynamicWhereAsync(int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag);

        public Task DeleteAsync(int calendarId);

        public Task<int> InsertAsync(Calendar calendar);

        public Task UpdateAsync(Calendar calendar);
    }
}
