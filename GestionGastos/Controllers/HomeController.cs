using GestionGastos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GestionGastos.Repositorios;
using System.Globalization;

namespace GestionGastos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositorioReportes _repositorioReportes;
        private readonly IRepositorioIngresos _repositorioIngresos;
        private readonly IRepositorioGastos _repositorioGastos;

        public HomeController(
            ILogger<HomeController> logger,
            IRepositorioReportes repositorioReportes,
            IRepositorioIngresos repositorioIngresos,
            IRepositorioGastos repositorioGastos)
        {
            _logger = logger;
            _repositorioReportes = repositorioReportes;
            _repositorioIngresos = repositorioIngresos;
            _repositorioGastos = repositorioGastos;
        }

        // --- ACCIÓN INDEX ---
        public async Task<IActionResult> Index()
        {
            int idUsuarioPrueba = 1;

            var modeloSaldo = await _repositorioReportes.ObtenerSaldoActual(idUsuarioPrueba);
            var gastosPorCategoria = await _repositorioReportes.ObtenerGastosPorCategoria(idUsuarioPrueba);

            var dashboardViewModel = new DashboardViewModel
            {
                Saldo = modeloSaldo,
                GastosPorCategoria = gastosPorCategoria
            };

            return View(dashboardViewModel);
        }

        // --- ACCIÓN REPORTE MENSUAL ---
        public async Task<IActionResult> ReporteMensual()
        {
            int idUsuarioPrueba = 1;
            int mesPrueba = 10;
            int anioPrueba = 2025;

            var modelo = await _repositorioReportes.ObtenerReporteMensual(
                idUsuarioPrueba,
                mesPrueba,
                anioPrueba
            );

            return View(modelo);
        }

        // --- NUEVA ACCIÓN: ANÁLISIS FINANCIERO ---
        public async Task<IActionResult> Analisis(int? mes, int? anio)
        {
            int idUsuarioPrueba = 1;
            int mesActual = mes ?? DateTime.Now.Month;
            int anioActual = anio ?? DateTime.Now.Year;

            // Obtener datos del mes actual
            var totalIngresos = await _repositorioIngresos.ObtenerTotalPorMesAnio(idUsuarioPrueba, mesActual, anioActual);

            var ingresosPorCategoria = await _repositorioIngresos.ObtenerIngresosPorCategoria(idUsuarioPrueba, 30);
            var gastosPorCategoria = await _repositorioReportes.ObtenerGastosPorCategoria(idUsuarioPrueba);

            // Calcular total de gastos del mes
            var gastosMes = await _repositorioGastos.ObtenerPorMesAnio(idUsuarioPrueba, mesActual, anioActual);
            var totalGastos = gastosMes.Sum(g => g.Monto);

            var balance = totalIngresos - totalGastos;
            var tasaAhorro = totalIngresos > 0 ? (balance / totalIngresos) * 100 : 0;

            // Obtener comparativa de últimos 6 meses
            var comparativaMensual = new List<ComparativaMensualViewModel>();
            var cultura = new CultureInfo("es-ES");

            for (int i = 5; i >= 0; i--)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                var mesComparativa = fecha.Month;
                var anioComparativa = fecha.Year;

                var ingresosDelMes = await _repositorioIngresos.ObtenerTotalPorMesAnio(idUsuarioPrueba, mesComparativa, anioComparativa);
                var gastosDelMes = await _repositorioGastos.ObtenerPorMesAnio(idUsuarioPrueba, mesComparativa, anioComparativa);
                var totalGastosDelMes = gastosDelMes.Sum(g => g.Monto);

                comparativaMensual.Add(new ComparativaMensualViewModel
                {
                    Mes = cultura.DateTimeFormat.GetAbbreviatedMonthName(mesComparativa),
                    Ingresos = ingresosDelMes,
                    Gastos = totalGastosDelMes,
                    Balance = ingresosDelMes - totalGastosDelMes
                });
            }

            var modelo = new AnalisisFinancieroViewModel
            {
                Mes = mesActual,
                Anio = anioActual,
                NombreMes = cultura.DateTimeFormat.GetMonthName(mesActual),
                TotalIngresos = totalIngresos,
                TotalGastos = totalGastos,
                Balance = balance,
                TasaAhorro = tasaAhorro,
                IngresosPorCategoria = ingresosPorCategoria,
                GastosPorCategoria = gastosPorCategoria,
                ComparativaMensual = comparativaMensual
            };

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