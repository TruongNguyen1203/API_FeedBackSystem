
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Admin
    {
        [Key]
        [MaxLength(50)]
        public string UserName { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string Password { get; set; }
    }
}