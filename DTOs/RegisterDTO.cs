using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; }
        public string knownAs { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        [Required]
        [StringLength(8,MinimumLength =4)]
        public string password { get; set; }
    }
}