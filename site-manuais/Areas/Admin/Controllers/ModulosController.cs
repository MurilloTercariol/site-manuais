using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using site_manuais.Data;
using site_manuais.Models;

namespace site_manuais.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ModulosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModulosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Modulos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Modulos.Include(m => m.Categoria);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Modulos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modulo = await _context.Modulos
                .Include(m => m.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modulo == null)
            {
                return NotFound();
            }

            return View(modulo);
        }

        // GET: Admin/Modulos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            return View();
        }

        // POST: Admin/Modulos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,CategoriaId")] Modulo modulo)
        {
            // Remove a validação da propriedade de navegação Categoria
            ModelState.Remove("Categoria");
            
            // LOG: Verificar se está chegando aqui
            Console.WriteLine("=== CREATE POST CHAMADO ===");
            Console.WriteLine($"Nome: {modulo.Nome}");
            Console.WriteLine($"CategoriaId: {modulo.CategoriaId}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            
            // Se houver erros de validação, mostrar
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Erro: {error.ErrorMessage}");
                }
            }
            
            if (ModelState.IsValid)
            {
                // Define a data de criação automaticamente
                modulo.DataCriacao = DateTime.Now;
                
                _context.Add(modulo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", modulo.CategoriaId);
            return View(modulo);
        }

        // GET: Admin/Modulos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", modulo.CategoriaId);
            return View(modulo);
        }

        //Verificar se existe
        private bool ModuloExists(int id)
        {
            return _context.Modulos.Any(e => e.Id == id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,CategoriaId")] Modulo modulo)
        {
            if (id != modulo.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Categoria");

            if (ModelState.IsValid)
            {
                try
                {
                    var moduloOriginal = await _context.Modulos.FindAsync(id);
                    if (moduloOriginal == null)
                        return NotFound();

                    moduloOriginal.Nome = modulo.Nome;
                    moduloOriginal.Descricao = modulo.Descricao;
                    moduloOriginal.CategoriaId = modulo.CategoriaId;
                    moduloOriginal.DataUltimaAlteracao = DateTime.Now;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuloExists(modulo.Id))
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
            
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", modulo.CategoriaId);
            return View(modulo);
        }

        // GET: Admin/Modulos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modulo = await _context.Modulos
                .Include(m => m.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modulo == null)
            {
                return NotFound();
            }

            return View(modulo);
        }

        // POST: Admin/Modulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modulo = await _context.Modulos
                .Include(c => c.Documentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (modulo == null)
            {
                return NotFound();
            }

            if (modulo.Documentos != null && modulo.Documentos.Any())
            {
                ModelState.AddModelError("",
                    $"Não é possível excluir. Esta categoria possui {modulo.Documentos.Count} módulo(s) associado(s).");
                return View("Delete", modulo);
            }

            _context.Modulos.Remove(modulo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
