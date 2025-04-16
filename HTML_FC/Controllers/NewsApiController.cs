using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using LIBCORE.Domain;
using Microsoft.AspNetCore.Authorization;

namespace HTML_FC.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public partial class NewsApiController : ControllerBase
    {
        private News _News;
        private readonly INewsBusinessLayer _NewsBusinessLayer;

        public NewsApiController(News News, INewsBusinessLayer NewsBusinessLayer)
        {
            _News = News;
            _NewsBusinessLayer = NewsBusinessLayer;
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] News model, bool isForListInlineOrListCrud = false)
        {
            return await AddEditAsync(model, CrudOperation.Add, isForListInlineOrListCrud);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] News model, bool isForListInlineOrListCrud = false)
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
                await _NewsBusinessLayer.DeleteAsync(id);

                // everthing went well
                return Ok();
            }
            catch (Exception ex)
            {

                // something went wrong
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        private async Task<IActionResult> AddEditAsync(News model, CrudOperation operation, bool isForListInlineOrListCrud = false)
        {
            try
            {
                if (operation == CrudOperation.Add)
                {
                    // insert new record
                    _News = new();
                }
                else
                    _News = await _NewsBusinessLayer.SelectByPrimaryKeyAsync(model.NewsId);

                _News.Title = model.Title;
                _News.Lead = model.Lead;
                _News.ContentNew = model.ContentNew;
                _News.Image = model.Image;

                _News.Field1 = model.Field1;
                _News.Field2 = model.Field2;
                _News.Field3 = model.Field3;
                _News.Field4 = model.Field4;
                _News.Field5 = model.Field5;
                _News.CreatedAt = model.CreatedAt;
                _News.Flag = model.Flag;

                if (operation == CrudOperation.Add)
                {
                    await _NewsBusinessLayer.InsertAsync(_News);
                }
                else
                {
                    await _NewsBusinessLayer.UpdateAsync(_News);
                }

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}
