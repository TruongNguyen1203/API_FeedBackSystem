using System;
using System.Collections.Generic;
using System.Linq;
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
    [ApiController]

    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassRepository _classtRepo;

        public ClassesController(IClassRepository classRepository)
        {
            _classtRepo = classRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetClasses()
        {
            try
            {
                return Ok(await _classtRepo.GetClasses());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        // [HttpGet("{id}")]
        // public ActionResult<Class> GetClass(int id)
        // {
        //     var @class = _unitOfWork.Class.GetFirstOrDefault(x => x.ClassID == id, includeProperties: "Assignments,Answers,Enrollments");
        //     if (@class == null)
        //         return NotFound();
        //     return Ok(@class);
        // }

        //Add new class
        //[Authorize(Roles = Role.Admin)]
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

         [HttpGet("{id:int}")]
        public async Task<ActionResult<Class>> GetClass(int id)
        {
            try
            {
                var result = await _classtRepo.GetClassById(id);

                if (result == null) return NotFound();

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //[Authorize(Roles = Role.Admin)]
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

        //Delete a class
        //[Authorize(Roles = Role.Admin)]
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