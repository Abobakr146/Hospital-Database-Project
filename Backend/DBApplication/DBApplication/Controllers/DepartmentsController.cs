using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DepartmentsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all hospital departments.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all clinical and administrative departments in the facility.</remarks>
        [HttpGet("all", Name = "GetAllDepartments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific department by its unique identifier.
        /// </summary>
        /// <remarks>This endpoint fetches the detailed record of a single department using its auto-generated database ID.</remarks>
        [HttpGet("{id}", Name = "GetDepartmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Department>> GetDepartment(short id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return department;
        }

        /// <summary>
        /// Creates a new hospital department.
        /// </summary>
        /// <remarks>Adds a new department to the system.</remarks>
        [HttpPost("create", Name = "CreateDepartment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Department>> PostDepartment(DepartmentDto dto)
        {
            // PROACTIVE CHECK (Only check if they actually provided a manager ID)
            if (!string.IsNullOrEmpty(dto.Manager_DocID))
            {
                if (!await _context.Doctors.AnyAsync(d => d.DoctorId == dto.Manager_DocID))
                    return BadRequest($"Cannot set manager. Doctor with ID '{dto.Manager_DocID}' does not exist.");
            }

            var department = new Department
            {
                DeptName = dto.DeptName,
                Location = dto.Location,
                ManagerDocId = dto.Manager_DocID
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDepartment), new { id = department.DeptId }, department);
        }

        /// <summary>
        /// Updates the details of an existing department.
        /// </summary>
        /// <remarks>Modifies properties of a specific department.</remarks>
        [HttpPut("update/{id}", Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDepartment(short id, DepartmentDto dto)
        {
            // PROACTIVE CHECK
            if (!string.IsNullOrEmpty(dto.Manager_DocID))
            {
                if (!await _context.Doctors.AnyAsync(d => d.DoctorId == dto.Manager_DocID))
                    return BadRequest($"Cannot set manager. Doctor with ID '{dto.Manager_DocID}' does not exist.");
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            department.DeptName = dto.DeptName;
            department.Location = dto.Location;
            department.ManagerDocId = dto.Manager_DocID;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Update department manager
        /// </summary>
        /// <remarks>Assigns a new doctor as the manager for a specific department.</remarks>
        [HttpPatch("{id}/manager", Name = "UpdateDepartmentManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDepartmentManager(short id, [FromBody] string newManagerDocId)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound("Department not found.");

            var doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == newManagerDocId);
            if (!doctorExists) return BadRequest("Cannot assign manager. The specified Doctor ID does not exist.");

            department.ManagerDocId = newManagerDocId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a department record.
        /// </summary>
        /// <remarks>Permanently removes the specified department from the hospital database.</remarks>
        [HttpDelete("delete/{id}", Name = "DeleteDepartment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDepartment(short id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}