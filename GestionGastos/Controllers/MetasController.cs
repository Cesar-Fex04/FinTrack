using GestionGastos.Models;
using GestionGastos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace GestionGastos.Controllers
{
    public class MetasController : Controller
    {
        private readonly IRepositorioReportes _repositorioReportes;
        private readonly IRepositorioIngresos _repositorioIngresos;
        private readonly IRepositorioGastos _repositorioGastos;

        public MetasController(
            IRepositorioReportes repositorioReportes,
            IRepositorioIngresos repositorioIngresos,
            IRepositorioGastos repositorioGastos)
        {
            _repositorioReportes = repositorioReportes;
            _repositorioIngresos = repositorioIngresos;
            _repositorioGastos = repositorioGastos;
        }

        // GET: Metas/Index
        public async Task<IActionResult> Index()
        {
            int idUsuarioPrueba = 1;

            // Obtener datos de los últimos 6 meses
            var cultura = new CultureInfo("es-ES");
            decimal totalIngresos = 0;
            decimal totalGastos = 0;
            int mesesConDatos = 0;

            var proyeccionAhorro = new List<ProyeccionMensual>();

            for (int i = 5; i >= 0; i--)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                var mes = fecha.Month;
                var anio = fecha.Year;

                var ingresosMes = await _repositorioIngresos.ObtenerTotalPorMesAnio(idUsuarioPrueba, mes, anio);
                var gastosMes = await _repositorioGastos.ObtenerPorMesAnio(idUsuarioPrueba, mes, anio);
                var totalGastosMes = gastosMes.Sum(g => g.Monto);

                if (ingresosMes > 0 || totalGastosMes > 0)
                {
                    totalIngresos += ingresosMes;
                    totalGastos += totalGastosMes;
                    mesesConDatos++;
                }

                proyeccionAhorro.Add(new ProyeccionMensual
                {
                    Mes = cultura.DateTimeFormat.GetAbbreviatedMonthName(mes),
                    AhorroAcumulado = ingresosMes - totalGastosMes,
                    IngresosMes = ingresosMes,
                    GastosMes = totalGastosMes
                });
            }

            var ingresoPromedio = mesesConDatos > 0 ? totalIngresos / mesesConDatos : 0;
            var gastoPromedio = mesesConDatos > 0 ? totalGastos / mesesConDatos : 0;
            var ahorroPromedio = ingresoPromedio - gastoPromedio;
            var tasaAhorro = ingresoPromedio > 0 ? (ahorroPromedio / ingresoPromedio) * 100 : 0;

            var saldo = await _repositorioReportes.ObtenerSaldoActual(idUsuarioPrueba);

            var modelo = new MetasFinancierasViewModel
            {
                IngresoMensualPromedio = ingresoPromedio,
                GastoMensualPromedio = gastoPromedio,
                AhorroMensualPromedio = ahorroPromedio,
                TasaAhorroActual = tasaAhorro,
                SaldoActual = saldo.SaldoActual,
                ProyeccionAhorro = proyeccionAhorro
            };

            return View(modelo);
        }

        // POST: Metas/CalcularTiempo
        [HttpPost]
        public IActionResult CalcularTiempo([FromBody] CalcularTiempoMetaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, mensaje = "Datos inválidos" });
            }

            var totalAhorrar = request.MontoMeta - request.SaldoInicial;
            var mesesNecesarios = (int)Math.Ceiling(totalAhorrar / request.AhorroMensual);
            var fechaEstimada = DateTime.Now.AddMonths(mesesNecesarios);

            var anios = mesesNecesarios / 12;
            var mesesRestantes = mesesNecesarios % 12;

            var ahorroPorDia = request.AhorroMensual / 30;
            var ahorroPorSemana = request.AhorroMensual / 4;

            // Verificar viabilidad
            bool esViable = mesesNecesarios <= 120; // Máximo 10 años
            string mensajeViabilidad = esViable
                ? "Tu meta es alcanzable con este plan de ahorro"
                : "Esta meta tomará más de 10 años. Considera aumentar tu ahorro mensual o reducir el monto objetivo.";

            // Generar proyección
            var proyeccion = new List<ProyeccionMensualMeta>();
            var cultura = new CultureInfo("es-ES");
            decimal ahorroAcumulado = request.SaldoInicial;

            for (int i = 1; i <= Math.Min(mesesNecesarios, 24); i++) // Máximo 24 meses en la proyección
            {
                ahorroAcumulado += request.AhorroMensual;
                var fecha = DateTime.Now.AddMonths(i);
                var porcentaje = (ahorroAcumulado / request.MontoMeta) * 100;

                proyeccion.Add(new ProyeccionMensualMeta
                {
                    Mes = i,
                    NombreMes = cultura.DateTimeFormat.GetMonthName(fecha.Month) + " " + fecha.Year,
                    AhorroMensual = request.AhorroMensual,
                    AhorroAcumulado = ahorroAcumulado,
                    PorcentajeCompletado = Math.Min(porcentaje, 100)
                });
            }

            var response = new CalcularTiempoMetaResponse
            {
                MesesNecesarios = mesesNecesarios,
                Anios = anios,
                MesesRestantes = mesesRestantes,
                FechaEstimada = fechaEstimada,
                MontoTotal = request.MontoMeta,
                SaldoInicial = request.SaldoInicial,
                TotalAhorrar = totalAhorrar,
                AhorroPorDia = ahorroPorDia,
                AhorroPorSemana = ahorroPorSemana,
                EsViable = esViable,
                MensajeViabilidad = mensajeViabilidad,
                Proyeccion = proyeccion
            };

            return Ok(new { success = true, data = response });
        }

        // POST: Metas/CalcularPlan
        [HttpPost]
        public async Task<IActionResult> CalcularPlan([FromBody] PlanAhorroRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, mensaje = "Datos inválidos" });
            }

            int idUsuarioPrueba = 1;

            // Obtener ingreso promedio
            var totalIngresos = 0m;
            var mesesConDatos = 0;

            for (int i = 0; i < 3; i++)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                var ingresosMes = await _repositorioIngresos.ObtenerTotalPorMesAnio(
                    idUsuarioPrueba, fecha.Month, fecha.Year);

                if (ingresosMes > 0)
                {
                    totalIngresos += ingresosMes;
                    mesesConDatos++;
                }
            }

            var ingresoPromedio = mesesConDatos > 0 ? totalIngresos / mesesConDatos : 0;

            var totalAhorrar = request.MontoMeta - request.SaldoInicial;
            var ahorroMensualRequerido = totalAhorrar / request.MesesDisponibles;

            // Aplicar porcentaje extra si se especificó
            if (request.PorcentajeAhorroExtra > 0)
            {
                ahorroMensualRequerido *= (1 + request.PorcentajeAhorroExtra / 100);
            }

            var ahorroPorDia = ahorroMensualRequerido / 30;
            var ahorroPorSemana = ahorroMensualRequerido / 4;

            var porcentajeIngresoNecesario = ingresoPromedio > 0
                ? (ahorroMensualRequerido / ingresoPromedio) * 100
                : 0;

            bool esRealista = porcentajeIngresoNecesario <= 50; // No más del 50% del ingreso

            string recomendacion;
            if (porcentajeIngresoNecesario <= 20)
                recomendacion = "Excelente plan. Este ahorro es muy manejable con tus ingresos actuales.";
            else if (porcentajeIngresoNecesario <= 30)
                recomendacion = "Buen plan. Requerirá disciplina pero es alcanzable.";
            else if (porcentajeIngresoNecesario <= 50)
                recomendacion = "Plan desafiante. Considera reducir gastos no esenciales.";
            else
                recomendacion = "Plan muy ambicioso. Te recomendamos extender el plazo o buscar ingresos adicionales.";

            // Generar proyección
            var proyeccion = new List<ProyeccionMensualMeta>();
            var cultura = new CultureInfo("es-ES");
            decimal ahorroAcumulado = request.SaldoInicial;

            for (int i = 1; i <= request.MesesDisponibles; i++)
            {
                ahorroAcumulado += ahorroMensualRequerido;
                var fecha = DateTime.Now.AddMonths(i);
                var porcentaje = (ahorroAcumulado / request.MontoMeta) * 100;

                proyeccion.Add(new ProyeccionMensualMeta
                {
                    Mes = i,
                    NombreMes = cultura.DateTimeFormat.GetMonthName(fecha.Month) + " " + fecha.Year,
                    AhorroMensual = ahorroMensualRequerido,
                    AhorroAcumulado = Math.Min(ahorroAcumulado, request.MontoMeta),
                    PorcentajeCompletado = Math.Min(porcentaje, 100)
                });
            }

            var response = new PlanAhorroResponse
            {
                AhorroMensualRequerido = ahorroMensualRequerido,
                AhorroPorDia = ahorroPorDia,
                AhorroPorSemana = ahorroPorSemana,
                MontoTotal = request.MontoMeta,
                SaldoInicial = request.SaldoInicial,
                TotalAhorrar = totalAhorrar,
                PorcentajeIngresoNecesario = porcentajeIngresoNecesario,
                EsRealistaConIngresoActual = esRealista,
                Recomendacion = recomendacion,
                Proyeccion = proyeccion
            };

            return Ok(new { success = true, data = response });
        }
    }
}