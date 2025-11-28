using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionGastos.Models
{
    public class Gasto
    {
        // Propiedades principales
        public int id_gasto { get; set; }
        public int id_usuario { get; set; }
        public int id_categoria { get; set; }
        public int id_tipoMovimiento { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime fechaGasto { get; set; }
        public string MetodoPago { get; set; }

        // Propiedad adicional para mostrar nombre de categoría (NO se mapea a BD)
        [ValidateNever]
        public string? NombreCategoria { get; set; }
    }
}