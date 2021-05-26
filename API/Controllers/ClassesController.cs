using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using API.ViewModel;
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
    public class ClassesController : ControllerBase
    {
        private readonly IClassRepository _classtRepo;
        private readonly StoreContext _context;

        public ClassesController(IClassRepository classRepository, StoreContext context)
        {
            _classtRepo = classRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetClasses()
        {
            try
            {
                var role = HttpContext.Session.GetString(SessionKey.Role);
                var userId = HttpContext.Session.GetString(SessionKey.Id);
                return Ok(new {userId=userId});
                switch (role)
                {
                    case Role.Admin:
                        return Ok(await _classtRepo.GetClasses());
                    case Role.Trainer:
                        var assignments = _context.Assignments.Include(a => a.Class)
                                                                .ThenInclude(a => a.Enrollments)
                                                                .Where(a => a.TrainerID == userId)
                                                                .ToList();

                        List<ClassListByTrainerDTO> classList = new List<ClassListByTrainerDTO>();

                        foreach (var a in assignments)
                        {
                            ClassListByTrainerDTO classTemp = new ClassListByTrainerDTO();
                            classTemp.ClassID = a.ClassID;
                            classTemp.ClassName = a.Class.ClassName;
                            classTemp.NumberOfTrainee = a.Class.Enrollments.Count();
                            classList.Add(classTemp);
                        }
                        return Ok(classList);
                    
                    case Role.Trainee:
                        var enrollments = _context.Enrollments.Include(a => a.Class)
                                                                .Where(a => a.TraineeID == userId)
                                                                .ToList();
                                                                
                        List<ClassListByTrainee> classListByTrainees = new List<ClassListByTrainee>();

                        foreach(var e in enrollments)
                        {
                            ClassListByTrainee temp = new ClassListByTrainee();
                            temp.ClassID = e.ClassID;
                            temp.ClassName = e.Class.ClassName;
                            temp.NumberOfTrainee = e.Class.Enrollments.Count();
                            classListByTrainees.Add(temp);
                        }
                        return Ok(classListByTrainees);
                    default:
                        return Unauthorized();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetClass(int id)
        {
            try
            {
                var role = HttpContext.Session.GetString(SessionKey.Role);
                var userId = HttpContext.Session.GetString(SessionKey.Id);
                var result = await _classtRepo.GetClassById(id);

                if (result == null) return NotFound();
                switch (role)
                {
                    case Role.Admin:
                        return Ok(result);

                    case Role.Trainer: case Role.Trainee:
                        var @class = await _context.Classes.Include(c => c.Enrollments)
                                                            .ThenInclude(c => c.Trainee)
                                                            .ThenInclude(c => c.AppUser)
                                                            .FirstOrDefaultAsync(c => c.ClassID == id);

                        TraineeListVM trainees = new TraineeListVM();
                        trainees.ClassId = result.ClassID;
                        trainees.ClassName = result.ClassName;
                        List<TraineeVM> listTrainees = new List<TraineeVM>();

                        int count = 1;

                        foreach (var e in @class.Enrollments)
                        {
                            TraineeVM tempTrainee = new TraineeVM();
                            tempTrainee.Number = count++;
                            tempTrainee.TraineeID = e.Trainee.TraineeID;
                            tempTrainee.TraineeName = e.Trainee.AppUser.UserName;
                            listTrainees.Add(tempTrainee);

                        }
                        trainees.TraineeList = listTrainees;
                        return Ok(trainees);

                    default:
                        return Unauthorized();

                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }


        [HttpPost]
        public async Task<ActionResult<Class>> CreateClass(Class @class)
        {
            try
            {
                if (@class == null)
                    return BadRequest();

                var createdClass = await _classtRepo.AddClass(@class);

                return CreatedAtAction(nameof(GetClass),
                    new { id = createdClass.ClassID }, createdClass);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Class>> UpdateClass(int id, Class @class)
        {
            try
            {
                if (id != @class.ClassID)
                    return BadRequest("ClassID mismatch");

                var classToUpdate = await _classtRepo.GetClassById(id);

                if (classToUpdate == null)
                    return NotFound($"Class not found");

                return await _classtRepo.UpdateClass(@class);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Class>> DeleteClass(int id)
        {
            try
            {
                var classToDelete = await _classtRepo.GetClassById(id);

                if (classToDelete == null)
                {
                    return NotFound($"Class not found");
                }

                return await _classtRepo.DeleteClass(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }


    }
}