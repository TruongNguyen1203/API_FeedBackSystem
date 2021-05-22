using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Trainee 
    {
         public Trainee()
        {
            this.Enrollments = new HashSet<Enrollment>();
            this.Answers = new HashSet<Answer>();
        }
        [Key]
        public string TraineeID { get; set; }
        public AppUser AppUser { get; set; }
         public bool IsActive { get; set; } 
        public string ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
        
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
         
    }
}