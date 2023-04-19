using Microsoft.AspNetCore.Identity;

namespace OnlineBalance.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsGotMoneyGift { get; set; } = false;

        public List<Account> Accounts { get; set; } = new List<Account>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
