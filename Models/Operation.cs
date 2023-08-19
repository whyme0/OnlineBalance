namespace OnlineBalance.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public long SenderNumber { get; set; }
        public long RecipientNumber { get; set; }
        public DateTime Date { get; set; }
    }

    public class TemporaryOperation
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public long SenderNumber { get; set; }
        public long RecipientNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}
