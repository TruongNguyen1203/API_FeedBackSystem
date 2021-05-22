using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Feedback
    {
        public Feedback()
        {
            this.Feedback_Questions = new HashSet<Feedback_Question>();
            this.Modules = new HashSet<Module>();
        }
        [Key]
        public int FeedbackID { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public string AdminID { get; set; }
        public virtual Admin Admin { get; set; }
        public bool IsDelete { get; set; }
        public int TypeFeedbackID { get; set; }
        public virtual TypeFeedback TypeFeedback { get; set; }
        public virtual ICollection<Feedback_Question> Feedback_Questions { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
    }
}