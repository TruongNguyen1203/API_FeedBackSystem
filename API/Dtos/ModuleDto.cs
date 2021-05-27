using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ModuleDto
    {
        public string AdminID { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FeedbackStartTime { get; set; }
        public DateTime FeedbackEndTime { get; set; }
        public int FeedbackTitleID { get; set; }
        
    }
}