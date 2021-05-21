using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Class
    {

        public Class()
        {
            this.Enrollments = new HashSet<Enrollment>();
            this.Assignments = new HashSet<Assignment>();
            this.Answers = new HashSet<Answer>();
        }
        [Key]
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int Capacity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}