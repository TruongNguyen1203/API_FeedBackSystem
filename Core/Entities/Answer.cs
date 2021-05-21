using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Answer
    {
       
        public int ClassID {get; set;}
        public virtual Class Class {get; set;}

         public int ModuleID {get; set;}
        public virtual Module Module {get; set;}
       

        public int TraineeID {get; set;}
        public virtual Trainee Trainee {get; set;}
        
        public int QuestionID {get; set;}
        public virtual Question Question {get; set;}
       
        public int Value {get; set;}
        

    }
}