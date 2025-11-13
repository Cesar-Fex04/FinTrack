using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioCategorias
    {
        // Obtener todas las categorías
        Task<IEnumerable<Categoria>> ObtenerTodas();

        // Obtener categorías por tipo (Gasto o Ingreso)
        Task<IEnumerable<Categoria>> ObtenerPorTipo(string tipo);

        // Obtener una categoría por ID
        Task<Categoria?> ObtenerPorId(int id);

        // Crear nueva categoría
        Task<int> Crear(Categoria categoria);

        // Actualizar categoría existente
        Task<bool> Actualizar(Categoria categoria);

        // Eliminar categoría
        Task<bool> Eliminar(int id);

        // Verificar si existe una categoría con el mismo nombre
        Task<bool> ExisteNombre(string nombre, int? idExcluir = null);
    }
}