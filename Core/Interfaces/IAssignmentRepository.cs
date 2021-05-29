using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IAssignmentRepository
    {
       // Task<IEnumerable<Assignment>> GetAssignments();
        Task<Assignment> GetAssignment(int moduleId, int classId, string trainerId);
        Task<Assignment> AddAssignment(Assignment assignment);

        Task<IEnumerable<object>>  SearchAssignments(string inputText);
        Task<IEnumerable<object>>  SearchAssignmentsByTrainer(string trainerId, string inputText);

        

    }
}