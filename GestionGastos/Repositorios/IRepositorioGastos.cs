using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioGastos
    {
        // --- Métodos para el formulario de Crear ---
        Task<IEnumerable<Categoria>> ObtenerCategoriasGasto();
        Task<IEnumerable<tipoMovimiento>> ObtenerTiposMovimiento();
        Task Crear(Gasto gasto);

        // --- Método para obtener gastos por mes y año ---

        Task<IEnumerable<Gasto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio);
    }
}