using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebAppMyper3.Data;
using WebAppMyper3.Models;

namespace WebAppMyper3.Controllers
{
    public class TrabajadoresController : Controller
    {
        private readonly AppDbContext _context;

        public TrabajadoresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sexoFiltro = null)
        {
            try
            {
                // Uso con LinQ
                //var query = _context.Trabajadores
                //.Include(t => t.IdDepartamentoNavigation)
                //.Include(t => t.IdProvinciaNavigation)
                //.Include(t => t.IdDistritoNavigation)
                //.AsQueryable();
                //if (!string.IsNullOrEmpty(sexoFiltro) && (sexoFiltro == "M" || sexoFiltro == "F"))
                //{
                //    query = query.Where(t => t.Sexo == sexoFiltro);
                //}
                //var trabajadores = await query.ToListAsync();

                // Uso con Store Procedure
                var param = new SqlParameter("@SexoFiltro", (object)sexoFiltro ?? DBNull.Value);

                var trabajadores = await _context
                    .Trabajadores
                    .FromSqlRaw("EXEC sp_ListarTrabajadores @SexoFiltro", param)
                    .ToListAsync();

                foreach (var trabajador in trabajadores)
                {
                    if (trabajador.IdDepartamento.HasValue)
                    {
                        trabajador.IdDepartamentoNavigation = await _context.Departamento
                            .FindAsync(trabajador.IdDepartamento);
                    }

                    if (trabajador.IdProvincia.HasValue)
                    {
                        trabajador.IdProvinciaNavigation = await _context.Provincia
                            .FindAsync(trabajador.IdProvincia);
                    }

                    if (trabajador.IdDistrito.HasValue)
                    {
                        trabajador.IdDistritoNavigation = await _context.Distrito
                            .FindAsync(trabajador.IdDistrito);
                    }
                }

                ViewBag.SexoFiltro = sexoFiltro;

                return View(trabajadores);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al cargar los trabajadores. {ex.Message}";
                return View(new List<Trabajadores>());
            }
        }

        // GET: Trabajadores/Create
        public IActionResult Create()
        {
            ViewBag.Departamentos = new SelectList(_context.Departamento, "Id", "NombreDepartamento");
            ViewBag.Provincias = new SelectList(new List<Provincia>(), "Id", "NombreProvincia");
            ViewBag.Distritos = new SelectList(new List<Distrito>(), "Id", "NombreDistrito");

            return View();
        }

        // POST: Trabajadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipoDocumento,NumeroDocumento,Nombres,Sexo,IdDepartamento,IdProvincia,IdDistrito")] Trabajadores trabajadores)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(trabajadores);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Nuevo trabajador creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Departamentos = new SelectList(_context.Departamento, "Id", "NombreDepartamento");
                ViewBag.Provincias = new SelectList(new List<Provincia>(), "Id", "NombreProvincia");
                ViewBag.Distritos = new SelectList(new List<Distrito>(), "Id", "NombreDistrito");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear el trabajador. {ex.Message}";
            }
            return View(trabajadores);
        }

        // GET: Trabajadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajadores = await _context.Trabajadores.FindAsync(id);
            if (trabajadores == null)
            {
                return NotFound();
            }
            ViewBag.Departamentos = new SelectList(_context.Departamento, "Id", "NombreDepartamento");
            ViewBag.Provincias = new SelectList(new List<Provincia>(), "Id", "NombreProvincia");
            ViewBag.Distritos = new SelectList(new List<Distrito>(), "Id", "NombreDistrito");
            return View(trabajadores);
        }

        // POST: Trabajadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipoDocumento,NumeroDocumento,Nombres,Sexo,IdDepartamento,IdProvincia,IdDistrito")] Trabajadores trabajadores)
        {
            try
            {
                if (id != trabajadores.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(trabajadores);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Trabajador actualizado correctamente";
                    }
                    catch (Exception ex)
                    {
                        if (!TrabajadoresExists(trabajadores.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Error al editar el trabajador. {ex.Message}";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al editar el trabajador. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDepartamento"] = new SelectList(_context.Departamento, "Id", "Id", trabajadores.IdDepartamento);
            ViewData["IdDistrito"] = new SelectList(_context.Distrito, "Id", "Id", trabajadores.IdDistrito);
            ViewData["IdProvincia"] = new SelectList(_context.Provincia, "Id", "Id", trabajadores.IdProvincia);
            return View(trabajadores);
        }

        // GET: Trabajadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajadores = await _context.Trabajadores
                .Include(t => t.IdDepartamentoNavigation)
                .Include(t => t.IdDistritoNavigation)
                .Include(t => t.IdProvinciaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajadores == null)
            {
                return NotFound();
            }

            return View(trabajadores);
        }

        // POST: Trabajadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var trabajador = await _context.Trabajadores.FindAsync(id);
                if (trabajador == null)
                {
                    TempData["ErrorMessage"] = "Trabajador no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                _context.Trabajadores.Remove(trabajador);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Trabajador eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el trabajador. {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TrabajadoresExists(int id)
        {
            return _context.Trabajadores.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<IActionResult> GetProvinciasByDepartamento(int departamentoId)
        {
            try
            {
                var provincias = await _context.Provincia
                .Where(p => p.IdDepartamento == departamentoId)
                .Select(p => new { id = p.Id, nombreProvincia = p.NombreProvincia })
                .ToListAsync();
                return Json(provincias);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al cargar provincias. {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDistritosByProvincia(int provinciaId)
        {
            try
            {
                var distritos = await _context.Distrito
                .Where(d => d.IdProvincia == provinciaId)
                .Select(d => new { id = d.Id, nombreDistrito = d.NombreDistrito })
                .ToListAsync();
                return Json(distritos);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al cargar distritos. {ex.Message}" });
            }
        }
    }
}
