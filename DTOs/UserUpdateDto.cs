namespace Meshwark.DTOs
{
    public class UserUpdateDto
    {
        public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FcmToken { get; set; }
        public IFormFile? PersonalImage { get; set; }
        public string? Gender { get; set; }
        public bool? IsActive { get; set; }
        // Add other properties as nullable
        public bool? IsOnline { get; set; }
        public int? AvailableSeats { get; set; }
        public int? ReservedSeats { get; set; }
        public string? NextDestination { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? CarColor { get; set; }
        public int? CarYear { get; set; }
        public string? CarModel { get; set; }
        public string? TypeOfTrip { get; set; }
        public IFormFile? PlateImage { get; set; }
        public IFormFile? InsuranceImage { get; set; }
        public string? NumberPlate { get; set; }
        public IFormFile? LicenseImage { get; set; }
        public double? Rating { get; set; }
        public int? CompletedTrips { get; set; }
        public int? CanceledTrips { get; set; }
        public bool? IsTripActive { get; set; }
        public IFormFile? IdImage { get; set; }
        public bool? IsApproved { get; set; }
    }
}
