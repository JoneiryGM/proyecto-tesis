using System.ComponentModel.DataAnnotations;

namespace Api_Eden.DTOs
{
    public class CrearAnimalDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = null!;


        [Required(ErrorMessage = "La especie es obligatoria")]
        public int EspecieId { get; set; }


        [StringLength(100)]
        public string? Raza { get; set; }

        [Range(0, 30, ErrorMessage = "La edad debe estar entre 0 y 30")]
        public int? Edad { get; set; }

        public DateOnly? FechaIngreso { get; set; }

        [StringLength(50)]
        public string? Sexo { get; set; }

        public int? ZonaActualId { get; set; }



    }
}
