using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Core.Entities
{
    public class Feedback_Question
    {
        public int FeedbackID { get; set; }
        public Feedback Feedback { get; set; }
        public int QuestionID { get; set; }
         public Question Question { get; set; }
    }
}