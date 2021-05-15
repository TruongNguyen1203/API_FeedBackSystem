using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Module
    {
        [Key]
        public int ModuleID { get; set; }
        public int AdminID { get; set; }
        public Admin Admin { get; set; }
        [MaxLength(50)]       
         public string ModuleName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDelete { get; set; }
        public DateTime FeedbackStartTime { get; set; }
        public DateTime FeedbackEndTime { get; set; }
        public int FeedbackID { get; set; }
        public Feedback Feedback { get; set; }
    }
}