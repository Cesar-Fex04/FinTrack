using GestionGastos.Models;

namespace GestionGastos.Repositorios
{
    public interface IRepositorioPresupuestos
    {
        // Obtener todos los presupuestos de un usuario
        Task<IEnumerable<Presupuesto>> ObtenerTodos(int idUsuario);

        // Obtener presupuestos por mes y año
        Task<IEnumerable<Presupuesto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio);

        // Obtener un presupuesto por ID
        Task<Presupuesto?> ObtenerPorId(int id);

        // Crear nuevo presupuesto
        Task<int> Crear(Presupuesto presupuesto);

        // Actualizar presupuesto existente
        Task<bool> Actualizar(Presupuesto presupuesto);

        // Eliminar presupuesto
        Task<bool> Eliminar(int id);

        // Verificar si ya existe un presupuesto para esa categoría/mes/año
        Task<bool> ExistePresupuesto(int idUsuario, int idCategoria, int mes, int anio, int? idExcluir = null);

        // Obtener resumen de presupuestos con gastos actuales
        Task<IEnumerable<Presupuesto>> ObtenerResumenConGastos(int idUsuario, int mes, int anio);
    }
}