using Dapper;
using GestionGastos.Models;
using Microsoft.Data.SqlClient;

namespace GestionGastos.Repositorios
{
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string _connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodas()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria ORDER BY tipo, nombre"
                );
            }
        }

        public async Task<IEnumerable<Categoria>> ObtenerPorTipo(string tipo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE tipo = @tipo ORDER BY nombre",
                    new { tipo }
                );
            }
        }

        public async Task<Categoria?> ObtenerPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE id_categoria = @id",
                    new { id }
                );
            }
        }

        public async Task<int> Crear(Categoria categoria)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Categoria (nombre, tipo, descripcion, imagen_url) 
                           VALUES (@nombre, @tipo, @descripcion, @imagen_url);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.QuerySingleAsync<int>(sql, categoria);
            }
        }

        public async Task<bool> Actualizar(Categoria categoria)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Categoria 
                           SET nombre = @nombre, 
                               tipo = @tipo, 
                               descripcion = @descripcion,
                               imagen_url = @imagen_url
                           WHERE id_categoria = @id_categoria";

                var filasAfectadas = await connection.ExecuteAsync(sql, categoria);
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Categoria WHERE id_categoria = @id";
                var filasAfectadas = await connection.ExecuteAsync(sql, new { id });
                return filasAfectadas > 0;
            }
        }

        public async Task<bool> ExisteNombre(string nombre, int? idExcluir = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT COUNT(1) 
                           FROM Categoria 
                           WHERE LOWER(nombre) = LOWER(@nombre) 
                           AND (@idExcluir IS NULL OR id_categoria != @idExcluir)";

                var count = await connection.QuerySingleAsync<int>(sql, new { nombre, idExcluir });
                return count > 0;
            }
        }
    }
}