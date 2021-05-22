using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ModuleDto
    {
        public string AdminID { get; set; }

        [Required(ErrorMessage="Please enter module name and less than 255")]
        [StringLength(255)]
        public string ModuleName { get; set; }

        [Required(ErrorMessage="Please choose start date or fill mm/dd/yyyy")]
        [Compare("Now",ErrorMessage="Please choose start date after now date")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage="Please choose start date or fill mm/dd/yyyy")]
        [Compare("Now",ErrorMessage="Please choose start date after now date")]
        public DateTime EndTime { get; set; }
        public bool IsDelete { get; set; }

        // [Compare("Now",ErrorMessage="Please choose Feedback start date after now date")]
        public DateTime FeedbackStartTime { get; set; }

        // [Compare("Now",ErrorMessage="Please choose Feedback end date after now date")]
        public DateTime FeedbackEndTime { get; set; }

        public string FeedbackTitle { get; set; }
        
    }
}