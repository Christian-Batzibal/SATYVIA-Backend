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

        private string GetImagePath(int roomId)
        {
            int branch = (roomId - 1) / 6 + 1;
            int room = ((roomId - 1) % 6) + 1;
            return $"/images/rooms/{branch}Room{room}/1.jpg";
        }

        // ✅ GET: api/Reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReservations()
        {
            var reservations = await _context.Reservation
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomServices)
                        .ThenInclude(rs => rs.Service)
                .Where(r => r.StartDate >= DateTime.Now || r.StartDate == null)
                .ToListAsync();

            var result = reservations
                .OrderBy(r => r.StartDate ?? DateTime.MaxValue)
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
                    ImagePath = GetImagePath(r.Room.Id),
                    Room = new
                    {
                        r.Room.Id,
                        r.Room.Name,
                        r.Room.Capacity,
                        r.Room.Price,
                        r.Room.ImagePath,
                        Services = r.Room.RoomServices
                            .Select(rs => new
                            {
                                rs.Service.Id,
                                rs.Service.Name,
                                rs.Service.Description
                            }).ToList()
                    }
                });

            return Ok(result);
        }

        // ✅ GET: api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReservation(int id)
        {
            var reservation = await _context.Reservation
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomServices)
                        .ThenInclude(rs => rs.Service)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return new
            {
                reservation.Id,
                reservation.ClientId,
                reservation.RoomId,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status,
                reservation.FullName,
                reservation.Email,
                reservation.Phone,
                ImagePath = GetImagePath(reservation.Room.Id),
                Room = new
                {
                    reservation.Room.Id,
                    reservation.Room.Name,
                    reservation.Room.Capacity,
                    reservation.Room.Price,
                    reservation.Room.ImagePath,
                    Services = reservation.Room.RoomServices
                        .Select(rs => new
                        {
                            rs.Service.Id,
                            rs.Service.Name,
                            rs.Service.Description
                        }).ToList()
                }
            };
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
        public async Task<ActionResult<object>> PostReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                reservation.Room = null;

                var room = await _context.Room
                    .Include(r => r.RoomServices)
                        .ThenInclude(rs => rs.Service)
                    .FirstOrDefaultAsync(r => r.Id == reservation.RoomId);

                if (room == null)
                    return BadRequest("La habitación especificada no existe.");

                _context.Reservation.Add(reservation);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetReservation", new { id = reservation.Id }, new
                {
                    reservation.Id,
                    reservation.ClientId,
                    reservation.RoomId,
                    reservation.StartDate,
                    reservation.EndDate,
                    reservation.Status,
                    reservation.FullName,
                    reservation.Email,
                    reservation.Phone,
                    ImagePath = GetImagePath(room.Id),
                    Room = new
                    {
                        room.Id,
                        room.Name,
                        room.Capacity,
                        room.Price,
                        room.ImagePath,
                        Services = room.RoomServices
                            .Select(rs => new
                            {
                                rs.Service.Id,
                                rs.Service.Name,
                                rs.Service.Description
                            }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
