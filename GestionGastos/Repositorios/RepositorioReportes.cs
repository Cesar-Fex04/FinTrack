using Dapper;
using GestionGastos.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestionGastos.Repositorios
{
    public class RepositorioReportes : IRepositorioReportes
    {
        private readonly string _connectionString;

        public RepositorioReportes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- MÉTODO CORREGIDO ---
        public async Task<SaldoViewModel> ObtenerSaldoActual(int id_usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var resultado = await connection.QueryFirstOrDefaultAsync<SaldoViewModel>(
                    "CalcularSaldoActual",
                    new { id_usuario },
                    commandType: CommandType.StoredProcedure
                );

                return resultado;
            }
        }

        // --- IMPLEMENTACIÓN 1 (Para el dropdown) ---
        public async Task<IEnumerable<Categoria>> ObtenerCategoriasGasto()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Categoria>(
                    "SELECT * FROM Categoria WHERE tipo = 'Gasto' ORDER BY nombre"
                );
            }
        }

        // --- IMPLEMENTACIÓN 2 (Para llamar al SP) ---
        public async Task<PresupuestoViewModel> VerificarPresupuesto(int id_usuario, int mes, int anio, int id_categoria)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parametros = new
                {
                    id_usuario,
                    mes,
                    anio,
                    id_categoria
                };

                return await connection.QueryFirstOrDefaultAsync<PresupuestoViewModel>(
                    "VerificarPresupuestoCategoria",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        // --- IMPLEMENTACIÓN 3 (Para llamar al SP del Reporte) ---
        public async Task<IEnumerable<ReporteMensualViewModel>> ObtenerReporteMensual(int id_usuario, int mes, int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parametros = new
                {
                    id_usuario,
                    mes,
                    anio
                };

                return await connection.QueryAsync<ReporteMensualViewModel>(
                    "GenerarReporteMensual",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        // --- MÉTODO IMPLEMENTADO CON CORRECCIÓN FINAL ---
        public async Task<IEnumerable<GastoPorCategoriaViewModel>> ObtenerGastosPorCategoria(int idUsuarioPrueba)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Esta consulta suma los gastos por categoría de los últimos 30 días
                // para el usuario especificado.
                var query = @"SELECT 
                                c.nombre AS Categoria, 
                                SUM(g.monto) AS Monto
                            FROM 
                                Gastos g 
                            INNER JOIN 
                                Categoria c ON g.id_categoria = c.id_categoria
                            WHERE 
                                g.id_usuario = @idUsuarioPrueba
                                AND g.fechaGasto >= DATEADD(day, -30, GETDATE()) -- CORRECCIÓN APLICADA AQUÍ
                            GROUP BY 
                                c.nombre";

                return await connection.QueryAsync<GastoPorCategoriaViewModel>(query, new { idUsuarioPrueba });
            }
        }
    }
}