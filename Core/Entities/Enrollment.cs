namespace Core.Entities
{
    public class Enrollment
    {
        public int ClassID {get; set;}
        public Class Class {get; set;}
        public Trainee Trainee {get; set;}
        

    }
}