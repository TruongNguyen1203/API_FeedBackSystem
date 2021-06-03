using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ModuleDtoTestDate
    {
        public string AdminID { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string FeedbackStartTime { get; set; }
        public string FeedbackEndTime { get; set; }
        public int FeedbackTitleID { get; set; }
        
    }
}