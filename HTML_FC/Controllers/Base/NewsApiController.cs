using LIBCORE.BusinessLayer;
using LIBCORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/")]
    [ApiController]
    public partial class NewsApiController : ControllerBase
    {
        private News _news;
        private readonly INewsBusinessLayer _newsBusinessLayer;

        public NewsApiController(News news,INewsBusinessLayer newsBusinessLayer)
        {
            _news = news;
            _newsBusinessLayer = newsBusinessLayer;
        }

        [Route("[controller]/selectbyprimarykey")]
        [HttpGet]
        public async Task<News> SelectByPrimaryKey(int id)
        {
            News news = await _newsBusinessLayer.SelectByPrimaryKeyAsync(id);
            return news;
        }

        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<List<News>> SelectAll()
        {
            List<News> newss;

            newss = await _newsBusinessLayer.SelectAllAsync();

            return newss;
        }

        [Route("[controller]/selectalldynamicwhere")]
        [HttpGet]
        public async Task<List<News>> SelectAllDynamicWhereAsync(int? newsId, string title, string content, string image, string type, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            List<News> newss = await _newsBusinessLayer.SelectAllDynamicWhereAsync(newsId, title, content, image, type, field1, field2, field3, field4, field5, createdAt, flag);
            return newss;
        }
    }
}
