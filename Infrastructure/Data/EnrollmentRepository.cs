using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly StoreContext _context;

        public EnrollmentRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> AddEnrollment(Enrollment enrollment)
        {
            var result = await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Enrollment> DeleteEnrollment(int classId, string traineeId)
        {
            var result = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.ClassID == classId && e.TraineeID == traineeId);

            if (result != null)
            {
                _context.Enrollments.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }
             return null;
        }

        public async Task<Enrollment> GetEnrollment(int classId, string traineeId)
        {
           return await _context.Enrollments
                            .FirstOrDefaultAsync(c => c.ClassID == classId && c.TraineeID == traineeId);
        }

        public async Task<IEnumerable<object>> GetEnrollments()
        {
            return (IEnumerable<object>)await _context.Enrollments.Select(x=> new {
                                                    traineeId=x.TraineeID,
                                                    TraineeUserName=x.Trainee.AppUser.UserName,
                                                    TraineeName=x.Trainee.AppUser.Name,
                                                    classId=x.ClassID,
                                                    ClassName=x.Class.ClassName
                                                })
                                                .ToListAsync();
        }

        public Task<Enrollment> UpdateEnrollment(Enrollment enrollment)
        {
            throw new System.NotImplementedException();
        }
    }
}