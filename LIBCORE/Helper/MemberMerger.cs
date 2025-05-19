using LIBCORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Helper
{
    public static class MemberMerger
    {
        public static void Merge(Member source, Member target)
        {
            target.FirstName = string.IsNullOrWhiteSpace(source.FirstName) ? target.FirstName : source.FirstName;
            target.MiddleName = string.IsNullOrWhiteSpace(source.MiddleName) ? target.MiddleName : source.MiddleName;
            target.LastName = string.IsNullOrWhiteSpace(source.LastName) ? target.LastName : source.LastName;
            target.Email = string.IsNullOrWhiteSpace(source.Email) ? target.Email : source.Email;
            target.Phone = string.IsNullOrWhiteSpace(source.Phone) ? target.Phone : source.Phone;
            target.Address = string.IsNullOrWhiteSpace(source.Address) ? target.Address : source.Address;
            target.Avatar = string.IsNullOrWhiteSpace(source.Avatar) ? target.Avatar : source.Avatar;
            target.Facebook = string.IsNullOrWhiteSpace(source.Facebook) ? target.Facebook : source.Facebook;
            target.Type = string.IsNullOrWhiteSpace(source.Type) ? target.Type : source.Type;
            target.Role = string.IsNullOrWhiteSpace(source.Role) ? target.Role : source.Role;
            target.Username = string.IsNullOrWhiteSpace(source.Username) ? target.Username : source.Username;
            target.Flag = string.IsNullOrWhiteSpace(source.Flag) ? target.Flag : source.Flag;

            // Password: chỉ ghi đè nếu có và đã được hash trước đó
            target.Password = string.IsNullOrWhiteSpace(source.Password) ? target.Password : source.Password;

            // Các trường kỹ thuật (nullable)
            target.Field1 = source.Field1 ?? target.Field1;
            target.Field2 = source.Field2 ?? target.Field2;
            target.Field3 = source.Field3 ?? target.Field3;
            target.Field4 = source.Field4 ?? target.Field4;
            target.Field5 = source.Field5 ?? target.Field5;

            target.RefreshToken = source.RefreshToken ?? target.RefreshToken;
            target.RefreshTokenExpiryTime = source.RefreshTokenExpiryTime ?? target.RefreshTokenExpiryTime;
            target.CreatedAt = source.CreatedAt ?? target.CreatedAt;
        }
    }
}
