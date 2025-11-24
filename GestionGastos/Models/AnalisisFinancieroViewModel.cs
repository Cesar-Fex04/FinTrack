namespace GestionGastos.Models
{
    public class AnalisisFinancieroViewModel
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string NombreMes { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal Balance { get; set; }
        public decimal TasaAhorro { get; set; }
        public IEnumerable<IngresoPorCategoriaViewModel> IngresosPorCategoria { get; set; }
        public IEnumerable<GastoPorCategoriaViewModel> GastosPorCategoria { get; set; }
        public IEnumerable<ComparativaMensualViewModel> ComparativaMensual { get; set; }
    }

    public class ComparativaMensualViewModel
    {
        public string Mes { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }
        public decimal Balance { get; set; }
    }
}