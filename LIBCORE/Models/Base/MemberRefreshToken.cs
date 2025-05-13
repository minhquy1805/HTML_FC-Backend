using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Models
{
    public partial class MemberRefreshToken
    {
        [Display(Name = "MemberRefreshTokens Id")]
        public int MemberRefreshTokensId { get; set; }


        [Display(Name = "Member Id")]
        public int MemberId { get; set; }

        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "RefreshToken")]
        public string? RefreshToken { get; set; }

        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RefreshTokenExpiry { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Device Information")]
        public string? DeviceInfo { get; set; }

        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Field1")]
        public string? Field1 { get; set; }


        [Display(Name = "Field2")]
        public string? Field2 { get; set; }


        [Display(Name = "Field3")]
        public string? Field3 { get; set; }

        [Display(Name = "Field4")]
        public string? Field4 { get; set; }

        [Display(Name = "Field5")]
        public string? Field5 { get; set; }


        [StringLength(1, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Flag")]
        public string? Flag { get; set; }

        public Member? member { get; set; }
    }
}
