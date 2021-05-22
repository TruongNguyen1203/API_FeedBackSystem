using System.Collections.Generic;
using Core.Entities;

namespace API.Dtos
{
    public class ClassListByTrainerDTO
    {
        public int ClassID {get;set;}
        public string ClassName {get;set;}
        public int NumberOfTrainee {get;set;}

    }
}