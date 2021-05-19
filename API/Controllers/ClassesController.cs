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
    public class ClassesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public ClassesController(IUnitOfWork unitOfWork, StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var classes = _unitOfWork.Class.GetAll(includeProperties: "Assignments,Answers,Enrollments");
            //var classes = _unitOfWork.Class.GetAll();
            return Ok(classes);
        }

        [HttpGet("{id}")]
        public ActionResult<Class> GetClass(int id)
        {
            var @class = _unitOfWork.Class.GetFirstOrDefault(x => x.ClassID == id, includeProperties: "Assignments,Answers,Enrollments");
            //var @class = _unitOfWork.Class.Get(id);
            if (@class == null)
                return NotFound();
            return Ok(@class);
        }

        //Add new class
        [HttpPost]
        public ActionResult<Class> CreateClass(Class @class)
        {
            try
            {
                if (@class == null)
                    return BadRequest();

                _unitOfWork.Class.Add(@class);
                _unitOfWork.Save();

                return CreatedAtAction(nameof(CreateClass), new { id = @class.ClassID }, @class);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateClass(int id, Class @class)
        {
            if (id != @class.ClassID)
                return BadRequest();

            // var existingClass = _unitOfWork.Class.GetFirstOrDefault(x => x.ClassID == id, includeProperties: "Assignments,Answers,Enrollments");
            var existingClass = _context.Classes.AsNoTracking().FirstOrDefault(x => x.ClassID == id);
            if (existingClass is null)
                return NotFound();

            _unitOfWork.Class.Update(@class);
            _unitOfWork.Save();

            return NoContent();
        }

        //Delete a class
         [HttpDelete("{id:int}")]
    public ActionResult<Class> DeleteClass(int id)
    {
        try
        {
           var classToDelete = _context.Classes.AsNoTracking().FirstOrDefault(x => x.ClassID == id);

            if (classToDelete == null)
            {
                return NotFound($"Class with Id = {id} not found");
            }

            _unitOfWork.Class.Remove(classToDelete);
            _unitOfWork.Save();
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error deleting data");
        }
    }

    }
}