using System.ComponentModel.DataAnnotations;

namespace OnlineBalance.Models
{
    public class CreateAccountDTO
    {
        [Required]
        public CurrencyType Currency { get; set; }
    }
}
