using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainee : IdentityUser
    {
         public string Name { get; set; }
         public bool IsActive { get; set; } 
        public string ActivationCode { get; set; }
         public string ResetPasswordCode { get; set; }
    }
}