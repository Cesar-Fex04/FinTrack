using GestionGastos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GestionGastos.Repositorios;

namespace GestionGastos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositorioReportes _repositorioReportes;

        public HomeController(ILogger<HomeController> logger, IRepositorioReportes repositorioReportes)
        {
            _logger = logger;
            _repositorioReportes = repositorioReportes;
        }

        // --- ACCIÓN INDEX MODIFICADA ---
        public async Task<IActionResult> Index()
        {
            // TODO: Debes obtener el ID del usuario que ha iniciado sesión
            int idUsuarioPrueba = 1;

            // 1. Obtenemos el Saldo (Usando tu SP 'CalcularSaldoActual')
            var modeloSaldo = await _repositorioReportes.ObtenerSaldoActual(idUsuarioPrueba);

            // 2. Obtenemos los Gastos por Categoría (últimos 30 días)
            var gastosPorCategoria = await _repositorioReportes.ObtenerGastosPorCategoria(idUsuarioPrueba);

            // 3. Creamos el DashboardViewModel con ambos datos
            var dashboardViewModel = new DashboardViewModel
            {
                Saldo = modeloSaldo,
                GastosPorCategoria = gastosPorCategoria
            };

            // 4. Pasamos el DashboardViewModel a la vista
            return View(dashboardViewModel);
        }

        // --- ACCIÓN NUEVA PARA LA PÁGINA DEL REPORTE ---
        public async Task<IActionResult> ReporteMensual()
        {
            // TODO: Debes obtener el ID del usuario que ha iniciado sesión
            int idUsuarioPrueba = 1;
            // TODO: Debes obtener el Mes/Año del usuario, por ahora usamos los de prueba
            int mesPrueba = 10;
            int anioPrueba = 2025;

            var modelo = await _repositorioReportes.ObtenerReporteMensual(
                idUsuarioPrueba,
                mesPrueba,
                anioPrueba
            );

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}