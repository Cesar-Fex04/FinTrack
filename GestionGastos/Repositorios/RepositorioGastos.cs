using Dapper;
using GestionGastos.Models;
using Microsoft.Data.SqlClient;

namespace GestionGastos.Repositorios
{
    public class RepositorioGastos : IRepositorioGastos
    {
        private readonly string _connectionString;

        public RepositorioGastos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasGasto()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE tipo = 'Gasto' ORDER BY nombre"
                );
            }
        }

        public async Task<IEnumerable<tipoMovimiento>> ObtenerTiposMovimiento()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<tipoMovimiento>(
                    "SELECT * FROM tipoMovimiento"
                );
            }
        }

        public async Task Crear(Gasto gasto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // ---- INICIO DE LA CORRECCIÓN ----
                // Los parámetros (@Descripcion, @Monto) AHORA COINCIDEN 
                // con las propiedades de la clase Gasto (Descripcion, Monto)
                await connection.ExecuteAsync(
                    @"INSERT INTO Gastos 
                      (id_usuario, id_categoria, id_tipoMovimiento, descripcion, monto, fechaGasto, metodoPago)
                      VALUES 
                      (@id_usuario, @id_categoria, @id_tipoMovimiento, @Descripcion, @Monto, @fechaGasto, @MetodoPago)",
                    gasto // Dapper mapea las propiedades de este objeto a los parámetros
                );
                // ---- FIN DE LA CORRECCIÓN ----
            }
        }
        public async Task<IEnumerable<Gasto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT * FROM Gastos 
                           WHERE id_usuario = @idUsuario
                               AND MONTH(fechaGasto) = @mes
                               AND YEAR(fechaGasto) = @anio
                           ORDER BY fechaGasto DESC";

                return await connection.QueryAsync<Gasto>(sql, new { idUsuario, mes, anio });
            }
        }

    }
}