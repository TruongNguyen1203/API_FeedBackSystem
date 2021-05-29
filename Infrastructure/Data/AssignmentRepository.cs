using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly StoreContext _context;

        public AssignmentRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Assignment> AddAssignment(Assignment assignment)
        {
            var result = await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public Task<Assignment> DeleteAssignent(int Id)
        {
            throw new System.NotImplementedException();
        }

        // Get a assignment
        public async Task<Assignment> GetAssignment(int moduleId, int classId, string trainerName)
        {
            return await _context.Assignments.Include(a => a.Class)
                                                .Include(a => a.Module)
                                                .Include(a => a.Trainer)
                                                .ThenInclude(a => a.AppUser)
                                                .FirstOrDefaultAsync(a => a.ClassID == classId
                                                                           && a.ModuleID == moduleId
                                                                           && a.Trainer.AppUser.UserName == trainerName);
        }


        public async Task<IEnumerable<object>> SearchAssignments(string inputText)
        {
            IQueryable<Assignment> query = _context.Assignments.Include(a => a.Class)
                                                                .Include(a => a.Module)
                                                                .Include(a => a.Trainer)
                                                                .ThenInclude(a => a.AppUser);
            if (!string.IsNullOrEmpty(inputText))
            {
                query = query.Where(a => a.Module.ModuleName.Contains(inputText)
                                            || a.Class.ClassName.Contains(inputText)
                                            || a.Trainer.AppUser.UserName.Contains(inputText)
                                            || a.RegistrationCode.Contains(inputText));
                
                
            }
            return (IEnumerable<object>)await query.Select(x => new
                                                    {
                                                        moduleId = x.ModuleID,
                                                        moduleName = x.Module.ModuleName,
                                                        classId = x.ClassID,
                                                        className = x.Class.ClassName,
                                                        trainerId = x.TrainerID,
                                                        trainerName = x.Trainer.AppUser.UserName,
                                                        registrationCode = x.RegistrationCode,
                                                    })
                                                .ToListAsync();
        }

        public async Task<IEnumerable<object>> SearchAssignmentsByTrainer(string trainerName, string inputText)
        {
            IQueryable<Assignment> query = _context.Assignments.Include(a => a.Class)
                                                                .Include(a => a.Module)
                                                                .Include(a => a.Trainer)
                                                                .ThenInclude(a => a.AppUser);
            if (!string.IsNullOrEmpty(inputText))
            {
                query = query.Where(a => a.Trainer.AppUser.UserName == trainerName && (a.Module.ModuleName.Contains(inputText)
                                                                        || a.Class.ClassName.Contains(inputText)
                                                                        || a.Trainer.AppUser.UserName.Contains(inputText)
                                                                        || a.RegistrationCode.Contains(inputText)));
                
                
            }
            return (IEnumerable<object>)await query.Select(x => new
                                                    {
                                                       moduleId = x.ModuleID,
                                                        moduleName = x.Module.ModuleName,
                                                        classId = x.ClassID,
                                                        className = x.Class.ClassName,
                                                        trainerId = x.TrainerID,
                                                        trainerName = x.Trainer.AppUser.UserName,
                                                        registrationCode = x.RegistrationCode,
                                                    })
                                                .ToListAsync();
        }
    }
}