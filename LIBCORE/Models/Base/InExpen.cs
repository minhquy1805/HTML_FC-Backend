using System.ComponentModel.DataAnnotations;

namespace LIBCORE.Models
{
    public partial class InExpen
    {
        [Display(Name = "InExpen Id")]
        public int InExpenId { get; set; }

        [Display(Name = "Member Id")]
        public int? MemberId { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Transaction Time")]
        public DateTime? TransactionTime { get; set; }

        [Display(Name = "Money Value")]
        [Range(0, double.MaxValue, ErrorMessage = "Money Value cannot be negative.")]
        public float? MoneyValue { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "File Attach")]
        public string? FileAttach { get; set; }

        [Display(Name = "Field1")]
        public string? Field1 { get; set; }


        [Display(Name = "Field2")]
        public string? Field2 { get; set; }


        [Display(Name = "Field3")]
        public string? Field3 { get; set; }

        [Display(Name = "Field3")]
        public string? Field4 { get; set; }

        [Display(Name = "Field3")]
        public string? Field5 { get; set; }


        [StringLength(1, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Is Deleted")]
        public string? Flag { get; set; }

        public Member? Member { get; set; }
    }
}
