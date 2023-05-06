using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class ChangePasswordDTO
    {
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Repeat your password")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords didn't match")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Repeat your password")]
        [Display(Name = "New password again")]
        public string NewPasswordAgain { get; set; }
    }
}
