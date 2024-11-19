namespace Meshwark.DTOs
{
    public class UpdateDriverInfoDto
    {
        public string Gender { get; set; }
        public bool? IsOnline { get; set; }
        public int? AvailableSeats { get; set; }
        public int? ReservedSeats { get; set; }
        public string NextDestination { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CarColor { get; set; }
        public int? CarYear { get; set; }
        public string CarModel { get; set; }
        public string TypeOfTrip { get; set; }
        public string PlateImage { get; set; }
        public string InsuranceImage { get; set; }
        public string NumberPlate { get; set; }
        public string LicenseImage { get; set; }
        public double? Rating { get; set; }
        public int? CompletedTrips { get; set; }
        public int? CanceledTrips { get; set; }
        public string FcmToken{ get; set; }
    }
}
