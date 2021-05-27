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
        Task<Assignment> UpdateAssignment(Assignment assignment);
        Task<Assignment> DeleteAssignent(int Id);

        Task<IEnumerable<Assignment>> SearchAssignments(string inputText);

        

    }
}