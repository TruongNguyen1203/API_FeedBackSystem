using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Trainee_Assignment
    {
        [MaxLength(50)]
        public string RegistrationCode { get; set; }
        [MaxLength(50)]
        public string TraineeID { get; set; }
    }
}