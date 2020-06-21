using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cwiczenia10.DTOs.Requests;
using Cwiczenia10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenia10.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly s10926Context _db;

        public EnrollmentsController(s10926Context db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentDtoRequest student)
        {
            if (string.IsNullOrEmpty(student.IndexNumber) || string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || string.IsNullOrEmpty(student.Studies))
            {
                return BadRequest("Co najmniej jedna z przekazanych wartości jest pusta.");
            }
            if (_db.Student.Any(s => s.IndexNumber == student.IndexNumber))
            {
                return BadRequest("Student o podanym indeksie już istnieje w bazie danych.");
            }
            if (!_db.Studies.Any(s => s.Name == student.Studies))
            {
                return NotFound("Podane studia nie istnieją.");
            }

            var study = _db.Studies.Where(s => s.Name == student.Studies).FirstOrDefault();
            int idStudy = study.IdStudy;
            int idEnrollment = _db.Enrollment.Select(e => e.IdEnrollment).Max();

            Enrollment e = new Enrollment
            {
                IdEnrollment = idEnrollment + 1,
                Semester = 1,
                IdStudy = idStudy,
                StartDate = new DateTime()
            };

            Student s = new Student
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = student.BirthDate,
                IdEnrollmentNavigation = e
            };

            _db.Add(e);
            _db.Add(s);

            _db.SaveChanges();

            return Ok(s);
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsDtoRequest studies)
        {
            var enrollments = from study in _db.Studies
                              join enrollment in _db.Enrollment
                              on study.IdStudy equals enrollment.IdStudy
                              select new { Name = study.Name, Semester = enrollment.Semester };

            if (!enrollments.Any(e => e.Name == studies.Studies && e.Semester == studies.Semester))
            {
                return NotFound("Semestr studiów nie istnieje.");
            }

            var res = _db.Enrollment.FromSqlRaw<Enrollment>("EXEC PromoteStudents {0}, {1}", studies.Studies, studies.Semester);

            return Ok(res);
        }
    }
}
