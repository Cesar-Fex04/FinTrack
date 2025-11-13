using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // <-- ¡ESTE ES VITAL!

namespace GestionGastos.Models
{
    public class GastoCreacionViewModel
    {
        // --- Campos que SÍ se validan ---

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser positivo")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una categoría")]
        [Display(Name = "Categoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una categoría")]
        public int id_categoria { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un tipo")]
        [Display(Name = "Tipo de Movimiento")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un tipo")]
        public int id_tipoMovimiento { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        [Display(Name = "Método de Pago")]
        public string MetodoPago { get; set; }

        // --- Listas que NO se validan ---

        [ValidateNever] // <-- Ignora la validación de esta lista
        public IEnumerable<SelectListItem> Categorias { get; set; }

        [ValidateNever] // <-- Ignora la validación de esta lista
        public IEnumerable<SelectListItem> TiposMovimiento { get; set; }

    }
}