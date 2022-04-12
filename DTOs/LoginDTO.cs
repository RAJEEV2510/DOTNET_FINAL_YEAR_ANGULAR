using System.ComponentModel.DataAnnotations;
namespace API.DTOs

{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]

        public string password { get; set; }

    }
}