namespace Core.Entities
{
    public class Answer
    {
        public Class Class {get; set;}
        public int ClassID {get; set;}
        public Module Module {get; set;}
        public int ModuleID {get; set;}
        public Trainee Trainee {get; set;}
        public int TraineeID {get; set;}

        public Question Question {get; set;}
        public int QuestionID {get; set;}
        public int Value {get; set;}
        

    }
}