namespace Meshwark.Models
{
    public class DriverUpdateTrip
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool? IsOnline { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? AvailableSeats { get; set; }
        public int? ReservedSeats { get; set; }
        public string? NextDestination { get; set; }
        public string? NumberPlate{ get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public string? Gender { get; set; }

    }

}
