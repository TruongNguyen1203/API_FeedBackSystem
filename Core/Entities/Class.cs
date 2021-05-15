using System;

namespace Core.Entities
{
    public class Class
    {
        public int ClassID {get;set;}
        public string ClassName {get; set;}
        public int Capacity {get;set;}
        public DateTime StartTime {get; set;}
        public DateTime EndTime {get; set;}
        public bool IsDeleted {get;set;}

    }
}