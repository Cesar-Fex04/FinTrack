using GestionGastos.Models;
using GestionGastos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionGastos.Controllers
{
    public class IngresosController : Controller
    {
        private readonly IRepositorioIngresos _repositorioIngresos;

        public IngresosController(IRepositorioIngresos repositorioIngresos)
        {
            _repositorioIngresos = repositorioIngresos;
        }

        // GET: Ingresos
        public async Task<IActionResult> Index()
        {
            int idUsuarioPrueba = 1;
            var ingresos = await _repositorioIngresos.ObtenerTodos(idUsuarioPrueba);
            return View(ingresos);
        }

        // GET: Ingresos/Crear
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new Ingreso
            {
                fechaIngreso = DateTime.Now,
                Categorias = await ObtenerCategoriasIngreso()
            };

            return View(modelo);
        }

        // POST: Ingresos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Ingreso ingreso)
        {
            if (!ModelState.IsValid)
            {
                ingreso.Categorias = await ObtenerCategoriasIngreso();
                return View(ingreso);
            }

            int idUsuarioPrueba = 1;
            ingreso.id_usuario = idUsuarioPrueba;

            try
            {
                await _repositorioIngresos.Crear(ingreso);
                TempData["Mensaje"] = "Ingreso registrado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar el ingreso: {ex.Message}");
                ingreso.Categorias = await ObtenerCategoriasIngreso();
                return View(ingreso);
            }
        }

        // GET: Ingresos/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var ingreso = await _repositorioIngresos.ObtenerPorId(id);

            if (ingreso == null)
            {
                return NotFound();
            }

            ingreso.Categorias = await ObtenerCategoriasIngreso();
            return View(ingreso);
        }

        // POST: Ingresos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Ingreso ingreso)
        {
            if (!ModelState.IsValid)
            {
                ingreso.Categorias = await ObtenerCategoriasIngreso();
                return View(ingreso);
            }

            try
            {
                await _repositorioIngresos.Actualizar(ingreso);
                TempData["Mensaje"] = "Ingreso actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el ingreso: {ex.Message}");
                ingreso.Categorias = await ObtenerCategoriasIngreso();
                return View(ingreso);
            }
        }

        // GET: Ingresos/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ingreso = await _repositorioIngresos.ObtenerPorId(id);

            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // POST: Ingresos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _repositorioIngresos.Eliminar(id);
                TempData["Mensaje"] = "Ingreso eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el ingreso: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método privado para obtener categorías de ingreso
        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasIngreso()
        {
            var categorias = await _repositorioIngresos.ObtenerCategoriasIngreso();
            return categorias.Select(c => new SelectListItem(c.nombre, c.id_categoria.ToString()));
        }
    }
}