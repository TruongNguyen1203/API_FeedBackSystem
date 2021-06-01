using System;
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
    public class QuestionController :ControllerBase
    {
        private readonly StoreContext _context;

        public QuestionController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody]QuestionDto questionDto)
        {
            // check exist question
            var exist=await _context.Questions.Where(x=>x.TopicID==questionDto.TopicID 
                                        &&x.IsDeleted==false && x.QuestionContent.Trim()==questionDto.QuestionContent.Trim())
                                        .FirstOrDefaultAsync();
            if(exist!=null)
            {
                return Ok(new {success=false, message="Question existed!"});
            }
            try
            {
                var topic =await _context.Topics.Where(x=>x.TopicID==questionDto.TopicID).SingleOrDefaultAsync();

                var questionModel= new Question()
                {
                    TopicID=topic.TopicID,
                    Topic=topic,
                    QuestionContent=questionDto.QuestionContent
                };
                await _context.AddAsync(questionModel);
                await _context.SaveChangesAsync();
                return Ok(new {success=true, message="Add Question success!"});
            }
            catch(Exception e)
            {
                return Ok(new {success=false, message=e.ToString()});
            }
        }


        [HttpPut("{update}")]
        public async Task<ActionResult> Update(int questionID, string questionContent)
        {
            //check existed question
            var existed =await _context.Questions.Where(x=>x.QuestionID!=questionID &&x.IsDeleted==false && x.QuestionContent==questionContent)
                                            .ToListAsync();
            if(existed.Count()>0)
            {
                return Ok(new {success=false,message="Question existed!"});
            }
            try
            {
                var question =await _context.Questions.FirstOrDefaultAsync(x=>x.QuestionID==questionID);
                question.QuestionContent=questionContent;

                await _context.SaveChangesAsync();
                return Ok(new {success=true, message="Update Question success!"}); 
            }
            catch(Exception e)
            {
                return Ok(new {success=false, message=e.ToString()});
            }
            
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var question=await _context.Questions.FindAsync(id);
                question.IsDeleted=true;
                await _context.SaveChangesAsync();
                return Ok(new {success=true,message="Delete Success!"});
            }
            catch(Exception)
            {
                 return Ok(new {success=false,message="Delete fail!"});
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            var questions=await _context.Questions
                        .Where(x=>x.IsDeleted==false)
                        .Select(x=>new{
                            TopicID=x.TopicID,
                            TopicName=x.Topic.TopicName,
                            QuestionID=x.QuestionID,
                            QuestionContent=x.QuestionContent
                        })
                        .ToListAsync();
            return Ok(questions);
        }
        [HttpGet("{topicName}")]
        public async Task<ActionResult> Filter(string topicName)
        {
            var results=await _context.Questions.Where(x=>x.Topic.TopicName==topicName &&x.IsDeleted==false)
                        .Select(x=>new{
                            TopicID=x.TopicID,
                            TopicName=x.Topic.TopicName,
                            QuestionID=x.QuestionID,
                            QuestionContent=x.QuestionContent
                        }).ToListAsync();
            return Ok(results);
        }
    }
}