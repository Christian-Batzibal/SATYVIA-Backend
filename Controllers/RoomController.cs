using HotelReservationAPI.Data;
using HotelReservationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<object>> GetRooms(int page = 1, int pageSize = 6, int? branchId = null)
        {
            var query = _context.Room
                .AsNoTracking()
                .Include(r => r.RoomImages)
                .Include(r => r.Branch)
                .AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(r => r.BranchId == branchId.Value);
            }

            var total = await query.CountAsync();

            var rooms = await query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    capacity = r.Capacity,
                    price = r.Price,
                    branchId = r.BranchId,
                    branchName = r.Branch.Name,
                    roomNumber = r.RoomNumber,
                    coverImage = r.RoomImages
                                    .Select(i => i.ImageBase64)
                                    .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = rooms
            });
        }


        // GET: api/Room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRoom(int id)
        {
            var room = await _context.Room
                .Include(r => r.RoomImages)
                .Include(r => r.Branch)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            return Ok(new
            {
                id = room.Id,
                name = room.Name,
                capacity = room.Capacity,
                price = room.Price,
                branchId = room.BranchId,
                branchName = room.Branch.Name,
                roomNumber = room.RoomNumber,
                images = room.RoomImages.Select(i => i.ImageBase64).ToList(),
                coverImage = room.RoomImages.Select(i => i.ImageBase64).FirstOrDefault()
            });
        }


        // POST: api/Room
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Room.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        // GET: api/Room/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableRooms(
            int branchId, DateTime startDate, DateTime endDate, int guests)
        {
            var rooms = await _context.Room
                .Include(r => r.RoomImages)
                .Include(r => r.Branch)
                .Where(r => r.BranchId == branchId && r.Capacity >= guests)
                .ToListAsync();

            var reservedRoomIds = await _context.Reservation
                .Where(res =>
                    res.Room.BranchId == branchId &&
                    (
                        (startDate >= res.StartDate && startDate < res.EndDate) ||
                        (endDate > res.StartDate && endDate <= res.EndDate) ||
                        (startDate <= res.StartDate && endDate >= res.EndDate)
                    )
                )
                .Select(res => res.RoomId)
                .ToListAsync();

            var availableRooms = rooms
                .Where(r => !reservedRoomIds.Contains(r.Id))
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Capacity,
                    r.Price,
                    r.BranchId,
                    BranchName = r.Branch.Name,
                    r.RoomNumber,
                    Images = r.RoomImages.Select(i => i.ImageBase64).ToList(),
                    CoverImage = r.RoomImages.Select(i => i.ImageBase64).FirstOrDefault()
                })
                .ToList();

            return Ok(availableRooms);
        }

        // PUT: api/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

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
