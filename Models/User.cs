using System.ComponentModel.DataAnnotations;

namespace Meshwark.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string  FcmToken { get; set; }

        public string? Role { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? PersonalImagePath { get; set; }
        public bool? HasProfile { get; set; } = false;
        // Driver-specific fields
        public string? Gender { get; set; }
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
        public string? PlateImage { get; set; }
        public string? InsuranceImage { get; set; }
        public string? NumberPlate { get; set; }
        public string? LicenseImage { get; set; }
        public double? Rating { get; set; }
        public int? CompletedTrips { get; set; }
        public int? CanceledTrips { get; set; }
        public bool? IsTripActive { get; set; }
        public string? IdImage { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpExpirationTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for notifications
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        // Navigation property for trip history
        
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

    }
}
