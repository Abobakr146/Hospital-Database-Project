using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBApplication.Models;
using DBApplication.DTOs;

namespace DBApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public AppointmentsController(HospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all scheduled appointments in the hospital system.
        /// </summary>
        /// <remarks>This endpoint fetches a complete list of all appointments, regardless of their status.</remarks>
        [HttpGet("all", Name = "GetAllAppointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific appointment by its unique identifier.
        /// </summary>
        /// <remarks>This endpoint fetches the detailed record of a single appointment using its auto-generated database ID.</remarks>
        [HttpGet("{id}", Name = "GetAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Appointment>> GetAppointment(short id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            return appointment;
        }

        /// <summary>
        /// Schedules a new medical appointment.
        /// </summary>
        /// <remarks>Creates a new appointment record linking a patient to a doctor at a specific date and time.</remarks>
        [HttpPost("schedule", Name = "CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Appointment>> PostAppointment(AppointmentDto dto)
        {
            var appointment = new Appointment
            {
                ApptDate = dto.ApptDate,
                ApptTime = dto.ApptTime,
                Status = dto.Status,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.ApptId }, appointment);
        }

        /// <summary>
        /// Updates the details of an existing appointment.
        /// </summary>
        /// <remarks>Modifies properties of a specific appointment.</remarks>
        [HttpPut("update/{id}", Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAppointment(short id, AppointmentDto dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.ApptDate = dto.ApptDate;
            appointment.ApptTime = dto.ApptTime;
            appointment.Status = dto.Status;
            appointment.PatientId = dto.PatientId;
            appointment.DoctorId = dto.DoctorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Cancels and deletes an appointment record.
        /// </summary>
        /// <remarks>Permanently removes the specified appointment from the hospital database.</remarks>
        [HttpDelete("cancel/{id}", Name = "DeleteAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAppointment(short id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}