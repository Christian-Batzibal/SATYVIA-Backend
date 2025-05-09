using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelReservationAPI.Data;
using HotelReservationAPI.Models;

namespace HotelReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Select(r => new
                {
                    r.Id,
                    r.ClientId,
                    r.RoomId,
                    r.StartDate,
                    r.EndDate,
                    r.Status,
                    r.FullName,
                    r.Email,
                    r.Phone,
                    r.Comment,
                    ImagePath = $"/images/rooms/{r.Room.BranchId}Room{((r.Room.Id - 1) % 6 + 1)}/1.jpg"
                })
                .ToListAsync();

            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest();

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // ❗️Evitar conflicto con propiedad de navegación
                reservation.Room = null;

                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == reservation.RoomId);
                if (room == null)
                    return BadRequest("La habitación especificada no existe.");

                // Imagen basada en lógica de carpetas
                int folderRoom = ((room.Id - 1) % 6) + 1;
                reservation.ImagePath = $"/images/rooms/{room.BranchId}Room{folderRoom}/1.jpg";

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
