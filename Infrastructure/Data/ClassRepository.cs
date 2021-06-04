using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<Class> DeleteClass(int Id)
        {
             var result = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassID == Id);

            if (result != null)
            {
                result.IsDeleted = true;
                await _context.SaveChangesAsync();
                return result;
            }
             return null;
        }

        public async Task<Class> GetClassById(int id)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.ClassID == id);
        }

        public async Task<Class> GetClassByName(string className)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.ClassName == className);
        }

        public async Task<IEnumerable<object>> GetClasses()
        {
            return await _context.Classes.Where(x=> x.IsDeleted==false).Select(x=> new{
                                                ClassID=x.ClassID,
                                                ClassName=x.ClassName,
                                                Capacity=x.Capacity,
                                                StartDate=x.StartTime,
                                                EndDate=x.EndTime
                                            }).ToListAsync();
        }

         public async Task<IEnumerable<Class>> GetClassesByTrainer()
        {

            return await _context.Classes.Include(c => c.Assignments).Include(c => c.Enrollments).ThenInclude(t => t.Trainee).ThenInclude(u => u.AppUser).ToListAsync();
        }

    
    

        public async Task<Class> UpdateClass(Class @class)
        {
            var result = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassID == @class.ClassID);

            if (result != null)
            {
                result.ClassName = @class.ClassName;
                result.Capacity = @class.Capacity;
                result.StartTime = @class.StartTime;
                result.EndTime = @class.EndTime;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}