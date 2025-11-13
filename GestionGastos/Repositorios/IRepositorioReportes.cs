using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioReportes
    {
        // El método para el dashboard de saldo
        Task<SaldoViewModel> ObtenerSaldoActual(int id_usuario);

        // El método para el dropdown de categorías
        Task<IEnumerable<Categoria>> ObtenerCategoriasGasto();

        // El método para el SP de presupuesto
        Task<PresupuestoViewModel> VerificarPresupuesto(int id_usuario, int mes, int anio, int id_categoria);

        Task<IEnumerable<ReporteMensualViewModel>> ObtenerReporteMensual(int id_usuario, int mes, int anio);
        Task<IEnumerable<GastoPorCategoriaViewModel>> ObtenerGastosPorCategoria(int idUsuarioPrueba);
    }
}