// Controllers/UniteFonctionController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using g�n�rationd�tiquettes.Data;
using g�n�rationd�tiquettes.Models;

namespace g�n�rationd�tiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniteFonctionController : ControllerBase
    {
        private readonly BarcodeDbContext _context;

        public UniteFonctionController(BarcodeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UniteFonction>>> GetAll()
        {
            return await _context.UnitesFonction.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UniteFonction>> Get(int id)
        {
            var unite = await _context.UnitesFonction.FindAsync(id);
            if (unite == null)
                return NotFound();
            return unite;
        }

        [HttpPost]
        public async Task<ActionResult<UniteFonction>> Create(UniteFonction unite)
        {
            _context.UnitesFonction.Add(unite);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = unite.Id }, unite);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UniteFonction unite)
        {
            if (id != unite.Id)
                return BadRequest();

            _context.Entry(unite).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var unite = await _context.UnitesFonction.FindAsync(id);
            if (unite == null)
                return NotFound();

            _context.UnitesFonction.Remove(unite);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
