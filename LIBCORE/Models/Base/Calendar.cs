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

        [RegularExpression(@"^(((((0[13578])|([13578])|(1[02]))[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9])|(3[01])))|((([469])|(11))[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9])|(30)))|((02|2)[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9]))))[\-\/\s]?\d{4})(\s(((0[1-9])|([1-9])|(1[0-2]))\:([0-5][0-9])((\s)|(\:([0-5][0-9])\s))([AM|PM|am|pm]{2,2})))?$", ErrorMessage = "{0} must be a valid date!")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [StringLength(1, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Flag")]
        public string? Flag { get; set; }
    }
}
