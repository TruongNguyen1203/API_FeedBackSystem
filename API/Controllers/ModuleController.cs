using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        [HttpGet]
        public async  Task<ActionResult<IReadOnlyList<Module>>> GetAll()
        {
            var result= await _context.Modules.ToListAsync();
            return Ok(result);
        }
        [HttpPost("add")]
        public  async Task<ActionResult> Add([FromBody] ModuleDto module)
        {

            if(module.AdminID.Length==0)
            {
                var adminName = HttpContext.Session.GetString(SessionKey.AdminName);
                
                module.AdminID=adminName;
            }
            var user =_context.Admins.Include(x=>x.AppUser).Where(x=>x.AppUser.UserName==module.AdminID)
                        .SingleOrDefault();
            //check feedback Title
            if(module.FeedbackTitle.Length==0)
            {
                // pass default value for feedback Title
                module.FeedbackTitle="need to change";
            }
            Module m = new Module()
            {
                Admin=user,
                ModuleName=module.ModuleName,
                StartTime=module.StartTime,
                EndTime=module.EndTime,
                IsDelete=module.IsDelete,
                FeedbackStartTime=module.FeedbackStartTime,
                FeedbackEndTime=module.FeedbackEndTime,
                Feedback=null
            };
            try
            {
                _context.Add(m);
                 return Ok(new {success="True",messages="Create Module Successfully"});
            }
            catch(Exception e)
            {
                 return Ok(new {success="Failed",messages="Create Module Successfully"});
            }
            

           
        }
        private List<string> validation(ModuleDto module)
        {
            List<string> messages= new List<string>();
            //validate 
            // if(module.StartTime <= DateTime.Now )
            // {
            //     messages.Add("Please choose start date after now date");
            // }
            // if(module.EndTime<module.StartTime)
            // {
            //     messages.Add("Please choose end date after start date");
            // }
            // if(module.FeedbackStartTime!=null && module.FeedbackEndTime<module.FeedbackStartTime)
            // {
            //     messages.Add("Please choose Feedback end date after Feedback start date");
            // }
            return messages;
        }
    }
}