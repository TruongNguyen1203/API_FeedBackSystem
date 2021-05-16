using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Answer
    {
       
        public int ClassID {get; set;}
        public Class Class {get; set;}

         public int ModuleID {get; set;}
        public Module Module {get; set;}
       

        public int TraineeID {get; set;}
        public Trainee Trainee {get; set;}
        
        public int QuestionID {get; set;}
        public Question Question {get; set;}
       
        public int Value {get; set;}
        

    }
}