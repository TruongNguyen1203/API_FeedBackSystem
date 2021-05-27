using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController:ControllerBase
    {
        private readonly StoreContext _context;

        public DashboardController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll(string traineeID)
        {
            // get course of trainee
            var feedbacks=await _context.Trainee_Assignments.Where(x=>x.TraineeID==traineeID)
                                            .Join(_context.Assignments.Include(x=>x.Module).ThenInclude(X=>X.Feedback),
                                            x=>x.RegistrationCode,y=>y.RegistrationCode, 
                                            (x,y)=> new{
                                                Module=y.Module,
                                                Feedback=y.Module.Feedback,
                                                Class=y.Class
                                            })
                                            .Where(y=>y.Module.IsDelete==false)
                                            .Select(x=>new{
                                                FeedbackID=x.Feedback.FeedbackID,
                                                FeedbackTitle=x.Feedback.Title,
                                                ClassID=x.Class.ClassID,
                                                ClassName=x.Class.ClassName,
                                                ModuleID=x.Module.ModuleID,
                                                ModuleName=x.Module.ModuleName,
                                                EndTime=x.Module.EndTime.ToString("MM/dd/yyyy HH:mm"),
                                                StatusCode=x.Module.EndTime>DateTime.Now?"Complete":"InComplete"
                                            }).ToListAsync();
            return Ok(feedbacks);
        }
    }
}