using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ClassRepository : IClassRepository
    {
        private readonly StoreContext _context;
        public ClassRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Class> AddClass(Class @class)
        {
            var result = await _context.Classes.AddAsync(@class);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public void DeleteClass(int classId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IReadOnlyList<Answer>> GetAnswersAsync()
        {
            return await _context.Answers.ToListAsync();
        }

        public async Task<IReadOnlyList<Assignment>> GetAssignmentsAsync()
        {
            return await _context.Assignments.ToListAsync();
        }

        // Get class by Id
        public async Task<Class> GetClassByIdAsync(int id)
        {
            return await _context.Classes.FirstOrDefaultAsync(p => p.ClassID == id);
        }

        public async Task<IReadOnlyList<Class>> GetClassesAsync()
        {
           return await _context.Classes.ToListAsync();
        }

        public async Task<IReadOnlyList<Enrollment>> GetEnrollmentsAsync()
        {
            return await _context.Enrollments.ToListAsync();
        }

        public Task<Class> UpdateClass(Class @class)
        {
            throw new System.NotImplementedException();
        }
    }
}