using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var baseOnClass=CalStatistic(classID,moduleID);
            // var baseOnTopic=_context.
            return Ok(baseOnClass);
        }
        [HttpGet("topic")]
        public IActionResult PieTopic(int? classID, int? moduleID)
        {
            var lstTopic=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                        .Select(x=> x.Question.Topic.TopicName)
                                        .Distinct().ToList();
            Dictionary<string,List<PieStatistic>> result= new Dictionary<string, List<PieStatistic>>();

            foreach(var item in lstTopic)
            {
                result.Add(item,CalStatisticTopic(classID,moduleID,item));
            }
            return Ok(result);
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
        [HttpGet("admin")]
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
        [HttpGet("trainer")]
        public async Task<ActionResult> GetListClassModule(string trainerID)
        {
            var listClass=await _context.Assignments.Where(x=>x.TrainerID==trainerID)
                                                    .Select(x=> new{
                                                        ClassID=x.ClassID,
                                                        ClassName=x.Class.ClassName
                                                    }).Distinct().ToListAsync();
            var listModule=await _context.Assignments.Where(x=>x.TrainerID==trainerID)
                                                    .Select(x=> new{
                                                        ModuleID=x.ModuleID,
                                                        ModuleName=x.Module.ModuleName
                                                    }).Distinct().ToListAsync();
            return Ok(new{listClass,listModule});
        }
        public List<PieStatistic> CalStatistic(int? classID,int? moduleID)
        {
            // get all answer group by value
            int totalAnswer=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                            .Distinct().ToList().Count();
            // get statistic base on value of answer
            // group by 
            var baseOnClass=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID)
                                        .GroupBy(x=> x.Value)
                                        .Select(x=>new PieStatistic()
                                        {
                                            Value=x.Key,
                                            Percent=(decimal)x.Count()*100/totalAnswer
                                        })
                                        .ToList();
            return baseOnClass;
        } 
        public List<PieStatistic> CalStatisticTopic(int? classID,int? moduleID, string topicName)
        {
            // get all answer group by value
            int totalAnswer=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.Topic.TopicName==topicName)
                                            .Distinct().ToList().Count();
            // get statistic base on value of answer
            // group by 
            var baseOnClass=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.Topic.TopicName==topicName)
                                        .GroupBy(x=> x.Value)
                                        .Select(x=>new PieStatistic()
                                        {
                                            Value=x.Key,
                                            Percent=(decimal)x.Count()*100/totalAnswer
                                        })
                                        .ToList();
            return baseOnClass;
        }
    }
}