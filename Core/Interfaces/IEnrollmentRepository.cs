using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<object>> GetEnrollments();
        Task<Enrollment> GetEnrollment(int classId, string traineeId);
        Task<Enrollment> AddEnrollment(Enrollment enrollment);
        Task<Enrollment> UpdateEnrollment( Enrollment enrollment);
        Task<Enrollment> DeleteEnrollment(int classId, string traineeId);
    }
}