using Dapper;
using GestionGastos.Models;
using Microsoft.Data.SqlClient;

namespace GestionGastos.Repositorios
{
    public class RepositorioIngresos : IRepositorioIngresos
    {
        private readonly string _connectionString;

        public RepositorioIngresos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Ingreso>> ObtenerTodos(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                I.id_ingreso,
                                I.id_usuario,
                                I.id_categoria,
                                I.monto,
                                I.descripcion,
                                I.fechaIngreso,
                                C.nombre AS NombreCategoria
                            FROM Ingreso I
                            LEFT JOIN Categoria C ON I.id_categoria = C.id_categoria
                            WHERE I.id_usuario = @idUsuario
                            ORDER BY I.fechaIngreso DESC";

                return await connection.QueryAsync<Ingreso>(sql, new { idUsuario });
            }
        }

        public async Task<IEnumerable<Ingreso>> ObtenerPorRangoFechas(int idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                I.id_ingreso,
                                I.id_usuario,
                                I.id_categoria,
                                I.monto,
                                I.descripcion,
                                I.fechaIngreso,
                                C.nombre AS NombreCategoria
                            FROM Ingreso I
                            LEFT JOIN Categoria C ON I.id_categoria = C.id_categoria
                            WHERE I.id_usuario = @idUsuario
                                AND I.fechaIngreso >= @fechaInicio
                                AND I.fechaIngreso <= @fechaFin
                            ORDER BY I.fechaIngreso DESC";

                return await connection.QueryAsync<Ingreso>(sql, new { idUsuario, fechaInicio, fechaFin });
            }
        }

        public async Task<IEnumerable<Ingreso>> ObtenerPorMesAnio(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                I.id_ingreso,
                                I.id_usuario,
                                I.id_categoria,
                                I.monto,
                                I.descripcion,
                                I.fechaIngreso,
                                C.nombre AS NombreCategoria
                            FROM Ingreso I
                            LEFT JOIN Categoria C ON I.id_categoria = C.id_categoria
                            WHERE I.id_usuario = @idUsuario
                                AND MONTH(I.fechaIngreso) = @mes
                                AND YEAR(I.fechaIngreso) = @anio
                            ORDER BY I.fechaIngreso DESC";

                return await connection.QueryAsync<Ingreso>(sql, new { idUsuario, mes, anio });
            }
        }

        public async Task<Ingreso?> ObtenerPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                I.*,
                                C.nombre AS NombreCategoria
                            FROM Ingreso I
                            LEFT JOIN Categoria C ON I.id_categoria = C.id_categoria
                            WHERE I.id_ingreso = @id";

                return await connection.QueryFirstOrDefaultAsync<Ingreso>(sql, new { id });
            }
        }

        public async Task<int> Crear(Ingreso ingreso)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Ingreso (id_usuario, id_categoria, monto, descripcion, fechaIngreso)
                           VALUES (@id_usuario, @id_categoria, @monto, @descripcion, @fechaIngreso);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.QuerySingleAsync<int>(sql, ingreso);
            }
        }

        public async Task<bool> Actualizar(Ingreso ingreso)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Ingreso 
                           SET id_categoria = @id_categoria,
                               monto = @monto,
                               descripcion = @descripcion,
                               fechaIngreso = @fechaIngreso
                           WHERE id_ingreso = @id_ingreso";

                var filasAfectadas = await connection.ExecuteAsync(sql, ingreso);
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Ingreso WHERE id_ingreso = @id";
                var filasAfectadas = await connection.ExecuteAsync(sql, new { id });
                return filasAfectadas > 0;
            }
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasIngreso()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE tipo = 'Ingreso' ORDER BY nombre"
                );
            }
        }

        public async Task<decimal> ObtenerTotalPorMesAnio(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT ISNULL(SUM(monto), 0)
                           FROM Ingreso
                           WHERE id_usuario = @idUsuario
                               AND MONTH(fechaIngreso) = @mes
                               AND YEAR(fechaIngreso) = @anio";

                return await connection.QuerySingleAsync<decimal>(sql, new { idUsuario, mes, anio });
            }
        }

        public async Task<IEnumerable<IngresoPorCategoriaViewModel>> ObtenerIngresosPorCategoria(int idUsuario, int diasAtras = 30)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                c.nombre AS Categoria, 
                                SUM(i.monto) AS Monto
                            FROM 
                                Ingreso i 
                            INNER JOIN 
                                Categoria c ON i.id_categoria = c.id_categoria
                            WHERE 
                                i.id_usuario = @idUsuario
                                AND i.fechaIngreso >= DATEADD(day, -@diasAtras, GETDATE())
                            GROUP BY 
                                c.nombre
                            ORDER BY 
                                Monto DESC";

                return await connection.QueryAsync<IngresoPorCategoriaViewModel>(sql, new { idUsuario, diasAtras });
            }
        }
    }
}