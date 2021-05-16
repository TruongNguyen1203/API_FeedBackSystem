using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        [MaxLength(255)]
        public string Name { get; set; }
    }
}