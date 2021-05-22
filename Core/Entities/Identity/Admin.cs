using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Admin 
    {
        public Admin()
        {
            this.Modules = new HashSet<Module>();
            this.Feedbacks = new HashSet<Feedback>();
        }
        [Key]
        public string AdminID { get; set; }
        public AppUser AppUser { get; set; }

        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}