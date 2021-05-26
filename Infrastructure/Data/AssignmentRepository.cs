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
        public async Task<Assignment> GetAssignment(int moduleId, int classId, string trainerId)
        {
             return await _context.Assignments.FirstOrDefaultAsync(a => a.ClassID == classId
                                                                            && a.ModuleID == moduleId
                                                                            && a.TrainerID == trainerId);
        }


        public async Task<IEnumerable<Assignment>> SearchAssignments(string inputText)
        {
            IQueryable<Assignment> query = _context.Assignments.Include(a => a.Class)
                                                                .Include(a => a.Module)
                                                                .Include(a => a.Trainer)
                                                                .ThenInclude(a => a.AppUser);
            if(!string.IsNullOrEmpty(inputText))
            {
                query = query.Where(a => a.Module.ModuleName.Contains(inputText)
                                            || a.Class.ClassName.Contains(inputText)
                                            || a.Trainer.AppUser.UserName.Contains(inputText)
                                            || a.RegistrationCode.Contains(inputText));
            }
            return await query.ToListAsync();
        }

        //Update assignment
        public async Task<Assignment> UpdateAssignment(Assignment assignment)
        {
           var result = await _context.Assignments.FirstOrDefaultAsync(a => a.ClassID == assignment.ClassID 
                                                                            && a.ModuleID == assignment.ModuleID
                                                                            && a.TrainerID == assignment.TrainerID);

            if (result != null)
            {
                result.ClassID = assignment.ClassID;
                result.ModuleID = assignment.ModuleID;
                result.TrainerID = assignment.TrainerID;
                result.RegistrationCode = assignment.RegistrationCode;

                await _context.SaveChangesAsync();

                return result;
            }

            return null;
        }


    }
}