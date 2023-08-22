using System.ComponentModel.DataAnnotations;
using System.Configuration;
using OnlineBalance.Validation;

namespace OnlineBalance.Models
{
    public class TransferMoneyDTO
    {
        // amount must be >= than sender account balance | 
        [Display(Name = "Recipient account's number")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[1-9][0-9]{15}|[1-9]{4}\s*[0-9]{4}\s*[0-9]{4}\s*[0-9]{4}$", ErrorMessage = "Incorrect format of recipient account's number")]
        [Required]
        public long RecipientAccountNumber { get; set; }

        [PositiveNumber(ErrorMessage = "Invalid amount provided")]
        [Required]
        public double Amount { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(250, ErrorMessage = "Too long message")]
        public string? Description { get; set; }
    }
}
