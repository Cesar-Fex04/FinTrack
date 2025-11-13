using Dapper;
using GestionGastos.Models;
using Microsoft.Data.SqlClient;

namespace GestionGastos.Repositorios
{
    public class RepositorioPresupuestos : IRepositorioPresupuestos
    {
        private readonly string _connectionString;

        public RepositorioPresupuestos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerTodos(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                P.id_presupuesto,
                                P.id_usuario,
                                P.id_categoria,
                                P.montoLimite,
                                P.mes,
                                P.anio,
                                C.nombre AS NombreCategoria
                            FROM Presupuesto P
                            INNER JOIN Categoria C ON P.id_categoria = C.id_categoria
                            WHERE P.id_usuario = @idUsuario
                            ORDER BY P.anio DESC, P.mes DESC, C.nombre";

                return await connection.QueryAsync<Presupuesto>(sql, new { idUsuario });
            }
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerPorMesAnio(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                P.id_presupuesto,
                                P.id_usuario,
                                P.id_categoria,
                                P.montoLimite,
                                P.mes,
                                P.anio,
                                C.nombre AS NombreCategoria
                            FROM Presupuesto P
                            INNER JOIN Categoria C ON P.id_categoria = C.id_categoria
                            WHERE P.id_usuario = @idUsuario
                                AND P.mes = @mes
                                AND P.anio = @anio
                            ORDER BY C.nombre";

                return await connection.QueryAsync<Presupuesto>(sql, new { idUsuario, mes, anio });
            }
        }

        public async Task<Presupuesto?> ObtenerPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                P.*,
                                C.nombre AS NombreCategoria
                            FROM Presupuesto P
                            INNER JOIN Categoria C ON P.id_categoria = C.id_categoria
                            WHERE P.id_presupuesto = @id";

                return await connection.QueryFirstOrDefaultAsync<Presupuesto>(sql, new { id });
            }
        }

        public async Task<int> Crear(Presupuesto presupuesto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Presupuesto (id_usuario, id_categoria, montoLimite, mes, anio)
                           VALUES (@id_usuario, @id_categoria, @montoLimite, @mes, @anio);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.QuerySingleAsync<int>(sql, presupuesto);
            }
        }

        public async Task<bool> Actualizar(Presupuesto presupuesto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Presupuesto 
                           SET id_categoria = @id_categoria,
                               montoLimite = @montoLimite,
                               mes = @mes,
                               anio = @anio
                           WHERE id_presupuesto = @id_presupuesto";

                var filasAfectadas = await connection.ExecuteAsync(sql, presupuesto);
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Presupuesto WHERE id_presupuesto = @id";
                var filasAfectadas = await connection.ExecuteAsync(sql, new { id });
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> ExistePresupuesto(int idUsuario, int idCategoria, int mes, int anio, int? idExcluir = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT COUNT(1) 
                           FROM Presupuesto 
                           WHERE id_usuario = @idUsuario 
                               AND id_categoria = @idCategoria
                               AND mes = @mes
                               AND anio = @anio
                               AND (@idExcluir IS NULL OR id_presupuesto != @idExcluir)";

                var count = await connection.QuerySingleAsync<int>(sql,
                    new { idUsuario, idCategoria, mes, anio, idExcluir });
                return count > 0;
            }
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerResumenConGastos(int idUsuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                P.id_presupuesto,
                                P.id_usuario,
                                P.id_categoria,
                                P.montoLimite,
                                P.mes,
                                P.anio,
                                C.nombre AS NombreCategoria,
                                ISNULL(SUM(G.monto), 0) AS MontoGastado
                            FROM Presupuesto P
                            INNER JOIN Categoria C ON P.id_categoria = C.id_categoria
                            LEFT JOIN Gastos G ON G.id_categoria = P.id_categoria
                                AND G.id_usuario = P.id_usuario
                                AND MONTH(G.fechaGasto) = P.mes
                                AND YEAR(G.fechaGasto) = P.anio
                            WHERE P.id_usuario = @idUsuario
                                AND P.mes = @mes
                                AND P.anio = @anio
                            GROUP BY 
                                P.id_presupuesto,
                                P.id_usuario,
                                P.id_categoria,
                                P.montoLimite,
                                P.mes,
                                P.anio,
                                C.nombre
                            ORDER BY C.nombre";

                var presupuestos = await connection.QueryAsync<Presupuesto>(sql, new { idUsuario, mes, anio });

                // Calcular porcentaje consumido
                foreach (var p in presupuestos)
                {
                    if (p.montoLimite > 0)
                    {
                        p.PorcentajeConsumido = (p.MontoGastado / p.montoLimite) * 100;
                    }
                }

                return presupuestos;
            }
        }
    }
}