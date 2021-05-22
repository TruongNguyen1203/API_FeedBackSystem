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
                _context.Classes.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }
             return null;
        }

        public async Task<Class> GetClassById(int id)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.ClassID == id);
        }

        public async Task<IEnumerable<Class>> GetClasses()
        {
            return await _context.Classes.ToListAsync();
        }

    

        public async Task<Class> UpdateClass(Class @class)
        {
            var result = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassID == @class.ClassID);

            if (result != null)
            {
                result = @class;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}