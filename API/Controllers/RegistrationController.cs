using System;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly StoreContext _context;

        public RegistrationController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RegistrationTrainee([FromBody] Trainee_Assignment trainee_Assignment)
        {
            try
            {
                //kiem tra xem co ton tai trong trainee_assignment chua
                var result = _context.Trainee_Assignments.Where(x => x.TraineeID == trainee_Assignment.TraineeID && x.RegistrationCode == trainee_Assignment.RegistrationCode).FirstOrDefault();
                if (result == null)
                {
                    var assign = _context.Assignments.Where(a => a.RegistrationCode == trainee_Assignment.RegistrationCode).FirstOrDefault();
                    if (assign != null)
                    {
                        //Add traineeId va classId vao Enrollment
                        var enroll = _context.Enrollments.Where(e => e.TraineeID == trainee_Assignment.TraineeID && e.ClassID == assign.ClassID).FirstOrDefault();
                        //neu trainee va class chua ton tai trong enrollment -> add trainee va class do vao enrollment
                        if (enroll == null)
                        {
                            Enrollment enrol = new Enrollment();
                            enrol.ClassID = assign.ClassID;
                            enrol.TraineeID = trainee_Assignment.TraineeID;
                            _context.Enrollments.Add(enrol);

                        }
                        //neu ton tai roi thi add vao trainee_assignemnt thoi, enrollement k can add
                        _context.Trainee_Assignments.Add(trainee_Assignment);
                        _context.SaveChanges();
                        return Ok(new { success = true, message = "Join success!" });
                    }

                }
                else
                {
                    return Ok(new { success = false, message = "User already in this Module!" });
                }
                return Ok(new { success = false, message = "Join false!" });

            }
            catch (Exception)
            {
                return Ok(new { success = false, message = "Join false!" });
            }

        }
    }



}