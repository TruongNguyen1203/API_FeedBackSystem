using System;
using System.Collections.Generic;
using System.Linq;
using API.Dtos;
using API.Extensions;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetAll()
        {
            var feedbacks=_context.Feedbacks.Select(x=> new {
                FeedbackID=x.FeedbackID,
                Title=x.Title,
                AdminID=x.AdminID
            }).ToList();
            return Ok(feedbacks);
        }
        [HttpGet("{id}")]
        public IActionResult GetFeedback(int id)
        {
            Dictionary<string,List<string>> feedback = new Dictionary<string, List<string>>();
            var allTopic=_context.Topics.Select(x=>x.TopicName).ToList();
            foreach(var item in allTopic)
            {
                feedback.Add(item,_context.Feedback_Questions.Where(x=>x.FeedbackID==id &&x.Question.Topic.TopicName==item)
                                                .Select(x=>x.Question.QuestionContent).ToList());
            }
            return Ok(feedback);
        }
        [HttpGet("add")]
        public IActionResult Add()
        {
            // get all typefeedbacks
            var typeFbs =_context.TypeFeedbacks.ToList();

            // get list topic with its question
            var topic= _context.Topics.Include(x=>x.Questions).ToList().Distinct();

            return Ok(new {TypeFeedbacks=typeFbs,topic=topic});
        }
        [HttpPost("add")]
        public IActionResult Add([FromBody] FeedbackDto feedbackDto)
        {
            //check existed title
            var existed=_context.Feedbacks.Where(x=>x.Title==feedbackDto.Title).FirstOrDefault();
            if(existed!=null)
            {
                return Ok(new {success=false,message="Add failed! Feedback existed!"});
            }
            // get list question of feedbackDto 
            var lstQuestion = new List<Question>();
            foreach(var q in feedbackDto.lstQuestionID)
            {
                lstQuestion.Add(_context.Questions.Include(x=>x.Topic)
                            .Where(x=>x.QuestionID==q)
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
            var admin =_context.Admins.Include(x=>x.AppUser)
                    .Where(x=>x.AdminID==HttpContext.Session.GetString(SessionKey.Id))
                    .FirstOrDefault();
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
        
        [HttpGet("update")]
        public IActionResult Update(int id)
        {
            // get all typefeedbacks
            var typeFbs =_context.TypeFeedbacks.ToList();

            //get all admin
            var admins=_context.Admins.Include(x=>x.AppUser).ToList();
            var topic= _context.Topics.Include(x=>x.Questions).ToList().Distinct();
            var feedback=_context.Feedbacks.Include(x=>x.Admin)
                        .Include(x=>x.Feedback_Questions)
                        .ThenInclude(x=>x.Question)
                        .ThenInclude(x=>x.Topic)
                        .Where(x=>x.FeedbackID==id)
                        .FirstOrDefault();
            return Ok(new {feedback=feedback});
        }
        
        [HttpPut("update")]
        public IActionResult Update([FromBody] FeedbackDto feedbackDto)
        {
            // check exist name
            var exist =_context.Feedbacks.Where(x=>x.Title==feedbackDto.Title)
                        .FirstOrDefault();
            if(exist!=null)
            {
                return Ok(new {success=false,message="Update fail!"});
            }
            // get oldFeedback to remove old list question
            var oldFeedback =_context.Feedbacks.Include(x=>x.Feedback_Questions)
                            .Include(x=>x.Admin)
                            .Where(x=>x.FeedbackID==feedbackDto.ID)
                            .FirstOrDefault();
            
            // add new questions
             // get list question of feedbackDto 
            var lstQuestion = new List<Question>();
            foreach(var q in feedbackDto.lstQuestionID)
            {
                lstQuestion.Add(_context.Questions.Include(x=>x.Topic)
                            .Where(x=>x.QuestionID==q)
                            .FirstOrDefault());
            }
            // have to choose at least 1 question per topic
            var topic = lstQuestion
                        .Select(x=>x.TopicID)
                        .Distinct().Count();
            
            if(topic != _context.Topics.ToList().Count)
            {
                return Ok(new {success=false,message="Add fail!",topic=topic});
            }
            
            var typeFeedback=_context.TypeFeedbacks.Find(feedbackDto.TypeFeedbackID);
            oldFeedback.Title=feedbackDto.Title;
            oldFeedback.TypeFeedback=typeFeedback;
            

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
    }
}