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

        // GET: api/Reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReservations()
        {
            var reservations = await _context.Reservation
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomImages)
                .Where(r => r.StartDate >= DateTime.Now || r.StartDate == null)
                .OrderBy(r => r.StartDate ?? DateTime.MaxValue)
                .Select(r => new
                {
                    r.Id,
                    r.RoomId,
                    StartDate = r.StartDate.HasValue
                        ? r.StartDate.Value.ToString("dd-MM-yyyy")
                        : "No definida",
                    EndDate = r.EndDate.HasValue
                        ? r.EndDate.Value.ToString("dd-MM-yyyy")
                        : "No definida",
                    r.Status,
                    r.FullName,
                    r.Email,
                    r.Phone,
                    Room = new
                    {
                        r.Room.Id,
                        r.Room.Name,
                        r.Room.Capacity,
                        r.Room.Price,
                        r.Room.BranchId,
                        r.Room.RoomNumber,
                        CoverImage = r.Room.RoomImages
                            .Select(img => img.ImageBase64)
                            .FirstOrDefault(),
                        Images = r.Room.RoomImages
                            .Select(img => img.ImageBase64)
                            .ToList()
                    }
                })
                .ToListAsync();

            return Ok(reservations);
        }

        // GET: api/Reservation/MyReservations
        [HttpGet("MyReservations")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyReservations()
        {
            var reservations = await _context.Reservation
                .AsNoTracking()
                .Include(r => r.Room)
                    .ThenInclude(room => room.Branch)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new
                {
                    r.Id,
                    r.RoomId,
                    r.StartDate,
                    r.EndDate,
                    r.Status,
                    r.FullName,
                    r.Email,
                    r.Phone,
                    Room = new
                    {
                        r.Room.Id,
                        r.Room.Name,
                        BranchName = r.Room.Branch.Name,
                        CoverImage = _context.RoomImage
                            .Where(img => img.RoomId == r.Room.Id)
                            .Select(img => img.ImageBase64)
                            .FirstOrDefault()
                    }
                })
                .ToListAsync();

            return Ok(reservations);
        }


        // GET: api/Reservation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReservation(int id)
        {
            var reservation = await _context.Reservation
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomImages)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return Ok(new
            {
                reservation.Id,
                reservation.RoomId,
                StartDate = reservation.StartDate.HasValue
                    ? reservation.StartDate.Value.ToString("yyyy-MM-dd")
                    : "No definida",
                EndDate = reservation.EndDate.HasValue
                    ? reservation.EndDate.Value.ToString("yyyy-MM-dd")
                    : "No definida",
                reservation.Status,
                reservation.FullName,
                reservation.Email,
                reservation.Phone,
                Room = new
                {
                    reservation.Room.Id,
                    reservation.Room.Name,
                    reservation.Room.Capacity,
                    reservation.Room.Price,
                    reservation.Room.BranchId,
                    reservation.Room.RoomNumber,
                    CoverImage = reservation.Room.RoomImages
                        .Select(img => img.ImageBase64)
                        .FirstOrDefault(),
                    Images = reservation.Room.RoomImages
                        .Select(img => img.ImageBase64)
                        .ToList()
                }
            });
        }

        // POST: api/Reservation
        [HttpPost]
        public async Task<ActionResult<object>> PostReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _context.Room
                .Include(r => r.RoomImages)
                .FirstOrDefaultAsync(r => r.Id == reservation.RoomId);

            if (room == null)
                return BadRequest("La habitación especificada no existe.");

            reservation.Room = null;
            reservation.Status = "Activa";//No existe

            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, new
            {
                reservation.Id,
                reservation.RoomId,
                StartDate = reservation.StartDate?.ToString("dd-MM-yyyy") ?? "No definida",
                EndDate = reservation.EndDate?.ToString("dd-MM-yyyy") ?? "No definida",
                reservation.Status,
                reservation.FullName,
                reservation.Email,
                reservation.Phone,
                Room = new
                {
                    room.Id,
                    room.Name,
                    room.Capacity,
                    room.Price,
                    room.BranchId,
                    room.RoomNumber,
                    CoverImage = room.RoomImages
                        .Select(img => img.ImageBase64)
                        .FirstOrDefault()
                }
            });
        }

        // PUT: api/Reservation/Cancel/{id}
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var result = await (
                from r in _context.Reservation
                join room in _context.Room on r.RoomId equals room.Id
                join branch in _context.Branch on room.BranchId equals branch.Id
                where r.Id == id
                select new
                {
                    Entity = r,
                    Info = new
                    {
                        r.Id,
                        r.Status,
                        RoomName = room.Name,
                        BranchName = branch.Name,
                    }
                }
            ).FirstAsync();

            result.Entity.Status = "Canceled";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Reserva {result.Info.Id} cancelada",
                result.Info.RoomName,
                result.Info.BranchName,
                ReservationStatus = "Canceled"
            });
        }

    }
}
