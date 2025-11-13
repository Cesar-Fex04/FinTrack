using GestionGastos.Models;
using GestionGastos.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace GestionGastos.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriasController(
            IRepositorioCategorias repositorioCategorias,
            IWebHostEnvironment webHostEnvironment)
        {
            _repositorioCategorias = repositorioCategorias;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _repositorioCategorias.ObtenerTodas();
            return View(categorias);
        }

        // GET: Categorias/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Categorias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            // Verificar si ya existe una categoría con el mismo nombre
            if (await _repositorioCategorias.ExisteNombre(categoria.nombre))
            {
                ModelState.AddModelError("nombre", "Ya existe una categoría con este nombre");
                return View(categoria);
            }

            // Procesar la imagen si se subió
            if (categoria.ImagenArchivo != null && categoria.ImagenArchivo.Length > 0)
            {
                categoria.imagen_url = await GuardarImagen(categoria.ImagenArchivo);
            }
            else
            {
                // Imagen por defecto según el tipo
                categoria.imagen_url = categoria.tipo == "Gasto"
                    ? "/images/categorias/default-gasto.png"
                    : "/images/categorias/default-ingreso.png";
            }

            try
            {
                await _repositorioCategorias.Crear(categoria);
                TempData["Mensaje"] = "Categoría creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear la categoría: {ex.Message}");
                return View(categoria);
            }
        }

        // GET: Categorias/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _repositorioCategorias.ObtenerPorId(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            // Verificar si ya existe otra categoría con el mismo nombre
            if (await _repositorioCategorias.ExisteNombre(categoria.nombre, categoria.id_categoria))
            {
                ModelState.AddModelError("nombre", "Ya existe otra categoría con este nombre");
                return View(categoria);
            }

            // Obtener la categoría existente para mantener la imagen anterior si no se sube una nueva
            var categoriaExistente = await _repositorioCategorias.ObtenerPorId(categoria.id_categoria);

            if (categoriaExistente == null)
            {
                return NotFound();
            }

            // Procesar nueva imagen si se subió
            if (categoria.ImagenArchivo != null && categoria.ImagenArchivo.Length > 0)
            {
                // Eliminar imagen anterior si existe y no es la default
                if (!string.IsNullOrEmpty(categoriaExistente.imagen_url)
                    && !categoriaExistente.imagen_url.Contains("default"))
                {
                    EliminarImagen(categoriaExistente.imagen_url);
                }

                categoria.imagen_url = await GuardarImagen(categoria.ImagenArchivo);
            }
            else
            {
                // Mantener la imagen existente
                categoria.imagen_url = categoriaExistente.imagen_url;
            }

            try
            {
                await _repositorioCategorias.Actualizar(categoria);
                TempData["Mensaje"] = "Categoría actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar la categoría: {ex.Message}");
                return View(categoria);
            }
        }

        // GET: Categorias/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _repositorioCategorias.ObtenerPorId(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var categoria = await _repositorioCategorias.ObtenerPorId(id);

                if (categoria == null)
                {
                    return NotFound();
                }

                // Eliminar la imagen si existe y no es la default
                if (!string.IsNullOrEmpty(categoria.imagen_url)
                    && !categoria.imagen_url.Contains("default"))
                {
                    EliminarImagen(categoria.imagen_url);
                }

                await _repositorioCategorias.Eliminar(id);
                TempData["Mensaje"] = "Categoría eliminada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo eliminar la categoría. Es posible que esté siendo utilizada en gastos o presupuestos.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método privado para guardar imágenes
        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            // Crear nombre único para el archivo
            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);

            // Ruta completa donde se guardará
            string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categorias");

            // Crear carpeta si no existe
            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

            // Guardar el archivo
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Retornar la ruta relativa para guardar en BD
            return "/images/categorias/" + nombreArchivo;
        }

        // Método privado para eliminar imágenes
        private void EliminarImagen(string rutaImagen)
        {
            if (string.IsNullOrEmpty(rutaImagen))
                return;

            string rutaCompleta = Path.Combine(_webHostEnvironment.WebRootPath, rutaImagen.TrimStart('/'));

            if (System.IO.File.Exists(rutaCompleta))
            {
                System.IO.File.Delete(rutaCompleta);
            }
        }
    }
}