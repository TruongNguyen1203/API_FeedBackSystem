using System.Linq;
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
        public IActionResult GetAll()
        {
            var feedbacks=_context.Feedbacks.Select(x=> new {
                FeedbackID=x.FeedbackID,
                Title=x.Title,
                AdminID=x.AdminID
            }).ToList();
            return Ok(feedbacks);
        }
        [HttpGet("add")]
        public IActionResult Add()
        {
            var model =_context.Topics.Include(x=>x.Questions).ToList();
            return Ok(model);
        }
        [HttpPost("add")]
        public IActionResult Add([FromBody] Feedback feedback)
        {
            var model =_context.Topics.Include(x=>x.Questions).ToList();
            return Ok(model);
        }
    }
}