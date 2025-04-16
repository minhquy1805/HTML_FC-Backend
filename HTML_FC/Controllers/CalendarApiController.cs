using LIBCORE.BusinessLayer;
using LIBCORE.Domain;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public partial class CalendarApiController : ControllerBase
    {
        private Calendar _Calendar;
        private readonly ICalendarBusinessLayer _CalendarBusinessLayer;

        public CalendarApiController(Calendar Calendar, ICalendarBusinessLayer CalendarBusinessLayer)
        {
            _Calendar = Calendar;
            _CalendarBusinessLayer = CalendarBusinessLayer;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Calendar model, bool isForListInlineOrListCrud = false)
        {
            return await AddEditAsync(model, CrudOperation.Add, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Calendar model, bool isForListInlineOrListCrud = false)
        {
            // update existing record
            return await this.AddEditAsync(model, CrudOperation.Update, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete a record based on id(s)
                await _CalendarBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }


        private async Task<IActionResult> AddEditAsync(Calendar model, CrudOperation operation, bool isForListInlineOrListCrud = false)
        {
            try
            {
                if (operation == CrudOperation.Add)
                    _Calendar = new();
                else
                    _Calendar = await _CalendarBusinessLayer.SelectByPrimaryKeyAsync(model.CalendarId);

                _Calendar.Title = model.Title;
                _Calendar.Event = model.Event;
                _Calendar.CalendarTime = model.CalendarTime;

                _Calendar.Field1 = model.Field1;
                _Calendar.Field2 = model.Field2;
                _Calendar.Field3 = model.Field3;
                _Calendar.Field4 = model.Field4;
                _Calendar.Field5 = model.Field5;
                _Calendar.CreatedAt = model.CreatedAt;
                _Calendar.Flag = model.Flag;

                if(operation == CrudOperation.Add)
                    await _CalendarBusinessLayer.InsertAsync(_Calendar);
                else
                    await _CalendarBusinessLayer.UpdateAsync(_Calendar);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
