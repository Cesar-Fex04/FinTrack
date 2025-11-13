using GestionGastos.Models;
using GestionGastos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace GestionGastos.Controllers
{
    public class PresupuestoController : Controller
    {
        private readonly IRepositorioPresupuestos _repositorioPresupuestos;
        private readonly IRepositorioCategorias _repositorioCategorias;

        public PresupuestoController(
            IRepositorioPresupuestos repositorioPresupuestos,
            IRepositorioCategorias repositorioCategorias)
        {
            _repositorioPresupuestos = repositorioPresupuestos;
            _repositorioCategorias = repositorioCategorias;
        }

        // GET: Presupuesto/Index
        public async Task<IActionResult> Index()
        {
            int idUsuarioPrueba = 1;
            var presupuestos = await _repositorioPresupuestos.ObtenerTodos(idUsuarioPrueba);

            // Agregar nombre del mes a cada presupuesto
            foreach (var p in presupuestos)
            {
                p.NombreMes = ObtenerNombreMes(p.mes);
            }

            return View(presupuestos);
        }

        // GET: Presupuesto/Dashboard
        public async Task<IActionResult> Dashboard(int? mes, int? anio)
        {
            int idUsuarioPrueba = 1;
            int mesActual = mes ?? DateTime.Now.Month;
            int anioActual = anio ?? DateTime.Now.Year;

            var presupuestos = await _repositorioPresupuestos.ObtenerResumenConGastos(
                idUsuarioPrueba, mesActual, anioActual);

            foreach (var p in presupuestos)
            {
                p.NombreMes = ObtenerNombreMes(p.mes);
            }

            ViewBag.MesActual = mesActual;
            ViewBag.AnioActual = anioActual;
            ViewBag.NombreMesActual = ObtenerNombreMes(mesActual);

            return View(presupuestos);
        }

        // GET: Presupuesto/Crear
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new Presupuesto
            {
                mes = DateTime.Now.Month,
                anio = DateTime.Now.Year,
                Categorias = await ObtenerCategoriasGasto()
            };

            return View(modelo);
        }

        // POST: Presupuesto/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Presupuesto presupuesto)
        {
            if (!ModelState.IsValid)
            {
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }

            int idUsuarioPrueba = 1;
            presupuesto.id_usuario = idUsuarioPrueba;

            // Verificar si ya existe un presupuesto para esta categoría/mes/año
            if (await _repositorioPresupuestos.ExistePresupuesto(
                idUsuarioPrueba, presupuesto.id_categoria.Value, presupuesto.mes, presupuesto.anio))
            {
                ModelState.AddModelError(string.Empty,
                    "Ya existe un presupuesto para esta categoría en el mes y año seleccionados");
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }

            try
            {
                await _repositorioPresupuestos.Crear(presupuesto);
                TempData["Mensaje"] = "Presupuesto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear el presupuesto: {ex.Message}");
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }
        }

        // GET: Presupuesto/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var presupuesto = await _repositorioPresupuestos.ObtenerPorId(id);

            if (presupuesto == null)
            {
                return NotFound();
            }

            presupuesto.Categorias = await ObtenerCategoriasGasto();
            return View(presupuesto);
        }

        // POST: Presupuesto/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Presupuesto presupuesto)
        {
            if (!ModelState.IsValid)
            {
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }

            int idUsuarioPrueba = 1;

            // Verificar si ya existe otro presupuesto para esta categoría/mes/año
            if (await _repositorioPresupuestos.ExistePresupuesto(
                idUsuarioPrueba, presupuesto.id_categoria.Value, presupuesto.mes,
                presupuesto.anio, presupuesto.id_presupuesto))
            {
                ModelState.AddModelError(string.Empty,
                    "Ya existe otro presupuesto para esta categoría en el mes y año seleccionados");
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }

            try
            {
                await _repositorioPresupuestos.Actualizar(presupuesto);
                TempData["Mensaje"] = "Presupuesto actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el presupuesto: {ex.Message}");
                presupuesto.Categorias = await ObtenerCategoriasGasto();
                return View(presupuesto);
            }
        }

        // GET: Presupuesto/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var presupuesto = await _repositorioPresupuestos.ObtenerPorId(id);

            if (presupuesto == null)
            {
                return NotFound();
            }

            presupuesto.NombreMes = ObtenerNombreMes(presupuesto.mes);
            return View(presupuesto);
        }

        // POST: Presupuesto/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _repositorioPresupuestos.Eliminar(id);
                TempData["Mensaje"] = "Presupuesto eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el presupuesto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método privado para obtener categorías de gasto
        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasGasto()
        {
            var categorias = await _repositorioCategorias.ObtenerPorTipo("Gasto");
            return categorias.Select(c => new SelectListItem(c.nombre, c.id_categoria.ToString()));
        }

        // Método privado para obtener nombre del mes
        private string ObtenerNombreMes(int mes)
        {
            var cultura = new CultureInfo("es-ES");
            return cultura.DateTimeFormat.GetMonthName(mes);
        }
    }
}