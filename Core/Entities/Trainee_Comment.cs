using System.ComponentModel.DataAnnotations;
namespace Core.Entities
{
    public class Trainee_Comment
    {
       
        public int ClassID { get; set; }
     
        public int ModuleID { get; set; }
        
        [MaxLength(50)]
        public string TraineeID { get; set; }
        [MaxLength(255)]
        public string Comment { get; set; }
    }
}