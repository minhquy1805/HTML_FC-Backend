using System.ComponentModel.DataAnnotations;
namespace LIBCORE.Models
{
    public partial class Certificate
    {
        [Display(Name = "Certificate Id")]
        public int CertificateId { get; set; }

        [Display(Name = "Certificate Type Id")]
        public int? CertificateTypeId { get; set; }

        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Title")]
        public string? Title { get; set; }


        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Content Certificate")]
        public string? ContentCert { get; set; }


        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Certificate")]
        public DateTime? DateCert { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Sign Certificate")]
        public string? SignCert { get; set; }

        [Display(Name = "Reason Certificate")]
        public string? ReasonCert { get; set; }

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

        public CertificateType? CertificateType { get; set; }
    }
}
