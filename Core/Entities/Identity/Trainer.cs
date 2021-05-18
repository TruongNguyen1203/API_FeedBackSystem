using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainer
    {
        [Key]
        public int TrainerID { get; set; }
        public AppUser AppUser { get; set; }
         public string Address { get; set; }
         public bool IsActive { get; set; } 
         public int IdSkill { get; set; }
         public string ActivationCode { get; set; }
         public string ResetPasswordCode { get; set; }
         public bool IsReceiveNotification { get; set; }
          public ICollection<Assignment> Assignments { get; set; }

    }
}