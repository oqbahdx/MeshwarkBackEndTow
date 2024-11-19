using System.ComponentModel.DataAnnotations;

namespace Meshwark.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        public string? FcmToken { get; set; }
        [Required]
        public bool IsApproved { get; set; }
    }
}
