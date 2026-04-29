using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public MedicationsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all available medications.
        /// </summary>
        /// <remarks>This endpoint fetches a complete catalog of all medications managed by the hospital pharmacy.</remarks>
        [HttpGet("all", Name = "GetAllMedications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Medication>>> GetMedications()
        {
            return await _context.Medications.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific medication by its unique code.
        /// </summary>
        /// <remarks>This endpoint fetches the detailed record of a single medication using its auto-generated database ID.</remarks>
        [HttpGet("{id}", Name = "GetMedicationById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Medication>> GetMedication(short id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return NotFound();
            return medication;
        }

        /// <summary>
        /// Adds a new medication to the pharmacy catalog.
        /// </summary>
        /// <remarks>Creates a new medication record.</remarks>
        [HttpPost("add", Name = "CreateMedication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Medication>> PostMedication(MedicationDto dto)
        {
            var medication = new Medication
            {
                MedName = dto.MedName,
                Dosage = dto.Dosage,
                Unit = dto.Unit
            };

            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMedication), new { id = medication.MedCode }, medication);
        }

        /// <summary>
        /// Updates the details of an existing medication.
        /// </summary>
        /// <remarks>Modifies properties of a specific medication.</remarks>
        [HttpPut("update/{id}", Name = "UpdateMedication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMedication(short id, MedicationDto dto)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return NotFound();

            medication.MedName = dto.MedName;
            medication.Dosage = dto.Dosage;
            medication.Unit = dto.Unit;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Removes a medication from the catalog.
        /// </summary>
        /// <remarks>Permanently deletes the specified medication from the hospital database.</remarks>
        [HttpDelete("delete/{id}", Name = "DeleteMedication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedication(short id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return NotFound();
            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}