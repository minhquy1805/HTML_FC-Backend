using System.ComponentModel.DataAnnotations;

namespace LIBCORE.Models
{
    public partial class Member
    {
        [Display(Name = "Member Id")]
        public int MemberId { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Phone number must be between 7 and 15 digits and can include an optional '+' at the start.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Facebook")]
        public string? Facebook { get; set; }

        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [StringLength(1500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }

        [Display(Name = "Number Player")]
        public int? NumberPlayer { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Role")]
        public string? Role { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [StringLength(500, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Refresh Token")]
        public string? RefreshToken { get; set; }

        [Display(Name = "Refresh Token Expiry Time")]
        public DateTime? RefreshTokenExpiryTime { get; set; }

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

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [StringLength(1, ErrorMessage = "{0} must be a maximum of {1} characters long!")]
        [Display(Name = "Flag")]
        public string? Flag { get; set; }
    }
}
