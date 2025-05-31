using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.Models
{
    public partial class Calendar
    {
        [Display(Name = "Member Id")]
        public int CalendarId { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Event")]
        public string? Event { get; set; }

       
        [Display(Name = "Calendar Time")]
        public DateTime? CalendarTime { get; set; }

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
