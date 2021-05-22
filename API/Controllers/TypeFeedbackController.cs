using System.Linq;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Only Admin
    public class TypeFeedbackController:ControllerBase
    {
        private readonly StoreContext _context;

        public TypeFeedbackController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var typeFeedbacks=_context.TypeFeedbacks.ToList();
            return Ok(typeFeedbacks);
        }
        [HttpGet("{id}")]
        public IActionResult GetTypeFeedback(int id)
        {
            var typeFeedbacks =_context.TypeFeedbacks.Find(id);
            return Ok(typeFeedbacks);
        }
        [HttpPost("{add}")]
        public IActionResult Add(string typeName)
        {
            var exist=_context.TypeFeedbacks.Where(x=>x.TypeName==typeName).SingleOrDefault();
            if(exist!=null)
            {
                 return Ok(new {success=false,message="Add fail!"});
            }
            var typeFeedback= new TypeFeedback()
            {
                TypeName=typeName
            };
            _context.Add(typeFeedback);
            _context.SaveChanges();
            return Ok(new {success=true,message="Add success!"});
        }
        [HttpPut("{update}")]
        public IActionResult Update([FromBody] TypeFeedback typeFeedback)
        {
            // check exist name 
            var exist=_context.TypeFeedbacks
                .Where(x=>x.TypeName==typeFeedback.TypeName).SingleOrDefault();
            if(exist!=null)
            {
                 return Ok(new {success=false,message="Update fail!"});
            }
            var newTypeFeedback= _context.TypeFeedbacks.Find(typeFeedback.TypeID);
            newTypeFeedback.TypeName=typeFeedback.TypeName;

            _context.Update(newTypeFeedback);
            _context.SaveChanges();
            return Ok(new {success=true,message="Update success!"});
        }
    }
}