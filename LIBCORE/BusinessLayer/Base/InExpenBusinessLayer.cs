using LIBCORE.DataRepository;
using LIBCORE.Models;
using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.BusinessLayer
{
    public partial class InExpenBusinessLayer : IInExpenBusinessLayer
    {
        private readonly IInExpenRepository _inExpenRepository;
        private readonly IMemberBusinessLayer _memberBusinessLayer;

        public InExpenBusinessLayer(IInExpenRepository inExpenRepository, IMemberBusinessLayer memberBusinessLayer)
        {
            _inExpenRepository = inExpenRepository;
            _memberBusinessLayer = memberBusinessLayer;

        }

        public async Task<InExpen> SelectByPrimaryKeyAsync(int inExpenId)
        {
            DataTable dt = await _inExpenRepository.SelectByPrimaryKeyAsync(inExpenId);
            if (dt is not null && dt.Rows.Count > 0)
                return await this.CreateInExpenFromDataRow(dt.Rows[0]); ;

            return null!;
        }

        public async Task<List<InExpen>> SelectAllAsync()
        {
            DataTable dt = await _inExpenRepository.SelectAllAsync();
            return await this.GetListOfInExpen(dt);
        }

        public async Task<List<InExpen>> SelectAllDynamicWhereAsync(int? inExpenId, int? memberId, DateTime? transactionTime, float? moneyValue, string type, string description, DateTime? createdAt, string fileAttach, string field1, string field2, string field3, string field4, string field5, string flag)
        {
            DataTable dt = await _inExpenRepository.SelectAllDynamicWhereAsync(inExpenId, memberId, transactionTime, moneyValue, type, description, createdAt, fileAttach, field1, field2, field3, field4, field5, flag);
            return await this.GetListOfInExpen(dt);
        }

        public async Task<List<InExpen>> SelectAllByMemberId(int memberId)
        {
            DataTable dt = await _inExpenRepository.SelectAllByMemberId(memberId);
            return await this.GetListOfInExpen(dt);
        }

        public async Task<int> InsertAsync(InExpen inExpen)
        {
            return await _inExpenRepository.InsertAsync(inExpen);
        }

        public async Task UpdateAsync(InExpen inExpen)
        {
            await _inExpenRepository.UpdateAsync(inExpen);
        }

        public async Task DeleteAsync(int inExpenId)
        {
            await _inExpenRepository.DeleteAsync(inExpenId);
        }

        private async Task<List<InExpen>> GetListOfInExpen(DataTable dt)
        {
            List<InExpen> inExpens = null!;

            if(dt != null && dt.Rows.Count > 0)
            {
                inExpens = new List<InExpen>();

                foreach(DataRow dr in dt.Rows)
                {
                    InExpen inExpen = await this.CreateInExpenFromDataRow(dr);
                    inExpens.Add(inExpen);
                }
            }
            return inExpens;
        }

        private async Task<InExpen> CreateInExpenFromDataRow(DataRow dr)
        {
            InExpen inExpen = new();
            inExpen.InExpenId = Convert.ToInt32(dr["InExpenId"]);

            if (dr["MemberId"] != DBNull.Value)
            {
                int memberId = (int)dr["MemberId"];
                inExpen.MemberId = memberId;
                inExpen.Member = await _memberBusinessLayer.SelectByPrimaryKeyAsync(memberId);

            }
            else
            {
                inExpen.MemberId = null;
                inExpen.Member = null;
            }

            if(dr["TransactionTime"] != DBNull.Value)
                inExpen.TransactionTime = Convert.ToDateTime(dr["TransactionTime"]);
            else
                inExpen.TransactionTime = null;

            if (dr["MoneyValue"] != DBNull.Value)
                inExpen.MoneyValue = Convert.ToSingle(dr["MoneyValue"]);
            else
                inExpen.MoneyValue = null;

            if (dr["Type"] != DBNull.Value)
                inExpen.Type = dr["Type"].ToString();
            else
                inExpen.Type = null;

            if (dr["Description"] != DBNull.Value)
                inExpen.Description = dr["Description"].ToString();
            else
                inExpen.Description = null;

            if (dr["CreatedAt"] != DBNull.Value)
                inExpen.CreatedAt = Convert.ToDateTime(dr["CreatedAt"]);
            else
                inExpen.CreatedAt = null;

            if (dr["FileAttach"] != DBNull.Value)
                inExpen.FileAttach = dr["FileAttach"].ToString();
            else
                inExpen.FileAttach = null;

            if (dr["Field1"] != DBNull.Value)
                inExpen.Field1 = dr["Field1"].ToString();
            else
                inExpen.Field1 = null;

            if (dr["Field2"] != DBNull.Value)
                inExpen.Field2 = dr["Field2"].ToString();
            else
                inExpen.Field2 = null;

            if (dr["Field3"] != DBNull.Value)
                inExpen.Field3 = dr["Field3"].ToString();
            else
                inExpen.Field3 = null;

            if (dr["Field4"] != DBNull.Value)
                inExpen.Field4 = dr["Field4"].ToString();
            else
                inExpen.Field4 = null;

            if (dr["Field5"] != DBNull.Value)
                inExpen.Field5 = dr["Field5"].ToString();
            else
                inExpen.Field5 = null;

            if (dr["Flag"] != DBNull.Value)
                inExpen.Flag = dr["Flag"].ToString();
            else
                inExpen.Flag = null;

            return inExpen;
        }
    }
}
