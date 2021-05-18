using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainee 
    {
        [Key]
        public int TraineeID { get; set; }
        public AppUser AppUser { get; set; }
         public bool IsActive { get; set; } 
        public string ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
        
        public ICollection<Answer> Answers { get; set; }
        // public ICollection<Enrollment> Enrollments { get; set; }
         
    }
}