namespace Meshwark.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; } // Rider's ID
        public Guid ReceiverId { get; set; } // Driver's ID
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

}
