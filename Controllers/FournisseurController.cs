using Microsoft.AspNetCore.Mvc;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using Microsoft.EntityFrameworkCore;

namespace générationdétiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FournisseurController : ControllerBase
    {
        private readonly BarcodeDbContext _context;

        public FournisseurController(BarcodeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fournisseur>>> GetAll()
        {
            return await _context.Fournisseurs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Fournisseur>> Get(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null) return NotFound();
            return fournisseur;
        }

        [HttpPost]
        public async Task<ActionResult<Fournisseur>> Create(Fournisseur fournisseur)
        {
            _context.Fournisseurs.Add(fournisseur);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = fournisseur.Id }, fournisseur);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Fournisseur fournisseur)
        {
            if (id != fournisseur.Id) return BadRequest();
            _context.Entry(fournisseur).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null) return NotFound();
            _context.Fournisseurs.Remove(fournisseur);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
