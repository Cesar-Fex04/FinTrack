using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioIngresos
    {
        // Obtener todos los ingresos de un usuario
        Task<IEnumerable<Ingreso>> ObtenerTodos(int idUsuario);

        // Obtener ingresos por rango de fechas
        Task<IEnumerable<Ingreso>> ObtenerPorRangoFechas(int idUsuario, DateTime fechaInicio, DateTime fechaFin);

        // Obtener ingresos por mes y año
        Task<IEnumerable<Ingreso>> ObtenerPorMesAnio(int idUsuario, int mes, int anio);

        // Obtener un ingreso por ID
        Task<Ingreso?> ObtenerPorId(int id);

        // Crear nuevo ingreso
        Task<int> Crear(Ingreso ingreso);

        // Actualizar ingreso existente
        Task<bool> Actualizar(Ingreso ingreso);

        // Eliminar ingreso
        Task<bool> Eliminar(int id);

        // Obtener categorías de ingreso
        Task<IEnumerable<Categoria>> ObtenerCategoriasIngreso();

        // Obtener total de ingresos por período
        Task<decimal> ObtenerTotalPorMesAnio(int idUsuario, int mes, int anio);

        // Obtener ingresos agrupados por categoría
        Task<IEnumerable<IngresoPorCategoriaViewModel>> ObtenerIngresosPorCategoria(int idUsuario, int diasAtras = 30);
    }
}