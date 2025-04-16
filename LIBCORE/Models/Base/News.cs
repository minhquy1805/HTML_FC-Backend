using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Models
{
    public partial class News
    {
        [Display(Name = "News Id")]
        public int NewsId { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Lead")]
        public string? Lead { get; set; }

        [StringLength(3000, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Content News")]
        public string? ContentNew { get; set; }

        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Image")]
        public string? Image { get; set; }

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

       
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [StringLength(1, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Flag")]
        public string? Flag { get; set; }
    }
}
