using System.ComponentModel.DataAnnotations.Schema;

namespace Meshwark.Models
{
    public class Trip
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } // Combined date field
        public string Time { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<Guid> RiderIds { get; set; } = new List<Guid>(); // List to hold multiple rider IDs
    }
}
