namespace OnlineBalance.Models
{
    public class Account
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public long Number { get; set; }
        public User User { get; set; }
    }

    public enum CurrencyType
    {
        RUB,
        USD,
        UAH,
        EUR
    }
}
