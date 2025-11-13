namespace GestionGastos.Models
{
    // Esta clase recibirá el resultado del SP "GenerarReporteMensual"
    public class ReporteMensualViewModel
    {
        // Modificado para coincidir con el alias "AS Categoria" del SP
        public required string Categoria { get; set; }

        // Modificado para coincidir con el alias "AS Presupuesto" del SP
        public decimal Presupuesto { get; set; }

        // Modificado para coincidir con el alias "AS Gasto" del SP
        public decimal Gasto { get; set; }

        // Este ya coincide con "AS Diferencia"
        public decimal Diferencia { get; set; }
    }
}