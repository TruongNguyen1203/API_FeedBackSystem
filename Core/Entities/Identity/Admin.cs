using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Admin : AppUser
    {
      
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}