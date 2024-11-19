public class TripDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; } // Combined date field
    public string Time { get; set; }
    public string StartPoint { get; set; }
    public string EndPoint { get; set; }
    public decimal Price { get; set; } // Total price of the trip
    public Guid UserId { get; set; }
    public List<Guid> RiderIds { get; set; } // Multiple riders
}
