using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial interface INewsRepository
    {
        internal Task<DataTable> SelectAllAsync();


        internal Task<DataTable> SelectByPrimaryKeyAsync(int newsId);

        internal Task<DataTable> SelectAllDynamicWhereAsync(int? newsId, string title, string lead, string contentNew, string image, string field1, string field2, string field3, string field4, string field5, DateTime? createdAt, string flag);

        internal Task DeleteAsync(int newsId);

        internal Task<int> InsertAsync(News news);

        internal Task UpdateAsync(News news);
    }
}
