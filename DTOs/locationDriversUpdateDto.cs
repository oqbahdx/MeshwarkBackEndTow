namespace Meshwark.DTOs
{
    public class DriverDto
    {
        public string DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AvailableSeats { get; set; }
        public int ReservedSeats { get; set; }  
        public bool IsOnline { get; set; }
    }
}
