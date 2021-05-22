using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetEnrollments();
        Task<Enrollment> GetEnrollment(int classId, int traineeId);
        Task<Enrollment> AddEnrollment(Enrollment enrollment);
        Task<Enrollment> UpdateEnrollment( Enrollment enrollment);
        Task<Enrollment> DeleteEnrollment(int classId, int traineeId);
    }
}