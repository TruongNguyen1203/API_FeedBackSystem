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
                classID=_context.Classes.Where(x=>x.IsDeleted==false).Select(x=>x.ClassID).FirstOrDefault();
            }
            if(moduleID==null)
            {
                moduleID=_context.Modules.Where(x=>x.IsDelete==false).Select(x=>x.ModuleID).FirstOrDefault();
            }
            var baseOnClass=CalStatistic(classID,moduleID);
            return Ok(baseOnClass);
        }
        [HttpGet("topic")]
        public IActionResult PieTopic(int? classID, int? moduleID)
        {
            var lstTopic=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID 
                                        &&x.Class.IsDeleted==false &&x.Module.IsDelete==false)
                                        .Select(x=> x.Question.Topic.TopicName)
                                        .Distinct().ToList();
            Dictionary<string,List<PieStatistic>> result= new Dictionary<string, List<PieStatistic>>();

            foreach(var item in lstTopic)
            {
                result.Add(item,CalStatisticTopic(classID,moduleID,item));
            }
            var temp=result.Select(x=> new{
                        TopicName=x.Key,
                        Value=x.Value
            }).ToList();
            return Ok(new{data=temp});
        }
        [HttpGet("comment")]
        // chưa test
        public IActionResult GetAllComment(int? classID, int? moduleID)
        {
            if(classID==null)
            {
                classID=_context.Classes.Where(x=>x.IsDeleted==false).Select(x=>x.ClassID).FirstOrDefault();
            }
            if(moduleID==null)
            {
                moduleID=_context.Modules.Where(x=>x.IsDelete==false).Select(x=>x.ModuleID).FirstOrDefault();
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
            var classes =_context.Classes.Where(x=>x.IsDeleted==false).Select(x=> new {
                                            ClassID=x.ClassID,
                                            ClassName=x.ClassName
                                        }).ToList();

            // get all course (module)
            var courses =_context.Modules.Where(x=>x.IsDelete==false).Select(x=> new{
                                            ModuleID=x.ModuleID,
                                            ModuleName=x.ModuleName
                                        }).ToList();
            return Ok(new {classes,courses});
        }
        [HttpGet("trainer")]
        public async Task<ActionResult> GetListClassModule(string trainerID)
        {
            var listClass=await _context.Assignments.Where(x=>x.TrainerID==trainerID &&x.Class.IsDeleted==false &&x.Module.IsDelete==false)
                                                    .Select(x=> new{
                                                        ClassID=x.ClassID,
                                                        ClassName=x.Class.ClassName
                                                    }).Distinct().ToListAsync();
            var listModule=await _context.Assignments.Where(x=>x.TrainerID==trainerID &&x.Module.IsDelete==false &&x.Class.IsDeleted==false)
                                                    .Select(x=> new{
                                                        ModuleID=x.ModuleID,
                                                        ModuleName=x.Module.ModuleName
                                                    }).Distinct().ToListAsync();
            return Ok(new{listClass,listModule});
        }
        public List<PieStatistic> CalStatistic(int? classID,int? moduleID)
        {
            // get all answer group by value
            int totalAnswer=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.IsDeleted==false)
                                            .Distinct().ToList().Count();
            // get statistic base on value of answer
            // group by 
            var baseOnClass=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.IsDeleted==false)
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
            int totalAnswer=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.Topic.TopicName==topicName &&x.Question.IsDeleted==false)
                                            .Distinct().ToList().Count();
            // get statistic base on value of answer
            // group by 
            var baseOnClass=_context.Answers.Where(x=>x.ClassID==classID && x.ModuleID==moduleID &&x.Question.Topic.TopicName==topicName &&x.Question.IsDeleted==false)
                                        .GroupBy(x=> x.Value)
                                        .Select(x=>new PieStatistic()
                                        {
                                            Value=x.Key,
                                            Percent=(decimal)x.Count()*100/totalAnswer
                                        })
                                        .ToList();
            return baseOnClass;
        }

        [HttpGet("answer")]
        public async Task<ActionResult> GetAnswer(int classID, int moduleID)
        {
            Dictionary<string,object> basequestion = new Dictionary<string, object>();
            var topic =await _context.Topics.Select(x=>x.TopicName).ToListAsync();
            foreach (var topicname in topic)
            {
                int totalAnswer = _context.Answers.Where(x=>x.ClassID== classID && x.ModuleID==moduleID &&
                                            x.Class.IsDeleted==false &&x.Module.IsDelete==false &&x.Question.Topic.TopicName==topicname)
                                            .Distinct().ToList().Count();

                var temp= await _context.Answers.Where(x=>x.ClassID== classID && x.ModuleID==moduleID &&
                                            x.Class.IsDeleted==false &&x.Module.IsDelete==false &&x.Question.Topic.TopicName==topicname)
                                            .GroupBy(x=> new {x.QuestionID,x.Value})
                                            .Select(x=>new
                                            {
                                                QuestionID=x.Key.QuestionID,
                                                Value=x.Key.Value,
                                                Percent=(decimal)x.Count()*100/totalAnswer
                                            }).Distinct()
                                            .ToListAsync();
                basequestion.Add(topicname,temp);
            }
            var result=basequestion.Select(x=> new{
                topicName=x.Key,
                data=x.Value
            }).ToList();

            return Ok(result);
        }
    }
}