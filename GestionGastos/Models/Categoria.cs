using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionGastos.Models
{
    public class Categoria
    {
        public int id_categoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre de la Categoría")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [Display(Name = "Tipo de Categoría")]
        public string tipo { get; set; } // "Ingreso" o "Gasto"

        [StringLength(150, ErrorMessage = "La descripción no puede tener más de 150 caracteres")]
        [Display(Name = "Descripción")]
        public string? descripcion { get; set; }

        [Display(Name = "Imagen")]
        public string? imagen_url { get; set; }

        // Propiedad para subir la imagen (no se mapea a la BD)
        [ValidateNever]
        [Display(Name = "Subir Imagen")]
        public IFormFile? ImagenArchivo { get; set; }
    }
}