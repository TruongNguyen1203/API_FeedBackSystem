namespace Core.Entities
{
    public class Assignment
    {
        public Class Class {get; set;}
        public int ClassID {get; set;}
        public Module Module {get; set;}
        public int ModuleID {get; set;}
        public Trainer Trainer {get; set;}
        public int TrainerID {get; set;}
    }
}