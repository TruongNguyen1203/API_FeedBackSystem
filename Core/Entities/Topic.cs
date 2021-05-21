using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Topic
    {
        public Topic()
        {
            this.Questions = new HashSet<Question>();
        }
        [Key]
        public int TopicID { get; set; }
        [MaxLength(255)]
        public string TopicName { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}