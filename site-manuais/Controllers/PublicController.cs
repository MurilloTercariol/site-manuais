using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using site_manuais.Data;
using site_manuais.Models;

namespace site_manuais.Controllers
{
    [AllowAnonymous]
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PublicController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //GET: /Public/Index
        // Mostra todas as Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Modulos)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return View(categorias);
        }

        //GET: /Public/Categoria/5


        // Mostra Modulos de uma categoria

        public async Task<IActionResult> Categoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Documentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        //GET: /Public/Modulo/5
        // Mostra documentos de um modulo especifico
        public async Task<IActionResult> Modulo(int id)
        {
            var modulo = await _context.Modulos
                .Include(m => m.Categoria)
                .Include(m => m.Documentos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modulo == null)
            {
                return NotFound();
            }

            return View(modulo);
        }

        //GET: /Public/Documento/5
        //Mostra detalhes de um documento
        public async Task<IActionResult> Documento(int id)
        {
            var documento = await _context.Documentos
                .Include(d => d.Modulo)
                .ThenInclude(m => m.Categoria)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);

        }

        //GET /Public/Buscar?termo=cadastro
        //Busca

        public async Task<IActionResult> Buscar(string termo)
        {
            ViewData["Title"] = "Resultados da Busca";
            if (string.IsNullOrWhiteSpace(termo))
            {
                //se tiver em brano mostra tudo
                return RedirectToAction(nameof(Index));
            }

            //busca documento [titulo e descricao]
            var documentos = await _context.Documentos
                .Include(d => d.Modulo)
                .ThenInclude(m => m.Categoria)
                .Where(d =>
                    EF.Functions.Like(d.Titulo, $"%{termo}%") ||
                    (d.Descricao != null && EF.Functions.Like(d.Descricao, $"%{termo}%"))
                )
                .OrderBy(d => d.Titulo)
                .ToListAsync();

            ViewBag.TermoBusca = termo;
            ViewBag.TotalResultados = documentos.Count;

            return View(documentos);
        }

        // GET: /Public/Download/
        // Faz download do PDF
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
                return NotFound("Arquivo não encontrado");
            }

            //var arquivoBytes = await System.IO.File.ReadAllBytesAsync(caminhoArquivo);
            //return File(arquivoBytes, "application/pdf", documento.NomeArquivoOriginal);

            var arquivoBytes = await System.IO.File.ReadAllBytesAsync(caminhoArquivo);

            // Força o download com o nome original do arquivo
            return File(arquivoBytes, "application/pdf", documento.NomeArquivoOriginal);
        }




    }
}
