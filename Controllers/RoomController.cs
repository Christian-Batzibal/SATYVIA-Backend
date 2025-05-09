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
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        // GET: api/Room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return room;
        }

        // POST: api/Room
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // PUT: api/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id) return BadRequest();
            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

        // ✅ NUEVO ENDPOINT: /api/Room/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(
            [FromQuery] int branchId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int guests)
        {
            if (startDate >= endDate)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            // Buscar reservas que se crucen con las fechas indicadas
            var reservedRoomIds = await _context.Reservations
                .Where(r =>
                    r.StartDate < endDate &&
                    r.EndDate > startDate
                )
                .Select(r => r.RoomId)
                .Distinct()
                .ToListAsync();

            // Buscar habitaciones que:
            // 1. Sean de la sucursal indicada
            // 2. No estén reservadas en las fechas indicadas
            // 3. Tengan capacidad suficiente
            var availableRooms = await _context.Rooms
                .Where(r =>
                    r.BranchId == branchId &&
                    !reservedRoomIds.Contains(r.Id) &&
                    r.Capacity >= guests
                )
                .ToListAsync();

            return availableRooms;
        }
    }
}
