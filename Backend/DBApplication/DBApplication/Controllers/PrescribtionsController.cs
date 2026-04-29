using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescribtionsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PrescribtionsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all medical prescriptions.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all medications prescribed by doctors.</remarks>
        [HttpGet("all", Name = "GetAllPrescribtions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Prescribtion>>> GetPrescribtions()
        {
            return await _context.Prescribtions.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific prescription record.
        /// </summary>
        /// <remarks>This endpoint uses a composite key to fetch a specific prescription.</remarks>
        [HttpGet("{patientId}/{medCode}/{docId}", Name = "GetPrescribtion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Prescribtion>> GetPrescribtion(string patientId, short medCode, string docId)
        {
            var prescribtion = await _context.Prescribtions.FindAsync(patientId, medCode, docId);
            if (prescribtion == null) return NotFound();
            return prescribtion;
        }

        /// <summary>
        /// Get all prescriptions for a specific patient
        /// </summary>
        /// <remarks>Retrieves the complete prescription history for a single patient.</remarks>
        [HttpGet("patient/{patientId}", Name = "GetPrescribtionsByPatientId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Prescribtion>>> GetPrescribtionsByPatientId(string patientId)
        {
            if (!await _context.Patients.AnyAsync(p => p.PatientId == patientId))
            {
                return NotFound($"Patient with ID '{patientId}' was not found.");
            }

            return await _context.Prescribtions
                .Where(p => p.PatientId == patientId)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new prescription record.
        /// </summary>
        /// <remarks>Documents a doctor prescribing a specific medication to a specific patient.</remarks>
        [HttpPost("add", Name = "CreatePrescribtion")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Prescribtion>> PostPrescribtion(PrescribtionDto dto)
        {
            // PROACTIVE CHECKS
            if (!await _context.Patients.AnyAsync(p => p.PatientId == dto.PatientId))
                return BadRequest($"Patient with ID '{dto.PatientId}' does not exist.");

            if (!await _context.Medications.AnyAsync(m => m.MedCode == dto.MedCode))
                return BadRequest($"Medication with Code '{dto.MedCode}' does not exist.");

            if (!await _context.Doctors.AnyAsync(d => d.DoctorId == dto.DocId))
                return BadRequest($"Doctor with ID '{dto.DocId}' does not exist.");

            var prescribtion = new Prescribtion
            {
                PatientId = dto.PatientId,
                MedCode = dto.MedCode,
                DocId = dto.DocId,
                PrescribtionDate = dto.PrescribtionDate
            };

            _context.Prescribtions.Add(prescribtion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.Prescribtions.Any(e => e.PatientId == dto.PatientId && e.MedCode == dto.MedCode && e.DocId == dto.DocId)) return Conflict();
                else throw;
            }
            return CreatedAtAction(nameof(GetPrescribtion), new { patientId = prescribtion.PatientId, medCode = prescribtion.MedCode, docId = prescribtion.DocId }, prescribtion);
        }

        /// <summary>
        /// Updates an existing prescription record.
        /// </summary>
        /// <remarks>Modifies properties of a specific prescription.</remarks>
        [HttpPut("update/{patientId}/{medCode}/{docId}", Name = "UpdatePrescribtion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPrescribtion(string patientId, short medCode, string docId, PrescribtionDto dto)
        {
            var prescribtion = await _context.Prescribtions.FindAsync(patientId, medCode, docId);
            if (prescribtion == null) return NotFound();

            prescribtion.PrescribtionDate = dto.PrescribtionDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific prescription record.
        /// </summary>
        /// <remarks>Permanently removes the specified prescription link from the database.</remarks>
        [HttpDelete("delete/{patientId}/{medCode}/{docId}", Name = "DeletePrescribtion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePrescribtion(string patientId, short medCode, string docId)
        {
            var prescribtion = await _context.Prescribtions.FindAsync(patientId, medCode, docId);
            if (prescribtion == null) return NotFound();
            _context.Prescribtions.Remove(prescribtion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}