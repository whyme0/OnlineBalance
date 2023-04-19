using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class TransferMoneyDTO
    {
        // amount must be >= than sender account balance | 
        [Display(Name = "Recipient account's number")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[1-9][0-9]{15}$", ErrorMessage = "Incorrect format of recipient account's number")]
        public long RecipientAccountNumber { get; set; }

        public double Amount { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(250, ErrorMessage = "Too long message")]
        public string? Description { get; set; }
    }
}
