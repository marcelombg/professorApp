using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProfessorApp.Api.Data;
using ProfessorApp.Api.Models;

namespace ProfessorApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AprendizadosController : ControllerBase
    {
        private readonly ProfessorAppContext _context;

        public AprendizadosController(ProfessorAppContext context)
        {
            _context = context;
        }

        // GET: api/aprendizados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAprendizados(
            [FromQuery] int? alunoId = null, 
            [FromQuery] int? topicoId = null)
        {
            var query = _context.Aprendizados
                .Include(a => a.Aluno)
                .Include(a => a.Topicos)
                .AsQueryable();

            // Filtrar por aluno se especificado
            if (alunoId.HasValue)
            {
                query = query.Where(a => a.AlunoId == alunoId.Value);
            }

            // Filtrar por tópico se especificado
            if (topicoId.HasValue)
            {
                query = query.Where(a => a.Topicos.Any(t => t.Id == topicoId.Value));
            }

            var aprendizados = await query
                .OrderByDescending(a => a.DataRegistro)
                .Select(a => new
                {
                    a.Id,
                    a.AlunoId,
                    AlunoNome = a.Aluno.Nome,
                    a.DataRegistro,
                    a.Descricao,
                    Topicos = a.Topicos.Select(t => new { t.Id, t.Nome }).ToList()
                })
                .ToListAsync();

            return Ok(aprendizados);
        }

        // GET: api/aprendizados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAprendizado(int id)
        {
            var aprendizado = await _context.Aprendizados
                .Include(a => a.Aluno)
                .Include(a => a.Topicos)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.AlunoId,
                    AlunoNome = a.Aluno.Nome,
                    a.DataRegistro,
                    a.Descricao,
                    Topicos = a.Topicos.Select(t => new { t.Id, t.Nome }).ToList()
                })
                .FirstOrDefaultAsync();

            if (aprendizado == null)
            {
                return NotFound();
            }

            return aprendizado;
        }

        // POST: api/aprendizados
        [HttpPost]
        public async Task<ActionResult<Aprendizado>> PostAprendizado(AprendizadoDto aprendizadoDto)
        {
            var aprendizado = new Aprendizado
            {
                AlunoId = aprendizadoDto.AlunoId,
                Descricao = aprendizadoDto.Descricao,
                DataRegistro = DateTime.Now
            };

            // Adicionar tópicos se especificados
            if (aprendizadoDto.TopicoIds != null && aprendizadoDto.TopicoIds.Any())
            {
                var topicos = await _context.Topicos
                    .Where(t => aprendizadoDto.TopicoIds.Contains(t.Id))
                    .ToListAsync();
                
                foreach (var topico in topicos)
                {
                    aprendizado.Topicos.Add(topico);
                }
            }

            _context.Aprendizados.Add(aprendizado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAprendizado), new { id = aprendizado.Id }, aprendizado);
        }

        // PUT: api/aprendizados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAprendizado(int id, AprendizadoDto aprendizadoDto)
        {
            var aprendizado = await _context.Aprendizados
                .Include(a => a.Topicos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aprendizado == null)
            {
                return NotFound();
            }

            // Atualizar propriedades básicas
            aprendizado.AlunoId = aprendizadoDto.AlunoId;
            aprendizado.Descricao = aprendizadoDto.Descricao;

            // Limpar tópicos existentes
            aprendizado.Topicos.Clear();

            // Adicionar novos tópicos se especificados
            if (aprendizadoDto.TopicoIds != null && aprendizadoDto.TopicoIds.Any())
            {
                var topicos = await _context.Topicos
                    .Where(t => aprendizadoDto.TopicoIds.Contains(t.Id))
                    .ToListAsync();
                
                foreach (var topico in topicos)
                {
                    aprendizado.Topicos.Add(topico);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AprendizadoExists(id))
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

        // DELETE: api/aprendizados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAprendizado(int id)
        {
            var aprendizado = await _context.Aprendizados.FindAsync(id);
            if (aprendizado == null)
            {
                return NotFound();
            }

            _context.Aprendizados.Remove(aprendizado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/aprendizados/exportar
        [HttpGet("exportar")]
        public async Task<IActionResult> ExportarExcel(
            [FromQuery] int? alunoId = null, 
            [FromQuery] int? topicoId = null)
        {

            ExcelPackage.License.SetNonCommercialOrganization("Marcelo-Teste");

            var query = _context.Aprendizados
                .Include(a => a.Aluno)
                .Include(a => a.Topicos)
                .AsQueryable();

            // Aplicar filtros
            if (alunoId.HasValue)
            {
                query = query.Where(a => a.AlunoId == alunoId.Value);
            }

            if (topicoId.HasValue)
            {
                query = query.Where(a => a.Topicos.Any(t => t.Id == topicoId.Value));
            }

            var aprendizados = await query
                .OrderByDescending(a => a.DataRegistro)
                .ToListAsync();

            // Gerar arquivo Excel
            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Aprendizados");

            // Cabeçalhos
            worksheet.Cells[1, 1].Value = "Aluno";
            worksheet.Cells[1, 2].Value = "Data de Registro";
            worksheet.Cells[1, 3].Value = "Descrição";
            worksheet.Cells[1, 4].Value = "Tópicos";

            // Estilizar cabeçalhos
            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Dados
            for (int i = 0; i < aprendizados.Count; i++)
            {
                var aprendizado = aprendizados[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = aprendizado.Aluno.Nome;
                worksheet.Cells[row, 2].Value = aprendizado.DataRegistro.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 3].Value = aprendizado.Descricao;
                worksheet.Cells[row, 4].Value = string.Join(", ", aprendizado.Topicos.Select(t => t.Nome));
            }

            // Ajustar largura das colunas
            worksheet.Cells.AutoFitColumns();

            // Converter para bytes
            var fileBytes = package.GetAsByteArray();

            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"aprendizados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private bool AprendizadoExists(int id)
        {
            return _context.Aprendizados.Any(e => e.Id == id);
        }
    }

    // DTO para criação e atualização de aprendizados
    public class AprendizadoDto
    {
        public int AlunoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public List<int>? TopicoIds { get; set; }
    }
}

