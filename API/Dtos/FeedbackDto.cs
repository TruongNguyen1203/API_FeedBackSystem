using System.Collections.Generic;

namespace API.Dtos
{
    public class FeedbackDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string AdminID { get; set; }
        public int TypeFeedbackID { get; set; }
        public List<int> lstQuestionID { get; set; }
    }
}