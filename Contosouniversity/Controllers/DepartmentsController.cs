using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcWithEf.Models;

namespace MvcWithEf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        #region 請用 Raw SQL Query 的方式查詢 vwDepartmentCourseCount 檢視表的內容
        /// <summary>
        ///  api/Departments/GetCourses?DepartmentId=1
        /// </summary>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        [HttpGet("GetCourses")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetCourses(int DepartmentId)
        {
            return await _context.VwDepartmentCourseCount
                .FromSqlInterpolated($"select * from VwDepartmentCourseCount where DepartmentID = {DepartmentId}")
                .ToListAsync();
        }
        
        #endregion

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department
                .Where(d=>!d.IsDeleted)
                .ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department
                .Where(d=>d.DepartmentId == id && !d.IsDeleted).FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            //_context.Entry(department).State = EntityState.Modified;

            try
            {
                _context.Department
                .FromSqlInterpolated(
                $@"select * from Department where DepartmentId = {id};
                    EXECUTE dbo.Department_Update 
                    {id},
                    {department.Name},
                    {department.Budget},
                    {department.StartDate},
                    {department.InstructorId},
                    {department.RowVersion} ;"
                ).AsEnumerable().FirstOrDefault();

                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // POST: api/Departments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            Department d =
            _context.Department
                .FromSqlInterpolated(
                    $@"select * from Department where DepartmentId = @@identity;
                        EXECUTE dbo.Department_Insert 
                            @Name = {department.Name},
                            @Budget = {department.Budget},
                            @StartDate = {department.StartDate},
                            @InstructorId = {department.InstructorId} ;
                        "
                ).AsEnumerable().FirstOrDefault();

            return d;
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            
            if (!DepartmentExists(id))
            {
               return NotFound();
            }

            var department = await _context.Department.FindAsync(id);

            _context.Department
                .FromSqlInterpolated(
                    $@"select * from Department where DepartmentId = {id};
                       EXECUTE dbo.Department_Delete @DepartmentID = {id}, 
                        @RowVersion_Original = {department.RowVersion}; "
                 ).AsEnumerable().Any();

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department
                .Any(e => e.DepartmentId == id && !e.IsDeleted);
        }
    }
}
