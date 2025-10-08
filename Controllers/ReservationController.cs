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
                    .ThenInclude(r => r.RoomServices)
                        .ThenInclude(rs => rs.Service)
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomImages)
                .Where(r => r.StartDate >= DateTime.Now || r.StartDate == null)
                .OrderBy(r => r.StartDate ?? DateTime.MaxValue)
                .Select(r => new
                {
                    r.Id,
                    r.ClientId,
                    r.RoomId,
                    StartDate = r.StartDate.HasValue ? r.StartDate.Value.ToString("yyyy-MM-dd") : "No definida",
                    EndDate = r.EndDate.HasValue ? r.EndDate.Value.ToString("yyyy-MM-dd") : "No definida",
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
                        CoverImage = r.Room.RoomImages.Select(img => img.ImageBase64).FirstOrDefault(),
                        Images = r.Room.RoomImages.Select(img => img.ImageBase64).ToList(),
                        Services = r.Room.RoomServices.Select(rs => new
                        {
                            rs.Service.Id,
                            rs.Service.Name,
                            rs.Service.Description
                        }).ToList()
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
                    .ThenInclude(room => room.RoomImages)
                .Include(r => r.Room)
                    .ThenInclude(room => room.Branch)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new
                {
                    Id = r.Id,
                    RoomId = r.RoomId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    Status = r.Status,
                    FullName = r.FullName,
                    Email = r.Email,
                    Phone = r.Phone,
                    Room = new
                    {
                        Id = r.Room.Id,
                        Name = r.Room.Name,
                        BranchName = r.Room.Branch.Name,
                        CoverImage = r.Room.RoomImages.Select(i => i.ImageBase64).FirstOrDefault()
                    }
                })
                .ToListAsync();

            return Ok(reservations);
        }

        // GET: api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReservation(int id)
        {
            var reservation = await _context.Reservation
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomServices)
                        .ThenInclude(rs => rs.Service)
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomImages)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return new
            {
                reservation.Id,
                reservation.ClientId,
                reservation.RoomId,
                StartDate = reservation.StartDate.HasValue ? reservation.StartDate.Value.ToString("yyyy-MM-dd") : "No definida",
                EndDate = reservation.EndDate.HasValue ? reservation.EndDate.Value.ToString("yyyy-MM-dd") : "No definida",
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
                    CoverImage = reservation.Room.RoomImages.Select(img => img.ImageBase64).FirstOrDefault(),
                    Images = reservation.Room.RoomImages.Select(img => img.ImageBase64).ToList(),
                    Services = reservation.Room.RoomServices.Select(rs => new
                    {
                        rs.Service.Id,
                        rs.Service.Name,
                        rs.Service.Description
                    }).ToList()
                }
            };
        }

        // PUT: api/Reservation/5
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

        // POST: api/Reservation
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
                    .Include(r => r.RoomImages)
                    .FirstOrDefaultAsync(r => r.Id == reservation.RoomId);

                if (room == null)
                    return BadRequest("La habitación especificada no existe.");

                // Estado inicial por defecto: Activa
                reservation.Status ??= "Activa";

                _context.Reservation.Add(reservation);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetReservation", new { id = reservation.Id }, new
                {
                    reservation.Id,
                    reservation.ClientId,
                    reservation.RoomId,
                    StartDate = reservation.StartDate.HasValue ? reservation.StartDate.Value.ToString("yyyy-MM-dd") : "No definida",
                    EndDate = reservation.EndDate.HasValue ? reservation.EndDate.Value.ToString("yyyy-MM-dd") : "No definida",
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
                        CoverImage = room.RoomImages.Select(img => img.ImageBase64).FirstOrDefault(),
                        Images = room.RoomImages.Select(img => img.ImageBase64).ToList(),
                        Services = room.RoomServices.Select(rs => new
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

        // PUT: api/Reservation/ChangeStatus/5
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] string newStatus)
        {
            var reservation = await _context.Reservation
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound("Reserva no encontrada.");

            // Solo se permiten los estados definidos
            if (newStatus != "Activa" && newStatus != "Cancelada")
                return BadRequest("Estado inválido. Solo se permite 'Activa' o 'Cancelada'.");

            reservation.Status = newStatus;

            // Sincronizar el estado de la habitación
            if (reservation.Room != null)
            {
                if (newStatus == "Activa")
                    reservation.Room.Status = "Ocupada";
                else if (newStatus == "Cancelada")
                    reservation.Room.Status = "Disponible";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Estado de la reserva {id} actualizado a '{newStatus}'.",
                ReservationId = reservation.Id,
                RoomStatus = reservation.Room?.Status
            });
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
