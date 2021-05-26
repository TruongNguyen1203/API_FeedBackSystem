using System.Linq;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

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
        // [HttpGet]
        // public IActionResult Index()
        // {
        //     // get all class
        //     var classes =_context.Classes.Select(x=> new {
        //                                     ClassID=x.ClassID,
        //                                     ClassName=x.ClassName
        //                                 }).ToList();

        //     // get all course (module)
        //     var course =_context.Modules.Select(x=> new{
        //                                     ModuleID=x.ModuleID,
        //                                     ModuleName=x.ModuleName
        //                                 }).ToList();

        //     // get statistic 
        // }
        
    }
}