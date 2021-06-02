using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController:ControllerBase
    {
        private readonly StoreContext _context;

        public FeedbackController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var feedbacks=await _context.Feedbacks.Where(X=>X.IsDelete==false).Select(x=> new {
                FeedbackID=x.FeedbackID,
                Title=x.Title,
                AdminID=x.AdminID
            }).ToListAsync();
            return Ok(feedbacks);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFeedback(int id)
        {
            var info= await _context.Feedbacks.Where(x=>x.FeedbackID==id &&x.IsDelete==false).Select(x=> new{ 
                                           FeedbackTitle=x.Title,
                                           AdminID=x.AdminID             
                                        }).FirstOrDefaultAsync();
            Dictionary<string,List<string>> feedback = new Dictionary<string, List<string>>();
            var allTopic= await _context.Topics.Select(x=>x.TopicName).ToListAsync();
            foreach(var item in allTopic)
            {
                feedback.Add(item,await _context.Feedback_Questions.Where(x=>x.FeedbackID==id &&x.Question.Topic.TopicName==item
                                                && x.Feedback.IsDelete==false && x.Question.IsDeleted==false)
                                                .Select(x=>x.Question.QuestionContent).ToListAsync());
            }
            return Ok(new{feedbackTitle=info.FeedbackTitle,adminID=info.AdminID,content=feedback});
        }
        [HttpGet("add")]
        public async Task<ActionResult> Add()
        {
            Dictionary<string,List<QuestionDto>> feedback = new Dictionary<string, List<QuestionDto>>();
            var allTopic= await _context.Topics.Select(x=>x.TopicName).ToListAsync();
            foreach(var item in allTopic)
            {
                feedback.Add(item, await _context.Questions.Where(x=>x.IsDeleted==false && x.Topic.TopicName==item)
                                                    .Select(x=> new QuestionDto(){
                                                    QuestionID=x.QuestionID,
                                                    QuestionContent=x.QuestionContent,
                                                    TopicID=x.TopicID
                                                }).ToListAsync());
               
            }
             var result= feedback.Select(x=> new{
                    topicName=x.Key,
                    questions=x.Value
                }).ToList();
            return Ok(new {result});
        }
        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] FeedbackDto feedbackDto)
        {
            //check existed title
            var existed=await _context.Feedbacks.Where(x=>x.Title==feedbackDto.Title &&x.IsDelete==false).FirstOrDefaultAsync();
            if(existed!=null)
            {
                return Ok(new {success=false,message="Add failed! Feedback existed!"});
            }
            // get list question of feedbackDto 
            var lstQuestion = new List<Question>();
            foreach(var q in feedbackDto.lstQuestionID)
            {
                lstQuestion.Add(_context.Questions.Include(x=>x.Topic)
                            .Where(x=>x.QuestionID==q && x.IsDeleted==false)
                            .FirstOrDefault());
            }
            // have to choose at least 1 question per topic
            var topic = lstQuestion
                        .Select(x=>x.TopicID)
                        .Distinct().Count();
            
            if(topic != _context.Topics.ToList().Count)
            {
                return Ok(new {success=false,message="Add fail! You have to choose all topic!"});
            }

            // prepare
            var admin =await _context.Admins.Include(x=>x.AppUser)
                    .Where(x=>x.AdminID==feedbackDto.AdminID)
                    .FirstOrDefaultAsync();
            var typeFeedback=_context.TypeFeedbacks.Find(feedbackDto.TypeFeedbackID);

            var newFeedback= new Feedback()
            {
                Title=feedbackDto.Title,
                Admin=admin,
                TypeFeedback=typeFeedback
            };
            // add feedback_question
            List<Feedback_Question> lstFQ= new List<Feedback_Question>();
            foreach(var fq in lstQuestion)
            {
                lstFQ.Add(new Feedback_Question(){
                Feedback=newFeedback,
                Question=fq
                });
            }
            // use try catch because add both feedback and feedback_Question
            try
            {
                _context.Add(newFeedback);
                _context.SaveChanges();
                _context.AddRange(lstFQ);
                _context.SaveChanges();
                return Ok(new {success=true,message="Add success!"});
            }
            catch(Exception e)
            {
                // remove feedback when error
                _context.Remove(newFeedback);
                _context.SaveChanges();
                return Ok(new {success=false,message=e.Message.ToString()});
            }
        }
        [HttpGet("update/{id}")]
        public async Task<ActionResult> Update(int id)
        {
            // check exist feedback title
            var info= await _context.Feedbacks.Where(x=>x.FeedbackID==id &&x.IsDelete==false).Select(x=> new{ 
                                           FeedbackID=x.FeedbackID,
                                           FeedbackTitle=x.Title,
                                           AdminID=x.AdminID             
                                        }).FirstOrDefaultAsync();
            var allTopic= await _context.Topics.Select(x=>x.TopicName).ToListAsync();
            // return list question id of feedback
            var oldQuestion =await _context.Feedback_Questions.Where(x=>x.FeedbackID==id && x.Question.IsDeleted==false)
                                                                .Select(x=>x.QuestionID).ToListAsync();
            Dictionary<string,List<QuestionDto>> listFeedback = new Dictionary<string, List<QuestionDto>>();
            foreach(var item in allTopic)
            {
                listFeedback.Add(item, await _context.Questions.Where(x=>x.IsDeleted==false && x.Topic.TopicName==item)
                                                    .Select(x=> new QuestionDto(){
                                                    QuestionID=x.QuestionID,
                                                    QuestionContent=x.QuestionContent,
                                                    TopicID=x.TopicID
                                                }).ToListAsync());
            }
            var result= listFeedback.Select(x=> new{
                    topicName=x.Key,
                    questions=x.Value
                }).ToList();
            return Ok(new {info,oldQuestion,result});
        }
        [HttpPut("update")]
        public async Task<ActionResult> Update([FromBody] FeedbackDto feedbackDto)
        {
            // check exist name
            var exist =await _context.Feedbacks.Where(x=>x.FeedbackID!=feedbackDto.ID &&x.IsDelete==false && x.Title==feedbackDto.Title)
                        .FirstOrDefaultAsync();
            if(exist!=null)
            {
                return Ok(new {success=false,message="Update fail!"});
            }
           
            
            // add new questions
             // get list question of feedbackDto 
            var lstQuestion = new List<Question>();
            foreach(var q in feedbackDto.lstQuestionID)
            {
                lstQuestion.Add(await _context.Questions.Include(x=>x.Topic)
                            .Where(x=>x.QuestionID==q &&x.IsDeleted==false)
                            .FirstOrDefaultAsync());
            }
            // have to choose at least 1 question per topic
            var topic = lstQuestion
                        .Select(x=>x.TopicID)
                        .Distinct().Count();
            
            if(topic != _context.Topics.ToList().Count)
            {
                return Ok(new {success=false,message="Add fail!, You have to choose all topic!"});
            }
            
            var typeFeedback=_context.TypeFeedbacks.Find(feedbackDto.TypeFeedbackID);
            
            var admin=await _context.Admins.FirstOrDefaultAsync(x=>x.AdminID==feedbackDto.AdminID);
             // get oldFeedback to remove old list question
            var oldFeedback =await _context.Feedbacks.Include(x=>x.Feedback_Questions)
                            .Include(x=>x.Admin)
                            .Where(x=>x.FeedbackID==feedbackDto.ID)
                            .Select(x=> new Feedback(){
                                FeedbackID=x.FeedbackID,
                                Title=feedbackDto.Title,
                                Admin=admin,
                                IsDelete=x.IsDelete,
                                TypeFeedback=typeFeedback,
                                Feedback_Questions=x.Feedback_Questions
                            })
                            .FirstOrDefaultAsync();

            // add feedback_question
            List<Feedback_Question> lstFQ= new List<Feedback_Question>();
            foreach(var fq in lstQuestion)
            {
                lstFQ.Add(new Feedback_Question(){
                Feedback=oldFeedback,
                Question=fq
                });
            }
            try
            {
                // replace old questions
                _context.RemoveRange(oldFeedback.Feedback_Questions);
                _context.Feedbacks.Update(oldFeedback);
                _context.AddRange(lstFQ);
                _context.SaveChanges();
                return Ok(new {success=true, message="Update success!"});
            }
            catch(Exception e)
            {
                return Ok(new { success=false,message=e.ToString()});
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var feedback= await _context.Feedbacks.FindAsync(id);
            feedback.IsDelete=true;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new{success=true,message="Delete Success!"});
            }
            catch
            {
                return Ok(new{success=false,message="Delete fail!"});
            }
        }
    }
} 