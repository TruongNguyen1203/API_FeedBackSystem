using System;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class ModuleController:ControllerBase
    {
        private readonly StoreContext _context;
        public ModuleController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet("add")]
        public async Task<ActionResult> Add()
        {
            //get all admin ID
            var lstAdminID=await _context.Admins
                        .Select(x=> new {
                            AdminID=x.AdminID,
                            AdminUsername=x.AppUser.UserName
                        }).ToListAsync();

            // get all feedback title
            var lstFeedbackTitle=await _context.Feedbacks
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToListAsync();
            return Ok(new{listAdminID=lstAdminID, listFeedbackID=lstFeedbackTitle});
        }

        [HttpPost("add")]
        public  async Task<ActionResult> Add([FromBody] ModuleDto moduleDto)
        {
            var user =await _context.Admins.Include(x=>x.AppUser).Where(x=>x.AdminID==moduleDto.AdminID)
                        .SingleOrDefaultAsync();

            var feedback =await _context.Feedbacks.FindAsync(moduleDto.FeedbackTitleID);
            Module newModule = new Module()
            {
                Admin=user,
                ModuleName=moduleDto.ModuleName,
                StartTime=moduleDto.StartDate,
                EndTime=moduleDto.EndDate,
                FeedbackStartTime=moduleDto.FeedbackStartTime,
                FeedbackEndTime=moduleDto.FeedbackEndTime,
                Feedback=feedback
            };
            try
            {
                await _context.AddAsync(newModule);
                await _context.SaveChangesAsync();
                return Ok(new {success=true,messages="Add Success!"});
            }
            catch (Exception)
            {
                 return Ok(new {success=false,messages="Add fail!"});
            }
            
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var modules=await _context.Modules.Include(x=>x.Admin)
                        .Include(x=>x.Feedback)
                        .Where(x=>x.IsDelete==false)
                        .Select(x=>new ModuleDto(){
                            ModuleID=x.ModuleID,
                            ModuleName=x.ModuleName,
                            AdminID=x.AdminID,
                            StartDate=x.StartTime,
                            EndDate=x.EndTime,
                            FeedbackTitleID=x.Feedback.FeedbackID,
                            FeedbackStartTime=x.FeedbackStartTime,
                            FeedbackEndTime=x.FeedbackEndTime
                        })
                        .ToListAsync();
            return Ok(modules);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetModule(int id)
        {
            var module=await _context.Modules.FindAsync(id);
            return Ok(module);
        }
        [HttpGet("update/{id}")]
        public async Task<ActionResult> Update(int id)
        {
            var module =await _context.Modules.Include(x=>x.Feedback)
                        .Include(x=>x.Admin)
                        .Where(x=>x.ModuleID==id)
                        .FirstOrDefaultAsync();

            var lstAdminID=await _context.Admins
                        .Select(x=> new {
                            AdminID=x.AdminID,
                            AdminUsername=x.AppUser.UserName
                        }).ToListAsync();

            var lstFeedbackTitle=await _context.Feedbacks
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToListAsync();

            return Ok(new {module=true,listAdminID=lstAdminID,listFeedbackTitle=lstFeedbackTitle});
        }

        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] ModuleDto moduleDto)
        {
            var user=await _context.Admins.Include(x=>x.AppUser)
                            .Where(x=>x.AdminID==moduleDto.AdminID)
                            .FirstOrDefaultAsync();
            var feedback=await _context.Feedbacks.FindAsync(moduleDto.FeedbackTitleID);

            var module=await _context.Modules.Where(x=>x.ModuleID==moduleDto.ModuleID)
                        .Select(x=>new Module(){
                            ModuleID=x.ModuleID,
                            ModuleName=moduleDto.ModuleName,
                            StartTime=moduleDto.StartDate,
                            EndTime=moduleDto.EndDate,
                            Feedback=feedback,
                            Admin=user,
                            FeedbackStartTime=moduleDto.FeedbackStartTime,
                            FeedbackEndTime=moduleDto.FeedbackEndTime
                        }).FirstOrDefaultAsync();
            // await _context.Modules.Update(module);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new {success=true, message="Update Success!"});
            }
            catch(Exception)
            {
                 return Ok(new {success=false, message="Update fail!"});
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var module = await _context.Modules.FindAsync(id);
                module.IsDelete=true;
                await _context.SaveChangesAsync();
                return Ok(new{success=true, message="Update Success!"});
            }
            catch(Exception)
            {
                return Ok(new {success=false,message="Update fail!"});
            }
        }
        [HttpGet("trainer")]
        public async Task<ActionResult> GetAllForTrainer(string trainerID)
        {
            var assignment=await _context.Assignments.Include(x=>x.Module)
                            .Where(x=>x.TrainerID==trainerID && x.Module.IsDelete==false)
                            .Select(x=> new{
                                ModuleID=x.ModuleID,
                                ModuleName=x.Module.ModuleName,
                                AdminID=x.Module.AdminID,
                                StartDate=x.Module.StartTime,
                                EndDate=x.Module.EndTime,
                                FeedbackTitle=x.Module.Feedback.Title,
                                FeedbackStartTime=x.Module.FeedbackStartTime,
                                FeedbackEndTime=x.Module.FeedbackEndTime
                            }).ToListAsync();
            return Ok(assignment);
        }
        [HttpGet("trainee")]
        public async Task<ActionResult> GetAllForTrainee(string traineeID)
        {
            // var listModule=await _context.Assignments.Where(x=>x.Class.Enrollments.==traineeID).Select(x=>x.ClassID).ToListAsync();

            // var modules= await _context.Modules.Join(listModule,x=>x.)
            // var modules=_context.Assignments.Include(x=>x.Module).Include(x=>x.Class)
            //                                 .Join(_context.Trainee_Assignments,x=>x.Class.,y=>y.)
                                            
            return Ok();

                                                    
        }
    }
}   