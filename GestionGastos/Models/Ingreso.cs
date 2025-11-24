using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionGastos.Models
{
    public class Ingreso
    {
        public int id_ingreso { get; set; }

        public int id_usuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int? id_categoria { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto")]
        public decimal monto { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede tener más de 150 caracteres")]
        [Display(Name = "Descripción")]
        public string? descripcion { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        public DateTime fechaIngreso { get; set; } = DateTime.Now;

        // Propiedades adicionales para la vista
        [ValidateNever]
        public string? NombreCategoria { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? Categorias { get; set; }
    }
}