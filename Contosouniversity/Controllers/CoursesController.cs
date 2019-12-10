using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWithEf.Models;

namespace MvcWithEf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public CoursesController(
            ContosouniversityContext context
            )
        {
            _context = context;
        }

        #region 請在 CoursesController 中設計 vwCourseStudents 的 API 輸出
        /// <summary>
        /// https://localhost:5001/api/Courses/GetStudents?CourseId=5
        /// </summary>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        [HttpGet("GetStudents")]
        public async Task<ActionResult<IEnumerable<VwCourseStudents>>> GetStudents(int CourseId)
        {
            return await _context.VwCourseStudents
                .Where(t=>t.CourseId == CourseId)
                    .ToListAsync();
        }
        #endregion

        #region 請在 CoursesController 中設計 vwCourseStudentCount 檢視表的 API 輸出
        /// <summary>
        /// https://localhost:5001/api/Courses/GetStudentCount?CourseId=5
        /// </summary>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        [HttpGet("GetStudentCount")]
        public async Task<ActionResult<IEnumerable<VwCourseStudentCount>>> GetStudentCount(int CourseId)
        {
            return await _context.VwCourseStudentCount
                .Where(t=>t.CourseId == CourseId)
                    .ToListAsync();
        }
        #endregion

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Course.Where(c=>!c.IsDeleted).ToListAsync();
        }


        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseAsync(int id)
        {
            var course = await (
                from p in _context.Course
                .Include(p => p.Department)
                .Include(p => p.CourseInstructor)
                where p.CourseId == id && !p.IsDeleted
                select new
                {
                    p.CourseId,
                    p.Title,
                    p.Credits,
                    p.DepartmentId,
                    p.Department.Name,
                    Instructors =
                        p.CourseInstructor
                        .Select(t=>t.Instructor)
                        .Select(i=> $"{i.LastName} {i.FirstName}")

                }).SingleAsync();

            if (course == null)
            {
                return NotFound();
            }

            return new JsonResult(course);
        }


        // PUT: api/Courses/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse(int id)
        {
            if (!CourseExists(id))
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
 
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Where(c=>!c.IsDeleted).Any(e => e.CourseId == id);
        }
    }
}
