namespace Meshwark.DTOs
{
    public class TransactionDto
    {
        public Guid SenderId { get; set; } // Rider's ID
        public Guid ReceiverId { get; set; } // Driver's ID
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

}
