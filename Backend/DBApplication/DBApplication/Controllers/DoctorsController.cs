using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DoctorsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all registered doctors.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all medical professionals currently employed at the hospital.</remarks>
        [HttpGet("all", Name = "GetAllDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            return await _context.Doctors.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific doctor by their National ID or specific identifier.
        /// </summary>
        /// <remarks>This endpoint fetches the detailed record of a single doctor using their explicit string identifier.</remarks>
        [HttpGet("{id}", Name = "GetDoctorById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Doctor>> GetDoctor(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            return doctor;
        }

        /// <summary>
        /// Get all doctors in a specific department
        /// </summary>
        /// <remarks>Retrieves a list of all doctors assigned to a given department ID.</remarks>
        [HttpGet("department/{deptId}", Name = "GetDoctorsByDepartmentId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsByDepartment(short deptId)
        {
            if (!await _context.Departments.AnyAsync(d => d.DeptId == deptId))
            {
                return NotFound($"Department with ID '{deptId}' was not found.");
            }

            return await _context.Doctors
                .Where(d => d.DeptId == deptId)
                .ToListAsync();
        }

        /// <summary>
        /// Registers a new doctor in the system.
        /// </summary>
        /// <remarks>Adds a new doctor to the database using their explicit ID.</remarks>
        [HttpPost("add", Name = "CreateDoctor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorDto dto)
        {
            // PROACTIVE CHECK
            if (!await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId))
                return BadRequest($"Department with ID '{dto.DeptId}' does not exist.");

            var doctor = new Doctor
            {
                DoctorId = dto.DoctorId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Specialty = dto.Specialty,
                DeptId = dto.DeptId
            };

            _context.Doctors.Add(doctor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.Doctors.Any(e => e.DoctorId == dto.DoctorId)) return Conflict();
                else throw;
            }
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctor);
        }

        /// <summary>
        /// Updates the details of an existing doctor.
        /// </summary>
        /// <remarks>Modifies properties of a specific doctor.</remarks>
        [HttpPut("update/{id}", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDoctor(string id, DoctorDto dto)
        {
            // PROACTIVE CHECK
            if (!await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId))
                return BadRequest($"Department with ID '{dto.DeptId}' does not exist.");

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Specialty = dto.Specialty;
            doctor.DeptId = dto.DeptId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a doctor record.
        /// </summary>
        /// <remarks>Permanently removes the specified doctor from the hospital database.</remarks>
        [HttpDelete("delete/{id}", Name = "DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}