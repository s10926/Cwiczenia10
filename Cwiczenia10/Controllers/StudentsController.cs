using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cwiczenia10.DTOs.Requests;
using Cwiczenia10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia10.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly s10926Context _db;

        public StudentsController(s10926Context db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult getStudents()
        {
            return Ok(_db.Student.ToList());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(string id, UpdateStudentDtoRequest student)
        {
            if (!_db.Student.Any(s => s.IndexNumber == id))
            {
                return NotFound("Student o podanym indeksie nie istnieje.");
            }
            if (string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName))
            {
                return BadRequest("Co najmniej jedna z przekazanych wartości jest pusta.");
            }

            var student_ = new Student
            {
                IndexNumber = id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = student.BirthDate
            };

            _db.Attach(student_);
            _db.Entry(student_).Property("FirstName").IsModified = true;
            _db.Entry(student_).Property("LastName").IsModified = true;
            _db.Entry(student_).Property("BirthDate").IsModified = true;
            _db.SaveChanges();

            return Ok(student_);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(string id)
        {
            if (!_db.Student.Any(s => s.IndexNumber == id))
            {
                return NotFound("Student nie istnieje.");
            }

            var student = new Student
            {
                IndexNumber = id
            };

            _db.Attach(student);
            _db.Remove(student);
            _db.SaveChanges();

            return Ok();
        }
    }
}
