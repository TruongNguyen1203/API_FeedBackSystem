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
            var question =_context.Questions.Find(id);
            var q= new QuestionDto()
            {
                QuestionContent=question.QuestionContent,
                TopicID=question.TopicID
            };

            return Ok(q);
        }
        [HttpPut("{update}")]
        public IActionResult Update([FromBody]Question question)
        {
            var topic = _context.Topics.Where(x=>x.TopicID==question.TopicID).SingleOrDefault();

            question.Topic=topic;
            _context.Questions.Update(question);
            _context.SaveChanges();
            return Ok(new {success=true, message="Add Question success!"}); 
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            // ooh no
            var question=_context.Questions.Find(id);
             return Ok(new {success=true, message="Add Question success!",question=question});
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var questions=_context.Questions.Include(x=>x.Topic).ToList();
            return Ok(questions);
        }

    }
}