namespace Core.Entities
{
    public class Enrollment
    {
        public Class Class {get; set;}
        public int ClassID {get; set;}
        public Trainee Trainee {get; set;}
        public int TraineeID {get; set;}

    }
}