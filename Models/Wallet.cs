namespace Meshwark.Models
{
    public class Wallet
    {
        public Guid
            UserId { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
