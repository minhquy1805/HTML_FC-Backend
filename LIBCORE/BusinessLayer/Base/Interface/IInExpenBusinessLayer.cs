using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.BusinessLayer
{
    public partial interface IInExpenBusinessLayer
    {
        public Task<InExpen> SelectByPrimaryKeyAsync(int inExpenId);

        public Task<List<InExpen>> SelectAllAsync();

        public Task<List<InExpen>> SelectAllDynamicWhereAsync(int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag);

        public Task<List<InExpen>> SelectAllByMemberId(int memberId);

        public Task<int> InsertAsync(InExpen inExpen);

        public Task UpdateAsync(InExpen inExpen);

        public Task DeleteAsync(int inExpenId);
    }
}
