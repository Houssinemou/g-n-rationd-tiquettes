using Microsoft.AspNetCore.Mvc;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using Microsoft.EntityFrameworkCore;

namespace générationdétiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamilleController : ControllerBase
    {
        private readonly BarcodeDbContext _context;

        public FamilleController(BarcodeDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/famille
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Famille>>> GetAll()
        {
            return await _context.Familles.ToListAsync();
        }

        // 🔹 GET: api/famille/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Famille>> Get(int id)
        {
            var famille = await _context.Familles.FindAsync(id);
            if (famille == null)
                return NotFound();

            return Ok(famille);
        }

        // 🔹 POST: api/famille
        [HttpPost]
        public async Task<ActionResult<Famille>> Create(Famille famille)
        {
            _context.Familles.Add(famille);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = famille.Id }, famille);
        }

        // 🔹 PUT: api/famille/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Famille famille)
        {
            if (id != famille.Id)
                return BadRequest();

            _context.Entry(famille).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔹 DELETE: api/famille/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var famille = await _context.Familles.FindAsync(id);
            if (famille == null)
                return NotFound();

            _context.Familles.Remove(famille);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
