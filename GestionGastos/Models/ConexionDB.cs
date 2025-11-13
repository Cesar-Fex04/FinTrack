using System;
using Microsoft.Data.SqlClient;

namespace GestionGastos.Models
{
    public class ConexionDB
    {
        private string cadenaConexion = @"Server=localhost,1433;Database=ControlGastos;User Id=sa;Password=Allisonnc14!;TrustServerCertificate=True;";

        private SqlConnection conexion;

        public ConexionDB()
        {
            conexion = new SqlConnection(cadenaConexion);
        }

        public bool ProbarConexion()
        {
            try
            {
                conexion.Open();
                conexion.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al conectar: {ex.Message}");
            }
        }

        public SqlConnection ObtenerConexion()
        {
            return conexion;
        }

        public void Abrir()
        {
            if (conexion.State == System.Data.ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

        public void Cerrar()
        {
            if (conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
            }
        }
    }
}
