using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Module
    {
         public Module()
        {
            this.Assignments = new HashSet<Assignment>();
            this.Answers = new HashSet<Answer>();
        }
        [Key]
        public int ModuleID { get; set; }
        public string AdminID { get; set; }
        public virtual Admin Admin { get; set; }
        [MaxLength(50)]       
        public string ModuleName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDelete { get; set; }
        public DateTime FeedbackStartTime { get; set; }
        public DateTime FeedbackEndTime { get; set; }
        public int FeedbackID { get; set; }     // co nghe t noi ko
        public virtual Feedback Feedback { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }  
    }
}