using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using site_manuais.Data;
using site_manuais.Models;

namespace site_manuais.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Documentos
        public async Task<IActionResult> Index()
        {
            var documentos = await _context.Documentos
                .Include(d => d.Modulo)
                .ThenInclude(m => m.Categoria)
                .OrderByDescending(d => d.DataUpload)
                .ToListAsync();
            
            return View(documentos);
        }

        // GET: Admin/Documentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos
                .Include(d => d.Modulo)
                .ThenInclude(m => m.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // GET: Admin/Documentos/Create
        public IActionResult Create()
        {
            // Carregar módulos com suas categorias para exibir no dropdown
            var modulos = _context.Modulos
                .Include(m => m.Categoria)
                .Select(m => new
                {
                    m.Id,
                    NomeCompleto = m.Categoria.Nome + " → " + m.Nome
                })
                .ToList();

            ViewBag.ModuloId = new SelectList(modulos, "Id", "NomeCompleto");
            return View();
        }

        // POST: Admin/Documentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Descricao,ModuloId")] Documento documento, IFormFile arquivo)
        {
            // Remove validação das propriedades de navegação
            ModelState.Remove("Modulo");
            ModelState.Remove("CaminhoArquivo");
            ModelState.Remove("NomeArquivoOriginal");

            // Validar arquivo
            if (arquivo == null || arquivo.Length == 0)
            {
                ModelState.AddModelError("arquivo", "Selecione um arquivo PDF");
            }
            else if (!arquivo.ContentType.Equals("application/pdf") && !arquivo.FileName.ToLower().EndsWith(".pdf"))
            {
                ModelState.AddModelError("arquivo", "Apenas arquivos PDF são permitidos");
            }
            else if (arquivo.Length > 10 * 1024 * 1024) // 10MB
            {
                ModelState.AddModelError("arquivo", "O arquivo deve ter no máximo 10MB");
            }

            if (ModelState.IsValid && arquivo != null)
            {
                try
                {
                    // Gerar nome único para o arquivo
                    var extensao = Path.GetExtension(arquivo.FileName);
                    var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                    
                    // Caminho completo onde salvar
                    var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "documentos");
                    var arquivoPath = Path.Combine(uploadsPath, nomeArquivo);

                    // Criar diretório se não existir
                    Directory.CreateDirectory(uploadsPath);

                    // Salvar arquivo
                    using (var stream = new FileStream(arquivoPath, FileMode.Create))
                    {
                        await arquivo.CopyToAsync(stream);
                    }

                    // Preencher dados do documento
                    documento.NomeArquivoOriginal = arquivo.FileName;
                    documento.CaminhoArquivo = $"/uploads/documentos/{nomeArquivo}";
                    documento.TamanhoArquivo = arquivo.Length;
                    documento.DataUpload = DateTime.Now;

                    _context.Add(documento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao salvar arquivo: {ex.Message}");
                }
            }

            // Recarregar dropdown em caso de erro
            var modulos = _context.Modulos
                .Include(m => m.Categoria)
                .Select(m => new
                {
                    m.Id,
                    NomeCompleto = m.Categoria.Nome + " → " + m.Nome
                })
                .ToList();

            ViewBag.ModuloId = new SelectList(modulos, "Id", "NomeCompleto", documento.ModuloId);
            return View(documento);
        }

        // GET: Admin/Documentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos.FindAsync(id);
            if (documento == null)
            {
                return NotFound();
            }

            var modulos = _context.Modulos
                .Include(m => m.Categoria)
                .Select(m => new
                {
                    m.Id,
                    NomeCompleto = m.Categoria.Nome + " → " + m.Nome
                })
                .ToList();

            ViewBag.ModuloId = new SelectList(modulos, "Id", "NomeCompleto", documento.ModuloId);
            return View(documento);
        }

        // POST: Admin/Documentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,ModuloId,CaminhoArquivo,NomeArquivoOriginal,TamanhoArquivo,DataUpload")] Documento documento, IFormFile? arquivo)
        {
            if (id != documento.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Modulo");

            if (ModelState.IsValid)
            {
                try
                {
                    // Se um novo arquivo foi enviado
                    if (arquivo != null && arquivo.Length > 0)
                    {
                        // Validar novo arquivo
                        if (!arquivo.ContentType.Equals("application/pdf") && !arquivo.FileName.ToLower().EndsWith(".pdf"))
                        {
                            ModelState.AddModelError("arquivo", "Apenas arquivos PDF são permitidos");
                            
                            var modulosErro = _context.Modulos
                                .Include(m => m.Categoria)
                                .Select(m => new
                                {
                                    m.Id,
                                    NomeCompleto = m.Categoria.Nome + " → " + m.Nome
                                })
                                .ToList();

                            ViewBag.ModuloId = new SelectList(modulosErro, "Id", "NomeCompleto", documento.ModuloId);
                            return View(documento);
                        }

                        // Deletar arquivo antigo
                        var caminhoAntigo = Path.Combine(_webHostEnvironment.WebRootPath, documento.CaminhoArquivo.TrimStart('/'));
                        if (System.IO.File.Exists(caminhoAntigo))
                        {
                            System.IO.File.Delete(caminhoAntigo);
                        }

                        // Salvar novo arquivo
                        var extensao = Path.GetExtension(arquivo.FileName);
                        var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "documentos");
                        var arquivoPath = Path.Combine(uploadsPath, nomeArquivo);

                        using (var stream = new FileStream(arquivoPath, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(stream);
                        }

                        documento.NomeArquivoOriginal = arquivo.FileName;
                        documento.CaminhoArquivo = $"/uploads/documentos/{nomeArquivo}";
                        documento.TamanhoArquivo = arquivo.Length;
                    }

                    documento.DataUltimaAlteracao = DateTime.Now;
                    _context.Update(documento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentoExists(documento.Id))
                    {
                        return NotFound();      
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var modulos = _context.Modulos
                .Include(m => m.Categoria)
                .Select(m => new
                {
                    m.Id,
                    NomeCompleto = m.Categoria.Nome + " → " + m.Nome
                })
                .ToList();

            ViewBag.ModuloId = new SelectList(modulos, "Id", "NomeCompleto", documento.ModuloId);
            return View(documento);
        }

        // GET: Admin/Documentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos
                .Include(d => d.Modulo)
                .ThenInclude(m => m.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // POST: Admin/Documentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documento = await _context.Documentos.FindAsync(id);
            if (documento != null)
            {
                // Deletar arquivo físico
                var caminhoArquivo = Path.Combine(_webHostEnvironment.WebRootPath, documento.CaminhoArquivo.TrimStart('/'));
                if (System.IO.File.Exists(caminhoArquivo))
                {
                    System.IO.File.Delete(caminhoArquivo);
                }

                _context.Documentos.Remove(documento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Download do arquivo
        public async Task<IActionResult> Download(int id)
        {
            var documento = await _context.Documentos.FindAsync(id);
            
            if (documento == null)
            {
                return NotFound();
            }

            var caminhoArquivo = Path.Combine(_webHostEnvironment.WebRootPath, documento.CaminhoArquivo.TrimStart('/'));
            
            if (!System.IO.File.Exists(caminhoArquivo))
            {
                return NotFound("Arquivo não encontrado no servidor");
            }

            var arquivoBytes = await System.IO.File.ReadAllBytesAsync(caminhoArquivo);
            return File(arquivoBytes, "application/pdf", documento.NomeArquivoOriginal);
        }

        private bool DocumentoExists(int id)
        {
            return _context.Documentos.Any(e => e.Id == id);
        }
    }
}


