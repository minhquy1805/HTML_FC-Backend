using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial interface IInExpenRepository
    {
        internal Task<DataTable> SelectByPrimaryKeyAsync(int inExpenId);

        internal Task<DataTable> SelectAllAsync();

        internal Task<DataTable> SelectAllDynamicWhereAsync(int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag);

        internal Task<DataTable> SelectAllByMemberId(int memberId);

        internal Task DeleteAsync(int inExpenId);

        internal Task<int> InsertAsync(InExpen objInExpen);

        internal Task UpdateAsync(InExpen objInExpen);
    }
}
