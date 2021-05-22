using System.Collections.Generic;

namespace API.ViewModel
{
    public class TraineeListVM
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<TraineeVM> TraineeList {get; set;} = new List<TraineeVM>();
    }
}