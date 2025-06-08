using LIBCORE.DataRepository;
using System.Data;
using LIBCORE.Models;
using LIBCORE.Helper;
namespace LIBCORE.BusinessLayer
{
    public partial class NewsBusinessLayer : INewsBusinessLayer
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly EmailService _emailService;

        public NewsBusinessLayer(INewsRepository newsRepository, IMemberRepository memberRepository, EmailService emailService)
        {
            _newsRepository = newsRepository;
            _memberRepository = memberRepository;
            _emailService = emailService;
        }

        public async Task<News> SelectByPrimaryKeyAsync(int newsId)
        {
            DataTable dt = await _newsRepository.SelectByPrimaryKeyAsync(newsId);

            // create News
            if (dt is not null && dt.Rows.Count > 0)
                return this.CreateNewsFromDataRow(dt.Rows[0]);

            return null!;
        }

        public async Task<List<News>> SelectAllAsync()
        {
            DataTable dt = await _newsRepository.SelectAllAsync();
            return this.GetListOfNews(dt);
        }


        public async Task<List<News>> SelectAllDynamicWhereAsync(int? newsId, string title, string lead, string contentNew, string image, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag)
        {
            DataTable dt = await _newsRepository.SelectAllDynamicWhereAsync(newsId, title, lead, contentNew, image, field1, field2, field3, field4, field5, createdAt, flag);
            return this.GetListOfNews(dt);
        }

        public async Task<int> InsertAsync(News news)
        {
            // 1. Insert tin tức mới
            int id = await _newsRepository.InsertAsync(news);

            // 2. Gửi email thông báo cho các member đã xác thực (Flag = 'T')
            DataTable dt = await _memberRepository.SelectAllAsync();

            foreach (DataRow row in dt.Rows)
            {
                string flag = row["Flag"]?.ToString() ?? "";
                string email = row["Email"]?.ToString() ?? "";

                if (flag == "T" && !string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendNewsNotificationEmailAsync(email, news);
                }
            }

            return id;
        }

        public async Task UpdateAsync(News news)
        {
            await _newsRepository.UpdateAsync(news);
        }

        public async Task DeleteAsync(int newsId)
        {
            await _newsRepository.DeleteAsync(newsId);
        }

        private List<News> GetListOfNews(DataTable dt)
        {
            List<News> objNewsList = null!;

            // build the list of News
            if (dt != null && dt.Rows.Count > 0)
            {
                objNewsList = new List<News>();
                foreach (DataRow dr in dt.Rows)
                {
                    News news = this.CreateNewsFromDataRow(dr);
                    objNewsList.Add(news);
                }
            }

            return objNewsList;
        }

        private News CreateNewsFromDataRow(DataRow dr)
        {
            News news = new News();
            
            news.NewsId = (int)dr["NewsId"];

            if (dr["Title"] != DBNull.Value)
                news.Title = (string)dr["Title"];

            if (dr["Lead"] != DBNull.Value)
                news.Lead = (string)dr["Lead"];

            if (dr["ContentNew"] != DBNull.Value)
                news.ContentNew = (string)dr["ContentNew"];

            if (dr["Image"] != DBNull.Value)
                news.Image = (string)dr["Image"];

            if (dr["Field1"] != DBNull.Value)
                news.Field1 = (string)dr["Field1"];

            if (dr["Field2"] != System.DBNull.Value)
                news.Field2 = dr["Field2"].ToString();
            else
                news.Field2 = null;

            if (dr["Field3"] != System.DBNull.Value)
                news.Field3 = dr["Field3"].ToString();
            else
                news.Field3 = null;

            if (dr["Field4"] != System.DBNull.Value)
                news.Field4 = dr["Field4"].ToString();
            else
                news.Field4 = null;

            if (dr["Field5"] != System.DBNull.Value)
                news.Field5 = dr["Field5"].ToString();
            else
                news.Field5 = null;

            if (dr["CreatedAt"] != System.DBNull.Value)
                news.CreatedAt = (DateTime)dr["CreatedAt"];
            else
                news.CreatedAt = null;

            if (dr["Flag"] != System.DBNull.Value)
                news.Flag = dr["Flag"].ToString();
            else
                news.Flag = null;

            return news;
        }
    }
}
