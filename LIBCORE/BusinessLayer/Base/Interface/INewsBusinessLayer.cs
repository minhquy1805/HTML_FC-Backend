using LIBCORE.Models;

namespace LIBCORE.BusinessLayer
{
    public partial interface INewsBusinessLayer
    {
        public Task<News> SelectByPrimaryKeyAsync(int newsId);

        public Task<List<News>> SelectAllAsync();

        public Task<List<News>> SelectAllDynamicWhereAsync(int? newsId, string title, string lead, string contentNew, string image, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag);

        public Task<int> InsertAsync(News news);

        public Task UpdateAsync(News news);

        public Task DeleteAsync(int newsId);
    }
}
