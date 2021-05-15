using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public int AdminID { get; set; }
        public Admin Admin { get; set; }
        public bool IsDelete { get; set; }
        public int TypeFeedbackID { get; set; }
        public TypeFeedback TypeFeedback { get; set; }
    }
}