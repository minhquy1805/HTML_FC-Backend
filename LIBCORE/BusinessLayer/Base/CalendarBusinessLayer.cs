using LIBCORE.DataRepository;
using LIBCORE.Helper;
using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar = LIBCORE.Models.Calendar;


namespace LIBCORE.BusinessLayer
{
    public partial class CalendarBusinessLayer : ICalendarBusinessLayer
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly EmailService _emailService;

        public CalendarBusinessLayer(ICalendarRepository calendarRepository, IMemberRepository memberRepository, EmailService emailService)
        {
            _calendarRepository = calendarRepository;
            _memberRepository = memberRepository;
            _emailService = emailService;
        }

        public async Task<Calendar> SelectByPrimaryKeyAsync(int calendarId) 
        {
            DataTable dt = await _calendarRepository.SelectByPrimaryKeyAsync(calendarId);

            if (dt is not null && dt.Rows.Count > 0)
                return this.CreateCalendarFromDataRow(dt.Rows[0]);

            return null!;
        }

        public async Task<List<Calendar>> SelectAllAsync()
        {
            DataTable dt = await _calendarRepository.SelectAllAsync();
            return this.GetListOfCalendar(dt);
        }

        public async Task<List<Calendar>> SelectAllDynamicWhereAsync(int? calendarId, string title, string eventCalendar, DateTime? calendarTime, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            DataTable dt = await _calendarRepository.SelectAllDynamicWhereAsyn(calendarId, title, eventCalendar, calendarTime, field1, field2, field3, field4, field5, createdAt, flag);
            return this.GetListOfCalendar(dt);

        }

        public async Task<int> InsertAsync(Calendar calendar)
        {
            // 1. Insert lịch mới
            int id = await _calendarRepository.InsertAsync(calendar);

            // 2. Gửi email cho các thành viên đã xác thực (Flag = 'T')
            DataTable dt = await _memberRepository.SelectAllAsync();
            foreach (DataRow row in dt.Rows)
            {
                string flag = row["Flag"]?.ToString() ?? "";
                string email = row["Email"]?.ToString() ?? "";

                if (flag == "T" && !string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendCalendarNotificationEmailAsync(email, calendar);
                }
            }

            return id;
        }


        public async Task UpdateAsync(Calendar calendar)
        {
            await _calendarRepository.UpdateAsync(calendar);
        }

        public async Task DeleteAsync(int memberId)
        {
            await _calendarRepository.DeleteAsync(memberId);
        }

        private List<Calendar> GetListOfCalendar(DataTable dt)
        {
            List<Calendar> objCalendar = null!;

            if(dt != null && dt.Rows.Count > 0)
            {
                objCalendar = new List<Calendar>();

                foreach (DataRow dr in dt.Rows) 
                {
                    Calendar calendar = this.CreateCalendarFromDataRow(dr);
                    objCalendar.Add(calendar);
                }
            }

            return objCalendar;
        }

       
        private Calendar CreateCalendarFromDataRow(DataRow dr)
        {
            Calendar calendar = new Calendar();

            calendar.CalendarId = (int)dr["CalendarId"];

            if(dr["Title"] != DBNull.Value)
                calendar.Title = dr["Title"].ToString();
            else
                calendar.Title = null;

            if(dr["Event"] != DBNull.Value)
                calendar.Event = dr["Event"].ToString();
            else
                calendar.Event = null;

            if (dr["CalendarTime"] != System.DBNull.Value)
                calendar.CalendarTime = (DateTime)dr["CalendarTime"];
            else
                calendar.CalendarTime = null;

            if (dr["Field1"] != System.DBNull.Value)
                calendar.Field1 = dr["Field1"].ToString();
            else
                calendar.Field1 = null;

            if (dr["Field2"] != System.DBNull.Value)
                calendar.Field2 = dr["Field2"].ToString();
            else
                calendar.Field2 = null;

            if (dr["Field3"] != System.DBNull.Value)
                calendar.Field3 = dr["Field3"].ToString();
            else
                calendar.Field3 = null;

            if (dr["Field4"] != System.DBNull.Value)
                calendar.Field4 = dr["Field4"].ToString();
            else
                calendar.Field4 = null;

            if (dr["Field5"] != System.DBNull.Value)
                calendar.Field5 = dr["Field5"].ToString();
            else
                calendar.Field5 = null;

            if (dr["CreatedAt"] != System.DBNull.Value)
                calendar.CreatedAt = (DateTime)dr["CreatedAt"];
            else
                calendar.CreatedAt = null;

            if (dr["Flag"] != System.DBNull.Value)
                calendar.Flag = dr["Flag"].ToString();
            else
                calendar.Flag = null;

            return calendar;
        }
    }
}
