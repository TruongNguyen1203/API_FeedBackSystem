using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Topic
    {
        [Key]
        public int TopicID { get; set; }
        [MaxLength(255)]
        public string TopicName { get; set; }
    }
}