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
        public async Task<ActionResult> GetClasses(string role, string userId)
        {
            try
            {
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
                            classTemp.NumberOfTrainee = _context.Enrollments.Where(x => x.ClassID == a.ClassID).Count();
                            classList.Add(classTemp);
                        }
                        return Ok(classList);

                    case Role.Trainee:
                        var enrollments = _context.Enrollments.Include(a => a.Class)
                                                                .Where(a => a.TraineeID == userId)
                                                                .ToList();

                        List<ClassListByTrainee> classListByTrainees = new List<ClassListByTrainee>();

                        foreach (var e in enrollments)
                        {
                            ClassListByTrainee temp = new ClassListByTrainee();
                            temp.ClassID = e.ClassID;
                            temp.ClassName = e.Class.ClassName;
                            temp.NumberOfTrainee = _context.Enrollments.Where(x => x.ClassID == e.ClassID).Count();
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

        [HttpGet("detail")]
        public async Task<ActionResult> GetClass(int id, string role, string userId)
        {
            try
            {
                var result = await _classtRepo.GetClassById(id);

                if (result == null) return NotFound();
                switch (role)
                {
                    // du
                    case Role.Admin:
                        return Ok(result);

                    case Role.Trainer:
                    case Role.Trainee:
                        var @class = await _context.Classes.Where(x => x.IsDeleted == false).Include(c => c.Enrollments)
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
                    return Ok(new { success = false, message = "Add class false!" });

                var checkClass = _context.Classes.Where(x => x.ClassName == @class.ClassName).FirstOrDefault();
                //neu ton tai ten class do -> ko add
                if (checkClass != null)
                {
                     return Ok(new { success = false, message = "Class name is already existed!" });
                }
                var createdClass = await _classtRepo.AddClass(@class);

                return Ok(new { success = true, message = "Add class success!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Class>> UpdateClass(Class @class)
        {
            try
            {

                var classToUpdate = await _classtRepo.GetClassById(@class.ClassID);

                if (classToUpdate == null)
                    return NotFound($"Class not found");

                var checkClass = _context.Classes.Where(x => x.ClassName == @class.ClassName).Count();
                //neu ton tai ten class do -> ko add
                if (checkClass > 1 )
                {
                     return Ok(new { success = false, message = "Class name is already existed!" });
                }

                await _classtRepo.UpdateClass(@class);
                return Ok(new { success = true, message = "Update class success!" });
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

                await _classtRepo.DeleteClass(id);
                return Ok(new { success = true, message = "Delete class success!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
        [HttpGet("update")]
        public async Task<ActionResult> GetDetail(int id, string role, string userId)
        {
           
            if (role != Role.Admin)
            {
                return Unauthorized();
            }
            var temp = await _context.Classes.Where(x => x.ClassID == id)
                                                    .Select(x => new
                                                    {
                                                        ClassID = x.ClassID,
                                                        ClassName = x.ClassName,
                                                        Capacity = x.Capacity,
                                                        StartDate = x.StartTime,
                                                        EndDate = x.EndTime
                                                    }).FirstOrDefaultAsync();
            return Ok(temp);
        }
        

    }
}
