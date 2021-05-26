using System;
using System.Linq;
using API.Dtos;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController:ControllerBase
    {
        private readonly StoreContext _context;

        public StatisticController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet("pie")]
        public IActionResult Pie(int? classID, int? moduleID)
        {
            if(classID==null)
            {
                classID=_context.Classes.Select(x=>x.ClassID).FirstOrDefault();
            }
            if(moduleID==null)
            {
                moduleID=_context.Modules.Select(x=>x.ModuleID).FirstOrDefault();
            }
            // get all answer
            int totalAnswer=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                            .Distinct().ToList().Count();
            // get statistic base on value of answer
            // group by 
            var values=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                        .GroupBy(x=> x.Value)
                                        .Select(x=>new 
                                        {
                                            Value=x.Key,
                                            Percent=(decimal)x.Count()*100/totalAnswer
                                        })
                                        .ToList();
            return Ok(values);
        }
        [HttpGet("comment")]
        // chưa test
        public IActionResult GetAllComment(int? classID, int? moduleID)
        {
            if(classID==null)
            {
                classID=_context.Classes.Select(x=>x.ClassID).FirstOrDefault();
            }
            if(moduleID==null)
            {
                moduleID=_context.Modules.Select(x=>x.ModuleID).FirstOrDefault();
            }
            var comments=_context.Trainee_Comments.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                                    .Join(_context.Trainees,x=>x.TraineeID,y=>y.TraineeID,
                                                    (x,y)=>new{
                                                        TraineeID=y.AppUser.UserName,
                                                        Content=x.Comment
                                                    })
                                                    .ToList();

            return Ok( comments);
        }
        [HttpGet("select")]
        public IActionResult GetSelectList()
        {
            // get all class
            var classes =_context.Classes.Select(x=> new {
                                            ClassID=x.ClassID,
                                            ClassName=x.ClassName
                                        }).ToList();

            // get all course (module)
            var courses =_context.Modules.Select(x=> new{
                                            ModuleID=x.ModuleID,
                                            ModuleName=x.ModuleName
                                        }).ToList();
            return Ok(new {classes,courses});
        }
        
    }
}