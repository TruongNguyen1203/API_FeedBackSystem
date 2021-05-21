using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Assignment
    {
        public virtual Class Class {get; set;}
        public int ClassID {get; set;}
        public virtual Module Module {get; set;}
        public int ModuleID {get; set;}
        public virtual Trainer Trainer {get; set;}
        public int TrainerID {get; set;}
    }
}