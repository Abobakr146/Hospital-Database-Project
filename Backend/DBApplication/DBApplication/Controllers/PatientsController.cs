using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PatientsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all registered patients.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all patient demographics.</remarks>
        [HttpGet("all", Name = "GetAllPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific patient by their National ID.
        /// </summary>
        /// <remarks>This endpoint fetches the detailed record of a single patient.</remarks>
        [HttpGet("{id}", Name = "GetPatientById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> GetPatient(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return patient;
        }

        /// <summary>
        /// Registers a new patient in the system.
        /// </summary>
        /// <remarks>Adds a new patient to the database using their explicitly provided ID.</remarks>
        [HttpPost("create", Name = "CreatePatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Patient>> PostPatient(PatientDto dto)
        {
            var patient = new Patient
            {
                PatientId = dto.PatientId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Dob = dto.DOB
            };

            _context.Patients.Add(patient);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.Patients.Any(e => e.PatientId == dto.PatientId)) return Conflict();
                else throw;
            }
            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        /// <summary>
        /// Updates the details of an existing patient.
        /// </summary>
        /// <remarks>Modifies properties of a specific patient.</remarks>
        [HttpPut("update/{id}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPatient(string id, PatientDto dto)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Dob = dto.DOB;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a patient record.
        /// </summary>
        /// <remarks>Permanently removes the specified patient from the hospital database.</remarks>
        [HttpDelete("delete/{id}", Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}