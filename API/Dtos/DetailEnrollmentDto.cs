using System;

namespace API.Dtos
{
    public class DetailEnrollmentDto
    {
        public string TraineeID { get; set; }
        public string Phone { get; set; }
        public string TraineeName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int CLassID { get; set; }
        public DateTime StartTime { get; set; }
        public string ClassName { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }

    }
}