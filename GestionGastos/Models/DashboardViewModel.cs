using System.Collections.Generic;

namespace GestionGastos.Models
{
    // Este ViewModel "contiene" los otros modelos para el Dashboard
    public class DashboardViewModel
    {
        public SaldoViewModel Saldo { get; set; }
        public IEnumerable<ReporteMensualViewModel> ReporteMensual { get; set; }
        public IEnumerable<GastoPorCategoriaViewModel> GastosPorCategoria { get; set; }

    }
}