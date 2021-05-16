
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Admin : IdentityUser
    {
        [MaxLength(255)]
        public string Name { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}