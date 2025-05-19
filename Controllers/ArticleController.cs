using Microsoft.AspNetCore.Mvc;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using Microsoft.EntityFrameworkCore;

namespace générationdétiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly BarcodeDbContext _context;

        public ArticleController(BarcodeDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/article
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }

        // 🔹 GET: api/article/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
                return NotFound();

            return article;
        }

        // 🔹 GET: api/article/familles
        [HttpGet("familles")]
        public async Task<ActionResult<IEnumerable<string>>> GetFamilles()
        {
            var familles = await _context.Articles
                .Where(a => !string.IsNullOrEmpty(a.Famille))
                .Select(a => a.Famille)
                .Distinct()
                .ToListAsync();

            return familles;
        }

        // 🔹 POST: api/article
        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(article.CodeArticle) && !string.IsNullOrWhiteSpace(article.Famille))
            {
                var prefix = GeneratePrefix(article.Famille); // ✅ utilise la méthode ici
                var sequence = await _context.CodeSequences.FirstOrDefaultAsync(s => s.Prefix == prefix);
                if (sequence == null)
                {
                    sequence = new CodeSequence { Prefix = prefix, LastNumber = 0 };
                    _context.CodeSequences.Add(sequence);
                }

                sequence.LastNumber++;
                await _context.SaveChangesAsync();

                article.CodeArticle = $"{prefix}-{sequence.LastNumber:D6}";
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        // 🔹 PUT: api/article/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] Article article)
        {
            if (id != article.Id)
                return BadRequest();

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // 🔹 DELETE: api/article/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔍 Vérifie l'existence d'un article
        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

        // ✅ Générateur de préfixes standardisés
        private string GeneratePrefix(string? famille)
        {
            if (string.IsNullOrWhiteSpace(famille))
                return "DEF";

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Imprimantes", "IMP" },
                { "Ordinateurs", "ORD" },
                { "Scanners", "SCA" },
                { "Vidéoprojecteurs", "VID" },
                { "Téléphones", "TEL" },
                { "Serveurs", "SRV" },
                { "Écrans", "ECR" },
                { "Bureau", "BUR" },
                { "Chaises", "CHA" },
                { "Chaise", "CHA" },
                { "Armoires", "ARM" },
                { "Tables", "TAB" },
                { "Routeurs", "ROU" },
                { "Switchs", "SWT" },
                { "Accessoires", "ACC" },
                { "Câbles", "CAB" },
                { "Logiciels", "LOG" },
                { "Onduleurs", "OND" }
            };

            if (dict.TryGetValue(famille.Trim(), out var prefix))
                return prefix;

            return new string(famille
                .Where(char.IsLetter)
                .Take(3)
                .Select(char.ToUpper)
                .ToArray());
        }
    }
}
