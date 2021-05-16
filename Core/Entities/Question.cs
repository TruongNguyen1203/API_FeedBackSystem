using System.Collections.Generic;

namespace Core.Entities
{
    public class Question
    {
        public int QuestionID {get; set;}
        public int TopicID {get; set;}
        public string QuestionContent {get; set;}
        public bool IsDeleted {get; set;}
         public ICollection<Answer> Answers { get; set; }
    }
}