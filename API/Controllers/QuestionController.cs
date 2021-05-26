using System.Linq;
using API.Dtos;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class QuestionController :ControllerBase
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public QuestionController(StoreContext context, IMapper mapper = null)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody]QuestionDto question)
        {
            var topic = _context.Topics.Where(x=>x.TopicID==question.TopicID).SingleOrDefault();

            var questionModel= new Question()
            {
                TopicID=topic.TopicID,
                Topic=topic,
                QuestionContent=question.QuestionContent
            };
            _context.Add(questionModel);
            _context.SaveChanges();
            return Ok(new {success=true, message="Add Question success!"});
        }

        [HttpGet("{id}")]
        public IActionResult GetQuestion(int id)
        {
            var question =_context.Questions.Where(x=>x.QuestionID==id)
                                            .Select(x=> new QuestionDto(){
                                                QuestionID=x.QuestionID,
                                                QuestionContent=x.QuestionContent,
                                                TopicID=x.TopicID
                                            }).FirstOrDefault();
            return Ok(question);
        }
        [HttpPut("{update}")]
        public IActionResult Update([FromBody]QuestionDto questionDto)
        {
            var question =_context.Questions.Where(x=>x.QuestionID==questionDto.QuestionID)
                                            .Select(x=> new Question(){
                                                QuestionID=x.QuestionID,
                                                Topic=x.Topic,
                                                IsDeleted=x.IsDeleted,
                                                Feedback_Questions=x.Feedback_Questions,
                                                Answers=x.Answers,
                                                QuestionContent=questionDto.QuestionContent
                                            }).FirstOrDefault();
            _context.Questions.Update(question);
            _context.SaveChanges();
            return Ok(new {success=true, message="Update Question success!"}); 
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            // ooh no
            var question=_context.Questions.Find(id);
            return Ok(new {success=true, message="Delete Question success!",question=question});
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var questions=_context.Questions
                        .Select(x=>new{
                            TopicID=x.TopicID,
                            TopicName=x.Topic.TopicName,
                            QuestionID=x.QuestionID,
                            QuestionContent=x.QuestionContent
                        })
                        .ToList();
            return Ok(questions);
        }

    }
}