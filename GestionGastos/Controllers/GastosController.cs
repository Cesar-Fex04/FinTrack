using GestionGastos.Models;
using GestionGastos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionGastos.Controllers
{
    public class GastosController : Controller
    {
        private readonly IRepositorioGastos _repositorioGastos;

        public GastosController(IRepositorioGastos repositorioGastos)
        {
            _repositorioGastos = repositorioGastos;
        }

        // GET: Gastos/Index
        public async Task<IActionResult> Index()
        {
            int idUsuarioPrueba = 1;
            var gastos = await _repositorioGastos.ObtenerTodos(idUsuarioPrueba);
            return View(gastos);
        }

        // GET: Gastos/Crear
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new GastoCreacionViewModel
            {
                fechaGasto = DateTime.Now,
                Categorias = await ObtenerCategoriasSelect()
            };
            return View(modelo);
        }

        // POST: Gastos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(GastoCreacionViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.Categorias = await ObtenerCategoriasSelect();
                return View(modelo);
            }

            int idUsuarioPrueba = 1;

            var gasto = new Gasto
            {
                id_usuario = idUsuarioPrueba,
                id_categoria = modelo.id_categoria,
                id_tipoMovimiento = 1, // SIEMPRE 1 para GASTO (hardcodeado)
                Descripcion = modelo.Descripcion,
                Monto = modelo.Monto,
                MetodoPago = modelo.MetodoPago,
                fechaGasto = modelo.fechaGasto
            };

            try
            {
                await _repositorioGastos.Crear(gasto);
                TempData["Mensaje"] = "Gasto registrado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al guardar: {ex.Message}");
                modelo.Categorias = await ObtenerCategoriasSelect();
                return View(modelo);
            }
        }

        // GET: Gastos/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var gasto = await _repositorioGastos.ObtenerPorId(id);

            if (gasto == null)
            {
                return NotFound();
            }

            var modelo = new GastoCreacionViewModel
            {
                Descripcion = gasto.Descripcion,
                Monto = gasto.Monto,
                id_categoria = gasto.id_categoria,
                MetodoPago = gasto.MetodoPago,
                fechaGasto = gasto.fechaGasto,
                Categorias = await ObtenerCategoriasSelect()
            };

            ViewBag.IdGasto = id;
            return View(modelo);
        }

        // POST: Gastos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, GastoCreacionViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.Categorias = await ObtenerCategoriasSelect();
                ViewBag.IdGasto = id;
                return View(modelo);
            }

            var gasto = new Gasto
            {
                id_gasto = id,
                id_categoria = modelo.id_categoria,
                Descripcion = modelo.Descripcion,
                Monto = modelo.Monto,
                MetodoPago = modelo.MetodoPago,
                fechaGasto = modelo.fechaGasto
            };

            try
            {
                await _repositorioGastos.Actualizar(gasto);
                TempData["Mensaje"] = "Gasto actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el gasto: {ex.Message}");
                modelo.Categorias = await ObtenerCategoriasSelect();
                ViewBag.IdGasto = id;
                return View(modelo);
            }
        }

        // GET: Gastos/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var gasto = await _repositorioGastos.ObtenerPorId(id);

            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }

        // POST: Gastos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _repositorioGastos.Eliminar(id);
                TempData["Mensaje"] = "Gasto eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el gasto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // --- Método privado ---
        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasSelect()
        {
            var categorias = await _repositorioGastos.ObtenerCategoriasGasto();
            return categorias.Select(c => new SelectListItem(c.nombre, c.id_categoria.ToString()));
        }
    }
}