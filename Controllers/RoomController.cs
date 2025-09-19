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

        // ✅ GET: api/Room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRooms()
        {
            var rooms = await _context.Room
                .Include(r => r.Branch)
                .Include(r => r.RoomServices)
                    .ThenInclude(rs => rs.Service)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Capacity,
                    r.Price,
                    r.ImagePath,
                    Branch = r.Branch != null ? r.Branch.Name : null,
                    Services = r.RoomServices.Select(rs => new
                    {
                        rs.Service.Id,
                        rs.Service.Name,
                        rs.Service.Description
                    }).ToList()
                })
                .ToListAsync();

            return Ok(rooms);
        }

        // ✅ GET: api/Room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRoom(int id)
        {
            var room = await _context.Room
                .Include(r => r.Branch)
                .Include(r => r.RoomServices)
                    .ThenInclude(rs => rs.Service)
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Capacity,
                    r.Price,
                    r.ImagePath,
                    Branch = r.Branch != null ? r.Branch.Name : null,
                    Services = r.RoomServices.Select(rs => new
                    {
                        rs.Service.Id,
                        rs.Service.Name,
                        rs.Service.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (room == null) return NotFound();
            return Ok(room);
        }

        // ✅ GET: api/Room/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableRooms(
            [FromQuery] int branchId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int guests)
        {
            if (startDate >= endDate)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            var reservedRoomIds = await _context.Reservation
                .Where(r =>
                    r.StartDate < endDate &&
                    r.EndDate > startDate
                )
                .Select(r => r.RoomId)
                .Distinct()
                .ToListAsync();

            var availableRooms = await _context.Room
                .Include(r => r.Branch)
                .Include(r => r.RoomServices)
                    .ThenInclude(rs => rs.Service)
                .Where(r =>
                    r.BranchId == branchId &&
                    !reservedRoomIds.Contains(r.Id) &&
                    r.Capacity >= guests
                )
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Capacity,
                    r.Price,
                    r.ImagePath,
                    Branch = r.Branch != null ? r.Branch.Name : null,
                    Services = r.RoomServices.Select(rs => new
                    {
                        rs.Service.Id,
                        rs.Service.Name,
                        rs.Service.Description
                    }).ToList()
                })
                .ToListAsync();

            return Ok(availableRooms);
        }

        // POST: api/Room
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Room.Add(room);
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
            var room = await _context.Room.FindAsync(id);
            if (room == null) return NotFound();

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.Id == id);
        }
    }
}
