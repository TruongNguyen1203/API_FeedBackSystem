using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainer : IdentityUser<Guid>
    {
         public string Name { get; set; }
         public string Address { get; set; }
         public bool IsActive { get; set; } 
         public int IdSkill { get; set; }
         public string ActivationCode { get; set; }
         public string ResetPasswordCode { get; set; }
         public bool IsReceiveNotification { get; set; }
          public ICollection<Assignment> Assignments { get; set; }

    }
}