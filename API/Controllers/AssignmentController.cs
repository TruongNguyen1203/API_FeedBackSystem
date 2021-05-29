using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
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

        //trainer get his assignment
        [HttpGet("trainer/{trainerId}")]
        public IActionResult GetAssignmentsByTrainer(string trainerId)
        {
            try
            {
                var data = _context.Assignments.Where(a => a.TrainerID == trainerId).Include(a => a.Class)
                                                    .Include(a => a.Module)
                                                    .Include(a => a.Trainer)
                                                    .ThenInclude(a => a.AppUser)
                                                    .Select(x => new
                                                    {
                                                        moduleId = x.ModuleID,
                                                        moduleName = x.Module.ModuleName,
                                                        classId = x.ClassID,
                                                        className = x.Class.ClassName,
                                                        trainerId = x.TrainerID,
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
        //trainer search
        [HttpGet("trainer/{trainerId}/{inputText}")]
        public async Task<ActionResult<IEnumerable<Assignment>>> SearchByTrainer(string trainerId, string inputText)
        {
            try
            {
                var result = await _assignRepo.SearchAssignmentsByTrainer(trainerId,inputText);

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
                                                        moduleId = x.ModuleID,
                                                        moduleName = x.Module.ModuleName,
                                                        classId = x.ClassID,
                                                        className = x.Class.ClassName,
                                                        trainerId = x.TrainerID,
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

        //multiple parameter
        [HttpGet("{moduleId}/{classId}/{trainerId}")]
        public async Task<IActionResult> GetAssignment(int moduleId, int classId, string trainerId)
        {
            try
            {
                var result = await _assignRepo.GetAssignment(moduleId, classId, trainerId);

                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{inputText}")]
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

                return Ok(new { success = true, message = "Add assignment success!" });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new Assignment record");
            }
        }


        //Edit assignment
        [HttpPut("update/{oldTrainerId}")]
        public IActionResult UpdateAssignment([FromBody] AssignmentDto assignmentDto, string oldTrainerId)
        {
            try
            {
                var deleted = _context.Assignments.Where(x => x.ClassID == assignmentDto.ClassID && x.ModuleID == assignmentDto.ModuleID && x.TrainerID == oldTrainerId).FirstOrDefault();

                _context.Assignments.Remove(deleted);
                _context.SaveChanges();

                //get trainer change

                var newTrainer = _context.Trainers.FirstOrDefault(c => c.TrainerID == assignmentDto.TrainerID);

                Assignment newAssignment = new Assignment();
                newAssignment.ClassID = assignmentDto.ClassID;
                newAssignment.ModuleID = assignmentDto.ModuleID;
                newAssignment.TrainerID = assignmentDto.TrainerID;
                newAssignment.RegistrationCode = deleted.RegistrationCode;

                _context.Assignments.Add(newAssignment);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Update enrollment success!" });

            }
            catch
            {
                return Ok(new { success = false, message = "Update enrollment fail!" });
            }

        }


        [HttpDelete("{ClassId}/{ModuleId}/{TrainerId}")]
        public IActionResult Delete(int ClassId, int ModuleId, string TrainerId)
        {
            try
            {
                var assignment = _context.Assignments.Where(x => x.ClassID == ClassId && x.ModuleID == ModuleId && x.TrainerID == TrainerId).FirstOrDefault();

                _context.Assignments.Remove(assignment);

                _context.SaveChanges();
                return Ok(new { success = true, message = "Delete Success!" });
            }
            catch (Exception)
            {
                return Ok(new { success = false, message = "Delete fail!" });
            }
        }

    }
}