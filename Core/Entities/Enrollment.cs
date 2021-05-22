using System;

namespace Core.Entities
{
    public class Enrollment
    {
        public int ClassID {get; set;}
        public virtual  Class Class {get; set;}
        public string TraineeID {get; set;}

        public virtual Trainee Trainee {get; set;}
        

    }
}