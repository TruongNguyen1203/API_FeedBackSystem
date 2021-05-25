using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using Core.Entities;
using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [Authorize(Roles=Role.Admin)]
    [Route("api/[controller]")]
    public class ModuleController:ControllerBase
    {
        private readonly StoreContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ModuleController(StoreContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet("add")]
        public IActionResult Add()
        {
            //get all admin ID
            var lstAdminID=_context.Admins
                        .Select(x=> new {
                            AdminID=x.AdminID,
                            AdminUsername=x.AppUser.UserName
                        }).ToList();

            // get all feedback title
            var lstFeedbackTitle=_context.Feedbacks
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToList();
            return Ok(new{listAdminID=lstAdminID, listFeedbackID=lstFeedbackTitle});
        }

        [HttpPost("add")]
        public  IActionResult Add([FromBody] ModuleDto moduleDto)
        {
            // get current admin if admin = null
            if(moduleDto.AdminID.Length==0)
            {
                var adminID = HttpContext.Session.GetString(SessionKey.Id);
                
                moduleDto.AdminID=adminID;
            }
            var user =_context.Admins.Include(x=>x.AppUser).Where(x=>x.AdminID==moduleDto.AdminID)
                        .SingleOrDefault();


            var feedback =_context.Feedbacks.Find(moduleDto.FeedbackTitleID);
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
            _context.Add(newModule);
            _context.SaveChanges();
            return Ok(new {success=true,messages="Create Module Successfully"});
        }
        // [Authorize(Roles=Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var modules=_context.Modules.Include(x=>x.Admin)
                        .Include(x=>x.Feedback)
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
                        .ToList();
            return Ok(modules);
        }
        [HttpGet("{id}")]
        public IActionResult GetModule(int id)
        {
            var module=_context.Modules.Find(id);
            return Ok(module);
        }
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            var module =_context.Modules.Include(x=>x.Feedback)
                        .Include(x=>x.Admin)
                        .Where(x=>x.ModuleID==id)
                        .FirstOrDefault();

            var lstAdminID=_context.Admins
                        .Select(x=> new {
                            AdminID=x.AdminID,
                            AdminUsername=x.AppUser.UserName
                        }).ToList();

            var lstFeedbackTitle=_context.Feedbacks
                                .Select(x=> new{
                                    FeedbackID=x.FeedbackID,
                                    lstFeedbackTitle=x.Title
                                }).ToList();

            return Ok(new {module=true,listAdminID=lstAdminID,listFeedbackTitle=lstFeedbackTitle});
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] ModuleDto moduleDto)
        {
            var user=_context.Admins.Include(x=>x.AppUser)
                            .Where(x=>x.AdminID==moduleDto.AdminID)
                            .FirstOrDefault();
            var feedback=_context.Feedbacks.Find(moduleDto.FeedbackTitleID);

            var module=_context.Modules.Where(x=>x.ModuleID==moduleDto.ModuleID)
                        .Select(x=>new Module(){
                            ModuleID=x.ModuleID,
                            ModuleName=moduleDto.ModuleName,
                            StartTime=moduleDto.StartDate,
                            EndTime=moduleDto.EndDate,
                            Feedback=feedback,
                            Admin=user,
                            FeedbackStartTime=moduleDto.FeedbackStartTime,
                            FeedbackEndTime=moduleDto.FeedbackEndTime
                        }).FirstOrDefault();
            _context.Modules.Update(module);
            _context.SaveChanges();
            return Ok(new {success=true, message="Update Success!"});
        }

        // [Authorize(Roles=Role.Trainer)]
        [HttpGet("trainer")]
        public IActionResult GetAllForTrainer()
        {
            string trainerID=HttpContext.Session.GetString(SessionKey.Id);
            var assignment=_context.Assignments.Include(x=>x.Module)
                            .Where(x=>x.TrainerID==trainerID)
                            .Select(x=> new{
                                ModuleID=x.ModuleID,
                                ModuleName=x.Module.ModuleName,
                                AdminID=x.Module.AdminID,
                                StartDate=x.Module.StartTime,
                                EndDate=x.Module.EndTime,
                                FeedbackTitle=x.Module.Feedback.Title,
                                FeedbackStartTime=x.Module.FeedbackStartTime,
                                FeedbackEndTime=x.Module.FeedbackEndTime
                            }).ToList();
            return Ok(assignment);
        }
    }
}