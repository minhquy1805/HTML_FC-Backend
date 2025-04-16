using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Models
{
    public partial class CertificateType
    {
        [Display(Name = "CertificateType Id")]
        public int CertificateTypeId { get; set; }


        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Certificate Title")]
        public string? CertificateTitle { get; set; }
    }
}
