using System;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly StoreContext _context;

        public EnrollmentController(IEnrollmentRepository enrollmentRepository,StoreContext context)
        {
            _enrollmentRepo = enrollmentRepository;
              _context = context;
        }

        [HttpGet("{className}")]
        public async Task<ActionResult> GetEnrollments(string className)
        {
            try
            {
                return Ok(await _enrollmentRepo.GetEnrollments(className));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //multiple parameter
        [HttpGet("getDetail/{classId}/{traineeId}")]
        public async Task<IActionResult> GetDetailEnrollment(int classId, string traineeId)
        {
            try
            {
                var result =  _context.Enrollments.Where(x => x.ClassID == classId && x.TraineeID == traineeId)
                                                    .Select(x => new DetailEnrollmentDto(){
                                                            TraineeID = x.TraineeID,
                                                            Phone = x.Trainee.AppUser.PhoneNumber,
                                                            TraineeName = x.Trainee.AppUser.UserName,
                                                            Address = "Thu Duc",
                                                            Email = x.Trainee.AppUser.Email,
                                                            CLassID = x.ClassID,
                                                            StartTime = x.Class.StartTime,
                                                            ClassName = x.Class.ClassName,
                                                            EndTime = x.Class.EndTime,
                                                            Capacity = x.Class.Capacity

                                                    })
                                                    .FirstOrDefault();


                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }


       [HttpPut("update/{oldClassId}/{oldTraineeId}")]
        public IActionResult Update([FromBody]EnrollmentDto enrollmentDto, int oldClassId, string oldTraineeId)
        {
            var deleted = _context.Enrollments.Where(x => x.ClassID == oldClassId && x.TraineeID == oldTraineeId).FirstOrDefault();

            _context.Enrollments.Remove(deleted);
            _context.SaveChanges();

            //get class change
            Class newClass = _context.Classes.FirstOrDefault(c => c.ClassName == enrollmentDto.ClassName);
                                    
            Enrollment newEnrollment = new Enrollment();
            newEnrollment.ClassID = newClass.ClassID;
            newEnrollment.TraineeID = enrollmentDto.TraineeID;

             _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();
            return Ok(new {success=true, message="Update enrollment success!"});

      
        }

        [HttpDelete]
        public async Task<ActionResult<Enrollment>> DeleteEnrollment(int classId, string traineeId)
        {
            try
            {
                var enrollmentToDelete = await _enrollmentRepo.GetEnrollment(classId, traineeId);

                if (enrollmentToDelete == null)
                {
                    return NotFound($"Enrollment not found");
                }

                await _enrollmentRepo.DeleteEnrollment(classId, traineeId);
                 return Ok(new {success=true, message="Delete enrollment success!"}); 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }





    }
}