using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meshwark.Models;

namespace Meshwark.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string Body { get; set; }

        // Foreign key relationship with User
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
