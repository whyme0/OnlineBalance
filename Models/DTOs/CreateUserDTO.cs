using Microsoft.AspNetCore.Components.Forms;
using OnlineBalance.Validation;
using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class CreateUserDTO
    {
        [Display(Name = "First name")]
        [RegularExpression(@"^[a-zA-Z]{2,}$", ErrorMessage = "Incorrect first name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [RegularExpression(@"^[a-zA-Z]{2,}$", ErrorMessage = "Incorrect last name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Birth date is required")]
        [AgeRestriction(18, ErrorMessage = "You must be 18 or older")]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Invalid format of email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Password field is required")]
        public string Password { get; set; }

        [Display(Name = "Reapeat password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords didn't match")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Repeat your password")]
        public string RepeatPassword { get; set; }
    }
}
