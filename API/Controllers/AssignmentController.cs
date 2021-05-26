using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentRepository _assignRepo;
        private readonly StoreContext _context;

        public AssignmentController(IAssignmentRepository assignRepo, StoreContext context)
        {
            _assignRepo = assignRepo;
            _context = context;
        }

        //Get all assignment
        [HttpGet]
        public IActionResult GetAssignments()
        {
            try
            {
                var data = _context.Assignments.Include(a => a.Class)
                                                    .Include(a => a.Module)
                                                    .Include(a => a.Trainer)
                                                    .ThenInclude(a => a.AppUser)
                                                    .Select(x => new
                                                    {
                                                        moduleName = x.Module.ModuleName,
                                                        className = x.Class.ClassName,
                                                        trainerName = x.Trainer.AppUser.UserName,
                                                        registrationCode = x.RegistrationCode,
                                                    })
                                                    .ToList();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Assignment>>> Search(string inputText)
        {
            try
            {
                var result = await _assignRepo.SearchAssignments(inputText);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }

        }


        [HttpPost]
        public async Task<ActionResult<Assignment>> AddAssignment(Assignment assignment)
        {
            try
            {
                if (assignment == null)
                    return BadRequest();

                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                assignment.RegistrationCode = "CL" + assignment.ClassID + "M" + assignment.ModuleID + "T" + Timestamp;

                 await _assignRepo.AddAssignment(assignment);

                return Ok(new {success=true, message="Update assignment success!"}); 
                
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new Assignment record");
            }
        }


        //Edit assignment
        [HttpPut]
        public async Task<ActionResult<Assignment>> UpdateAssignment(int moduleId, int classId, string trainerId, Assignment assignment)
        {
            try
            {
                if (moduleId != assignment.ModuleID && classId !=assignment.ClassID && trainerId != assignment.TrainerID)
                    return BadRequest("moduleId, classId, trainerId mismatch");

                var assignmentToUpdate = await _assignRepo.GetAssignment(moduleId,classId,trainerId);

                if (assignmentToUpdate == null)
                    return NotFound($"Assignment not found");

                await _assignRepo.UpdateAssignment(assignment);
                 return Ok(new {success=true, message="Update assignment success!"}); 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

    }
}