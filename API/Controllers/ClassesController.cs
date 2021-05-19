using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassRepository _repo;

        public ClassesController (IClassRepository repo)
        {
            _repo = repo;
        }

        // [HttpGet]
        // public async Task<ActionResult<List<Class>>> GetClassess()
        // {
            
        //     // var classes = await _repo.GetClassesAsync();
        //     // return Ok(classes);
        // }
        
    }
}