using System.ComponentModel.DataAnnotations;

namespace Meshwark.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
