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
    public class BranchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BranchController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Branch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBranches()
        {
            var branches = await _context.Branch
                .Include(b => b.BranchImages)
                .ToListAsync();

            var result = branches.Select(b => new
            {
                id = b.Id,
                name = b.Name,
                location = b.Location,
                coverImage = b.BranchImages
                    .Select(i => i.ImageBase64)
                    .FirstOrDefault() is string img && !string.IsNullOrEmpty(img)
                        ? (img.StartsWith("data:image")
                            ? img.Trim()
                            : "data:image/jpeg;base64," + img.Trim())
                        : null
            });

            return Ok(result);
        }

        // GET: api/Branch/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBranch(int id)
        {
            var branch = await _context.Branch
                .Include(b => b.BranchImages)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
                return NotFound();

            return Ok(new
            {
                id = branch.Id,
                name = branch.Name,
                location = branch.Location,
                images = branch.BranchImages.Select(i => i.ImageBase64).ToList(),
                coverImage = branch.BranchImages.Select(i => i.ImageBase64).FirstOrDefault()
            });
        }

        // PUT: api/Branch/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, Branch branch)
        {
            if (id != branch.Id)
            {
                return BadRequest();
            }

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
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

        // POST: api/Branch
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            _context.Branch.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branch);
        }

        // DELETE: api/Branch/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branch.Remove(branch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BranchExists(int id)
        {
            return _context.Branch.Any(e => e.Id == id);
        }
    }
}
