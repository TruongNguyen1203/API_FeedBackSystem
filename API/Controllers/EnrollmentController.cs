using System;
using System.Threading.Tasks;
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
    [Authorize(Roles = Role.Admin)]
    [ApiController]

    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepo;

        public EnrollmentController(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepo = enrollmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetEnrollments()
        {
            try
            {
                return Ok(await _enrollmentRepo.GetEnrollments());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Enrollment>> UpdateEnrollment(int classId, int traineeId, Enrollment enrollment)
        {
            try
            {
                if (classId != enrollment.ClassID && traineeId != enrollment.TraineeID)
                    return BadRequest("ClassID and TraineeID mismatch");

                var enrollmentToUpdate = await _enrollmentRepo.GetEnrollment(classId, traineeId);

                if (enrollmentToUpdate == null)
                    return NotFound($"Enrollment not found");

                return await _enrollmentRepo.UpdateEnrollment(enrollment);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete]
        public async Task<ActionResult<Enrollment>> DeleteEnrollment(int classId, int traineeId)
        {
            try
            {
                var enrollmentToDelete = await _enrollmentRepo.GetEnrollment(classId,traineeId);

                if (enrollmentToDelete == null)
                {
                    return NotFound($"Enrollment not found");
                }

               return await _enrollmentRepo.DeleteEnrollment(classId,traineeId);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }





    }
}