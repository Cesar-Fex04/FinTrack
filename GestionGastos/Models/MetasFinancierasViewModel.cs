using System.ComponentModel.DataAnnotations;

namespace GestionGastos.Models
{
    // ViewModel principal para la vista de Metas
    public class MetasFinancierasViewModel
    {
        public decimal IngresoMensualPromedio { get; set; }
        public decimal GastoMensualPromedio { get; set; }
        public decimal AhorroMensualPromedio { get; set; }
        public decimal TasaAhorroActual { get; set; }
        public decimal SaldoActual { get; set; }
        public List<ProyeccionMensual> ProyeccionAhorro { get; set; } = new List<ProyeccionMensual>();
    }

    // Proyección mes a mes
    public class ProyeccionMensual
    {
        public string Mes { get; set; }
        public decimal AhorroAcumulado { get; set; }
        public decimal IngresosMes { get; set; }
        public decimal GastosMes { get; set; }
    }

    // Request para calcular tiempo para meta
    public class CalcularTiempoMetaRequest
    {
        [Required(ErrorMessage = "El monto de la meta es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoMeta { get; set; }

        [Required(ErrorMessage = "El ahorro mensual es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "El ahorro mensual debe ser mayor a 0")]
        public decimal AhorroMensual { get; set; }

        public decimal SaldoInicial { get; set; } = 0;
    }

    // Response para calcular tiempo para meta
    public class CalcularTiempoMetaResponse
    {
        public int MesesNecesarios { get; set; }
        public int Anios { get; set; }
        public int MesesRestantes { get; set; }
        public DateTime FechaEstimada { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal TotalAhorrar { get; set; }
        public decimal AhorroPorDia { get; set; }
        public decimal AhorroPorSemana { get; set; }
        public bool EsViable { get; set; }
        public string MensajeViabilidad { get; set; }
        public List<ProyeccionMensualMeta> Proyeccion { get; set; } = new List<ProyeccionMensualMeta>();
    }

    // Proyección para una meta específica
    public class ProyeccionMensualMeta
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; }
        public decimal AhorroMensual { get; set; }
        public decimal AhorroAcumulado { get; set; }
        public decimal PorcentajeCompletado { get; set; }
    }

    // Request para plan de ahorro personalizado
    public class PlanAhorroRequest
    {
        [Required]
        [Range(1, double.MaxValue)]
        public decimal MontoMeta { get; set; }

        [Required]
        [Range(1, 360)]
        public int MesesDisponibles { get; set; }

        public decimal SaldoInicial { get; set; } = 0;

        [Range(0, 100)]
        public decimal PorcentajeAhorroExtra { get; set; } = 0;
    }

    // Response para plan de ahorro
    public class PlanAhorroResponse
    {
        public decimal AhorroMensualRequerido { get; set; }
        public decimal AhorroPorDia { get; set; }
        public decimal AhorroPorSemana { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal TotalAhorrar { get; set; }
        public decimal PorcentajeIngresoNecesario { get; set; }
        public bool EsRealistaConIngresoActual { get; set; }
        public string Recomendacion { get; set; }
        public List<ProyeccionMensualMeta> Proyeccion { get; set; } = new List<ProyeccionMensualMeta>();
    }

    // Request para comparar escenarios
    public class CompararEscenariosRequest
    {
        public decimal MontoMeta { get; set; }
        public decimal Escenario1_AhorroMensual { get; set; }
        public decimal Escenario2_AhorroMensual { get; set; }
        public decimal Escenario3_AhorroMensual { get; set; }
        public decimal SaldoInicial { get; set; } = 0;
    }
}