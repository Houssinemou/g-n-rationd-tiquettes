// Controllers/LocalisationController.cs
using Microsoft.AspNetCore.Mvc;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using Microsoft.EntityFrameworkCore;

namespace générationdétiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalisationController : ControllerBase
    {
        private readonly BarcodeDbContext _context;

        public LocalisationController(BarcodeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Localisation>>> GetAll()
        {
            return await _context.Localisations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Localisation>> Get(int id)
        {
            var localisation = await _context.Localisations.FindAsync(id);
            if (localisation == null)
                return NotFound();
            return localisation;
        }

        [HttpPost]
        public async Task<ActionResult<Localisation>> Create(Localisation loc)
        {
            _context.Localisations.Add(loc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = loc.Id }, loc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Localisation loc)
        {
            if (id != loc.Id)
                return BadRequest();

            _context.Entry(loc).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var loc = await _context.Localisations.FindAsync(id);
            if (loc == null)
                return NotFound();

            _context.Localisations.Remove(loc);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
