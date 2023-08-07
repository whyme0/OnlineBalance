using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
