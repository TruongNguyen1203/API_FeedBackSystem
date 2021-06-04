using System;
using System.Collections.Generic;
using System.Globalization;
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
                                .Where(x=>x.IsDelete==false)
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToListAsync();
            return Ok(new{listAdminID=lstAdminID, listFeedbackID=lstFeedbackTitle});
        }

        [HttpPost("add")]
        public  async Task<ActionResult> Add([FromBody] ModuleDto moduleDto)
        {
            try
            {
                // check exist name
                var existModule=await _context.Modules.Where(x=>x.ModuleName==moduleDto.ModuleName.Trim() &&x.IsDelete==false).FirstOrDefaultAsync();
                if(existModule!=null)
                {
                    return Ok(new {success=false,message="Existed name!"});
                }
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
                        .Where(x=>x.IsDelete==false &&x.Feedback.IsDelete==false)
                        .Select(x=>new {
                            ModuleID=x.ModuleID,
                            ModuleName=x.ModuleName,
                            AdminID=x.AdminID,
                            AdminName=x.Admin.AppUser.UserName,
                            StartDate=x.StartTime.ToString("dd/MM/yyyy"),
                            EndDate=x.EndTime.ToString("dd/MM/yyyy"),
                            FeedbackTitleID=x.Feedback.FeedbackID,
                            FeedbackStartTime=x.FeedbackStartTime.ToString("dd/MM/yyyy HH:mm"),
                            FeedbackEndTime=x.FeedbackEndTime.ToString("dd/MM/yyyy HH:mm"),
                            isWarning=x.StartTime<DateTime.Now?false:true
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
                        .Where(x=>x.ModuleID==id &&x.IsDelete==false &&x.Feedback.IsDelete==false)
                        .Select(x=>new {
                            ModuleID=x.ModuleID,
                            ModuleName=x.ModuleName,
                            AdminID=x.AdminID,
                            AdminName=x.Admin.AppUser.UserName,
                            StartDate=x.StartTime.ToString("dd/MM/yyyy"),
                            EndDate=x.EndTime.ToString("dd/MM/yyyy"),
                            FeedbackTitleID=x.Feedback.FeedbackID,
                            FeedbackStartTime=x.FeedbackStartTime.ToString("dd/MM/yyyy HH:mm"),
                            FeedbackEndTime=x.FeedbackEndTime.ToString("dd/MM/yyyy HH:mm")
                        })
                        .FirstOrDefaultAsync();

            var lstAdminID=await _context.Admins
                        .Select(x=> new {
                            AdminID=x.AdminID,
                            AdminUsername=x.AppUser.UserName
                        }).ToListAsync();

            var lstFeedbackTitle=await _context.Feedbacks
                                .Where(x=>x.IsDelete==false)
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToListAsync();

            return Ok(new {module=module,listAdminID=lstAdminID,listFeedbackTitle=lstFeedbackTitle});
        }

        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var existModule=await _context.Modules.Where(x=>x.ModuleName==moduleDto.ModuleName.Trim() && x.ModuleID!=moduleDto.ModuleID &&x.IsDelete==false).FirstOrDefaultAsync();
                if(existModule!=null)
                {
                    return Ok(new {success=false,message="Existed name!"});
                }

                var user=await _context.Admins.Include(x=>x.AppUser)
                            .Where(x=>x.AdminID==moduleDto.AdminID)
                            .FirstOrDefaultAsync();
                var feedback=await _context.Feedbacks.FindAsync(moduleDto.FeedbackTitleID);

                var module=await _context.Modules.Where(x=>x.ModuleID==moduleDto.ModuleID &&x.IsDelete==false&&x.Feedback.IsDelete==false)
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
                    _context.Update(module);
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
                return Ok(new{success=true, message="Delete Success!"});
            }
            catch(Exception)
            {
                return Ok(new {success=false,message="Delete fail!"});
            }
        }
        [HttpGet("trainer")]
        public async Task<ActionResult> GetAllForTrainer(string trainerID)
        {
            var assignment=await _context.Assignments.Include(x=>x.Module)
                            .Where(x=>x.TrainerID==trainerID && x.Module.IsDelete==false &&x.Class.IsDeleted==false &&x.Module.Feedback.IsDelete==false)
                            .Select(x=> new{
                                ModuleID=x.ModuleID,
                                ModuleName=x.Module.ModuleName,
                                AdminID=x.Module.AdminID,
                                StartDate=x.Module.StartTime.ToString("dd/MM/yyyy"),
                                EndDate=x.Module.EndTime.ToString("dd/MM/yyyy"),
                                FeedbackTitle=x.Module.Feedback.Title,
                                FeedbackStartTime=x.Module.FeedbackStartTime.ToString("dd/MM/yyyy HH:mm"),
                                FeedbackEndTime=x.Module.FeedbackEndTime.ToString("dd/MM/yyyy HH:mm")
                            }).ToListAsync();
            return Ok(assignment);
        }
        [HttpGet("trainee")]
        public async Task<ActionResult> GetAllForTrainee(string traineeID)
        {
            var modules=await _context.Trainee_Assignments.Where(x=>x.TraineeID==traineeID)
                                            .Join(_context.Assignments.Include(x=>x.Module).ThenInclude(x=>x.Assignments),
                                            x=>x.RegistrationCode,y=>y.RegistrationCode, 
                                            (x,y)=> y.Module)
                                            .Where(y=>y.IsDelete==false &&y.Assignments.Select(x=>x.Class.IsDeleted).SingleOrDefault()==false &&
                                            y.Feedback.IsDelete==false)
                                            .Select(x=>new{
                                                ModuleID=x.ModuleID,
                                                ModuleName=x.ModuleName,
                                                AdminID=x.AdminID,
                                                AdminUserName=x.Admin.AppUser.UserName,
                                                StartDate=x.StartTime.ToString("dd/MM/yyyy"),
                                                EndDate=x.EndTime.ToString("dd/MM/yyyy"),
                                                FeedbackTitle=x.Feedback.Title,
                                                FeedbackStartTime=x.FeedbackStartTime.ToString("dd/MM/yyyy HH:mm"),
                                                FeedbackEndTime=x.FeedbackEndTime.ToString("dd/MM/yyyy HH:mm")
                                            }).ToListAsync();
            return Ok(modules);                                                    
        }
    }
    
}   