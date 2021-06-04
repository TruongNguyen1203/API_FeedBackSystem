using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
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
                                            .Where(y=>y.Module.IsDelete==false && y.Module.FeedbackStartTime<=DateTime.Now &&y.Module.FeedbackEndTime>=DateTime.Now)
                                            .Select(x=>new{
                                                FeedbackID=x.Feedback.FeedbackID,
                                                FeedbackTitle=x.Feedback.Title,
                                                ClassID=x.Class.ClassID,
                                                ClassName=x.Class.ClassName,
                                                ModuleID=x.Module.ModuleID,
                                                ModuleName=x.Module.ModuleName,
                                                EndTime=x.Module.FeedbackEndTime.ToString("dd/MM/yyyy HH:mm"),
                                                StatusCode="InComplete"
                                            }).ToListAsync();
             // ====> get feedback which was not completed
            // => query where Answer has traineeID==.. && moduleID== ... => FEEDBACK COMPLETE
            var listComplete=feedbacks.Join(_context.Answers,x=>x.ModuleID,y=>y.ModuleID,
                                        (x,y)=> new{
                                            FeedbackID=x.FeedbackID,
                                            TraineeID=y.TraineeID
                                        })
                                        .Where(x=>x.TraineeID==traineeID)
                                        .Select(x=>x.FeedbackID)
                                        .Distinct().ToList();
            return Ok(new {listFeedback=feedbacks,listComplete=listComplete});
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFeedback(int id)
        {
            Dictionary<string,List<AnswerDto>> feedback = new Dictionary<string, List<AnswerDto>>();
            var allTopic= await _context.Topics.Select(x=>x.TopicName).ToListAsync();
            foreach(var item in allTopic)
            {
                feedback.Add(item,await _context.Feedback_Questions.Where(x=>x.FeedbackID==id &&x.Question.Topic.TopicName==item)
                                                .Select(x=> new AnswerDto(){
                                                    QuestionID=x.Question.QuestionID,
                                                    QuestionContent=x.Question.QuestionContent
                                                }).ToListAsync());
            }
            return Ok(feedback.Values);
        }
        [HttpPost("do")]
        public async Task<ActionResult> Execute(int id, string comment,string traineeID, List<AnswerDto> answerDto)
        {
            try
            {
                 // return Ok(answerDto);
                var module=await _context.Modules.Where(x=>x.FeedbackID==id).FirstOrDefaultAsync();
                
                // get classID to add to Trainee_Comment by join Module and Assignment
                var classOfTrainee=await _context.Assignments.Where(x=>x.ModuleID==module.ModuleID)
                                                            .Select(x=>x.Class)
                                                            .FirstOrDefaultAsync();  
                // get trainee
                var trainee=await _context.Trainees.Include(x=>x.AppUser).FirstOrDefaultAsync(x=>x.TraineeID==traineeID);          
                // add comment
                if(comment!=null)
                {
                    //check exist comment
                   
                    Trainee_Comment newComment = new Trainee_Comment()
                    {
                        ClassID=classOfTrainee.ClassID,
                        ModuleID=module.ModuleID,
                        TraineeID=traineeID,
                        Comment=comment
                    };
                    await _context.AddAsync(newComment);
                    await _context.SaveChangesAsync();
                }   
                // add answer
             
                List<Answer> newAnswer= new List<Answer>(); 
                foreach (var item in answerDto)
                {
                    newAnswer.Add( new Answer(){
                        Class=classOfTrainee,
                        Module=module,
                        Trainee=trainee,
                        Question=_context.Questions.Find(item.QuestionID),
                        Value=item.AnswerValue
                    });
                }
                await _context.AddRangeAsync(newAnswer);
                await _context.SaveChangesAsync();
                // return success
                return Ok(new{success=true, message="Submit Feedback Success"});
            }
            catch (Exception e)
            {
                return Ok(new {success=false, message=e.ToString()});
            }
           
        }
    }
}