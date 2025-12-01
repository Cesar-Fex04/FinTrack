using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioGastos
    {
        // --- CRUD Completo ---
        Task<IEnumerable<Gasto>> ObtenerTodos(int idUsuario);
        Task<Gasto?> ObtenerPorId(int id);
        Task<int> Crear(Gasto gasto);
        Task<bool> Actualizar(Gasto gasto);
        Task<bool> Eliminar(int id);

        // --- Métodos auxiliares ---
        Task<IEnumerable<Categoria>> ObtenerCategoriasGasto();
        Task<IEnumerable<Gasto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio);
    }
}
