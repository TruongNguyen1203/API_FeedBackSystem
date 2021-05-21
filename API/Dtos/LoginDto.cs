using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage="Username must have at least 1 character!")]
        public string UserName { get; set; }
        [Required(ErrorMessage="Password must have at least 1 character!")]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}