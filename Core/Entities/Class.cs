using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Class
    {
        [Key]
        public int ClassID {get;set;}
        public string ClassName {get; set;}
        public int Capacity {get;set;}
        public DateTime StartTime {get; set;}
        public DateTime EndTime {get; set;}
        public bool IsDeleted {get;set;}

        public ICollection<Assignment> Assignments { get; set; }
         public ICollection<Answer> Answers { get; set; }
         public ICollection<Enrollment> Enrollments { get; set; }

    }
}