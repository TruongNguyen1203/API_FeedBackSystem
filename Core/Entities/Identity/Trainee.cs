using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainee : IdentityUser<Guid>
    {
         public string Name { get; set; }
         public bool IsActive { get; set; } 
        public string ActivationCode { get; set; }
         public string ResetPasswordCode { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Enrollment> Enrollment { get; set; }
         
    }
}