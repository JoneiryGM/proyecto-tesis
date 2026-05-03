using System.ComponentModel.DataAnnotations;

namespace Api_Eden.DTOs.AnimalCreadoDto
{
    public record AnimalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Especie { get; set; }       // nombre de la especie
        public int? EspecieId { get; set; }
        public string? Raza { get; set; }
        public int? Edad { get; set; }
        public string? FechaIngreso { get; set; }  // ISO string para el frontend
        public string? Sexo { get; set; }
        public string? ZonaActual { get; set; }    // nombre de la zona
        public int? ZonaActualId { get; set; }
        public string? Color { get; set; }
        public string? FotografiaUrl { get; set; }
        public string? Observaciones { get; set; }
        public string? EstadoSalud { get; set; }
        public string? EstadoGeneral { get; set; }
    }
}
