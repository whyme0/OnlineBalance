using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class ResetPasswordDTO
    {
        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Password field is required")]
        [Display(Name = "New password")]
        public string Password { get; set; }
        [Display(Name = "Reapeat password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords didn't match")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Too short or too long password")]
        [Required(ErrorMessage = "Repeat your password")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
