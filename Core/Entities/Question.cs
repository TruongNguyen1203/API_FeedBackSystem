using System.Collections.Generic;

namespace Core.Entities
{
    public class Question
    {
        public Question()
        {
            this.Feedback_Questions = new HashSet<Feedback_Question>();
            this.Answers = new HashSet<Answer>();
        }
        public int QuestionID {get; set;}
        public int TopicID {get; set;}
        public virtual Topic Topic { get; set; }
        public string QuestionContent {get; set;}
        public bool IsDeleted {get; set;}
         public ICollection<Answer> Answers { get; set; }
        public ICollection<Feedback_Question> Feedback_Questions { get; set; }
    }
}