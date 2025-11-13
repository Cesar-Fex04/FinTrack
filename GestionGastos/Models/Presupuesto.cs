using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionGastos.Models
{
    public class Presupuesto
    {
        public int id_presupuesto { get; set; }

        public int id_usuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int? id_categoria { get; set; }

        [Required(ErrorMessage = "El monto límite es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto Límite")]
        public decimal montoLimite { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        [Display(Name = "Mes")]
        public int mes { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(2020, 2030, ErrorMessage = "El año debe ser válido")]
        [Display(Name = "Año")]
        public int anio { get; set; }

        // Propiedades adicionales para la vista
        [ValidateNever]
        public string? NombreCategoria { get; set; }

        [ValidateNever]
        public string? NombreMes { get; set; }

        [ValidateNever]
        public decimal? MontoGastado { get; set; }

        [ValidateNever]
        public decimal? PorcentajeConsumido { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? Categorias { get; set; }
    }
}