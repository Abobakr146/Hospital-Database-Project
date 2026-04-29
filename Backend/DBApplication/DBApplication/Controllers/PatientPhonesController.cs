using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientPhonesController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PatientPhonesController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all patient phone numbers.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all contact numbers associated with patients in the system.</remarks>
        [HttpGet("all", Name = "GetAllPatientPhones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientPhone>>> GetPatientPhones()
        {
            return await _context.PatientPhones.ToListAsync();
        }

        /// <summary>
        /// Get all phones for a specific patient
        /// </summary>
        /// <remarks>Retrieves a list of all contact numbers registered to a single patient ID.</remarks>
        /// <summary>
        /// Get all phones for a specific patient
        /// </summary>
        /// <remarks>Retrieves a list of all contact numbers registered to a single patient ID.</remarks>
        [HttpGet("patient/{patientId}", Name = "GetPhonesByPatientId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatientPhone>>> GetPhonesByPatientId(string patientId)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == patientId);
            if (!patientExists)
            {
                return NotFound($"Patient with ID '{patientId}' was not found.");
            }

            var phones = await _context.PatientPhones
                .Where(pp => pp.PatientId == patientId)
                .ToListAsync();
            return phones;
        }

        /// <summary>
        /// Retrieves a specific phone record for a patient.
        /// </summary>
        /// <remarks>This endpoint uses a composite key to fetch a specific phone number linked to a specific patient.</remarks>
        [HttpGet("{patientId}/{phone}", Name = "GetPatientPhone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientPhone>> GetPatientPhone(string patientId, string phone)
        {
            var patientPhone = await _context.PatientPhones.FindAsync(patientId, phone);
            if (patientPhone == null) return NotFound();
            return patientPhone;
        }

        [HttpPost("add", Name = "CreatePatientPhone")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientPhone>> PostPatientPhone(PatientPhoneDto dto)
        {
            // PROACTIVE CHECK: Does the patient even exist?
            if (!await _context.Patients.AnyAsync(p => p.PatientId == dto.PatientId))
            {
                return BadRequest($"Cannot add phone. Patient with ID '{dto.PatientId}' does not exist.");
            }

            var patientPhone = new PatientPhone
            {
                PatientId = dto.PatientId,
                Phone = dto.Phone
            };

            _context.PatientPhones.Add(patientPhone);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.PatientPhones.Any(e => e.PatientId == dto.PatientId && e.Phone == dto.Phone)) return Conflict("This exact phone number is already registered for this patient.");
                else throw;
            }
            return CreatedAtAction(nameof(GetPatientPhone), new { patientId = patientPhone.PatientId, phone = patientPhone.Phone }, patientPhone);
        }

        /// <summary>
        /// Updates an existing patient phone record.
        /// </summary>
        /// <remarks>Since both fields are primary keys, this allows updating by deleting the old and creating the new.</remarks>
        [HttpPut("update/{patientId}/{phone}", Name = "UpdatePatientPhone")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPatientPhone(string patientId, string phone, PatientPhoneDto dto)
        {
            var existingPhone = await _context.PatientPhones.FindAsync(patientId, phone);
            if (existingPhone == null) return NotFound();

            _context.PatientPhones.Remove(existingPhone);

            var newPhone = new PatientPhone
            {
                PatientId = dto.PatientId,
                Phone = dto.Phone
            };

            _context.PatientPhones.Add(newPhone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific phone number record from a patient.
        /// </summary>
        /// <remarks>Permanently removes the specified contact number linked to the patient.</remarks>
        [HttpDelete("delete/{patientId}/{phone}", Name = "DeletePatientPhone")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatientPhone(string patientId, string phone)
        {
            var patientPhone = await _context.PatientPhones.FindAsync(patientId, phone);
            if (patientPhone == null) return NotFound();
            _context.PatientPhones.Remove(patientPhone);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}