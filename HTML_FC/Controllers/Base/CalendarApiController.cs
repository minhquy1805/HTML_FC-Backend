using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/")]
    [ApiController]
    public partial class CalendarApiController : ControllerBase
    {
        private Calendar _calendar;
        private readonly ICalendarBusinessLayer _CalendarBusinessLayer;

        public CalendarApiController(Calendar calendar, ICalendarBusinessLayer calendarBusinessLayer)
        {
            _calendar = calendar;
            _CalendarBusinessLayer = calendarBusinessLayer;
        }


        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<Calendar> SelectByPrimaryKey(int id)
        {
            Calendar calendar = await _CalendarBusinessLayer.SelectByPrimaryKeyAsync(id);
            return calendar;
        }

        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<Calendar>> SelectAll()
        {
            List<Calendar> calendars;

            calendars = await _CalendarBusinessLayer.SelectAllAsync();

            return calendars;
        }

        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]

        public async Task<List<Calendar>> SelectAllDynamicWhereAsync(int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            List<Calendar> calendars = await _CalendarBusinessLayer.SelectAllDynamicWhereAsync(calendarId, title, eventCalendar, calendarTime, field1, field2, field3, field4, field5, createdAt, flag);
            return calendars;
        }
    }
}
