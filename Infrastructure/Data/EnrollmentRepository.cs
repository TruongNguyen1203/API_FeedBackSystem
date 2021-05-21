using System.Collections.Generic;
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

        public async Task<Enrollment> DeleteEnrollment(int classId, int traineeId)
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

        public async Task<Enrollment> GetEnrollment(int classId, int traineeId)
        {
           return await _context.Enrollments.FirstOrDefaultAsync(c => c.ClassID == classId && c.TraineeID == traineeId);
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollments()
        {
            return await _context.Enrollments.Include(c => c.Class)
                                                .Include(t => t.Trainee).ThenInclude( u => u.AppUser).ToListAsync();
        }

        public async Task<Enrollment> UpdateEnrollment(Enrollment enrollment)
        {
            var result = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.ClassID == enrollment.ClassID && e.TraineeID == enrollment.TraineeID);

            if (result != null)
            {
                result.Class = enrollment.Class;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }

    
    }
}