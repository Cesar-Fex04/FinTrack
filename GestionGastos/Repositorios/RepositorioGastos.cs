using Dapper;
using GestionGastos.Models;
using Microsoft.CodeAnalysis;
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

        // --- CRUD COMPLETO ---

        public async Task<IEnumerable<Gasto>> ObtenerTodos(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                G.id_gasto,
                                G.id_usuario,
                                G.id_categoria,
                                G.id_tipoMovimiento,
                                G.descripcion as Descripcion,
                                G.monto as Monto,
                                G.fechaGasto,
                                G.metodoPago as MetodoPago,
                                C.nombre AS NombreCategoria
                            FROM Gastos G
                            LEFT JOIN Categoria C ON G.id_categoria = C.id_categoria
                            WHERE G.id_usuario = @idUsuario
                            ORDER BY G.fechaGasto DESC";

                return await connection.QueryAsync<Gasto>(sql, new { idUsuario });
            }
        }

        public async Task<Gasto?> ObtenerPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                G.id_gasto,
                                G.id_usuario,
                                G.id_categoria,
                                G.id_tipoMovimiento,
                                G.descripcion as Descripcion,
                                G.monto as Monto,
                                G.fechaGasto,
                                G.metodoPago as MetodoPago,
                                C.nombre AS NombreCategoria
                            FROM Gastos G
                            LEFT JOIN Categoria C ON G.id_categoria = C.id_categoria
                            WHERE G.id_gasto = @id";

                return await connection.QueryFirstOrDefaultAsync<Gasto>(sql, new { id });
            }
        }

        public async Task<int> Crear(Gasto gasto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Gastos 
                           (id_usuario, id_categoria, id_tipoMovimiento, descripcion, monto, fechaGasto, metodoPago)
                           VALUES 
                           (@id_usuario, @id_categoria, @id_tipoMovimiento, @Descripcion, @Monto, @fechaGasto, @MetodoPago);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.QuerySingleAsync<int>(sql, gasto);
            }
        }

        public async Task<bool> Actualizar(Gasto gasto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Gastos 
                           SET id_categoria = @id_categoria,
                               descripcion = @Descripcion,
                               monto = @Monto,
                               fechaGasto = @fechaGasto,
                               metodoPago = @MetodoPago
                           WHERE id_gasto = @id_gasto";

                var filasAfectadas = await connection.ExecuteAsync(sql, gasto);
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Gastos WHERE id_gasto = @id";
                var filasAfectadas = await connection.ExecuteAsync(sql, new { id });
                return filasAfectadas > 0;
            }
        }

        // --- MÉTODOS AUXILIARES ---

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasGasto()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE tipo = 'Gasto' ORDER BY nombre"
                );
            }
        }

        public async Task<IEnumerable<Gasto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                id_gasto,
                                id_usuario,
                                id_categoria,
                                id_tipoMovimiento,
                                descripcion as Descripcion,
                                monto as Monto,
                                fechaGasto,
                                metodoPago as MetodoPago
                           FROM Gastos 
                           WHERE id_usuario = @idUsuario
                               AND MONTH(fechaGasto) = @mes
                               AND YEAR(fechaGasto) = @anio
                           ORDER BY fechaGasto DESC";

                return await connection.QueryAsync<Gasto>(sql, new { idUsuario, mes, anio });
            }
        }
    }
}
