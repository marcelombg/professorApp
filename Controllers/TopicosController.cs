using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Data;
using ProfessorApp.Api.Models;

namespace ProfessorApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicosController : ControllerBase
    {
        private readonly ProfessorAppContext _context;

        public TopicosController(ProfessorAppContext context)
        {
            _context = context;
        }

        // GET: api/topicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topico>>> GetTopicos()
        {
            return await _context.Topicos.ToListAsync();
        }

        // GET: api/topicos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Topico>> GetTopico(int id)
        {
            var topico = await _context.Topicos.FindAsync(id);

            if (topico == null)
            {
                return NotFound();
            }

            return topico;
        }

        // POST: api/topicos
        [HttpPost]
        public async Task<ActionResult<Topico>> PostTopico(Topico topico)
        {
            try
            {
                _context.Topicos.Add(topico);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTopico), new { id = topico.Id }, topico);
            }
            catch (DbUpdateException)
            {
                // Verifica se já existe um tópico com o mesmo nome
                if (_context.Topicos.Any(t => t.Nome == topico.Nome))
                {
                    return Conflict("Já existe um tópico com este nome.");
                }
                throw;
            }
        }

        // PUT: api/topicos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopico(int id, Topico topico)
        {
            if (id != topico.Id)
            {
                return BadRequest();
            }

            _context.Entry(topico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                // Verifica se já existe um tópico com o mesmo nome
                if (_context.Topicos.Any(t => t.Nome == topico.Nome && t.Id != id))
                {
                    return Conflict("Já existe um tópico com este nome.");
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/topicos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopico(int id)
        {
            var topico = await _context.Topicos.FindAsync(id);
            if (topico == null)
            {
                return NotFound();
            }

            _context.Topicos.Remove(topico);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TopicoExists(int id)
        {
            return _context.Topicos.Any(e => e.Id == id);
        }
    }
}

