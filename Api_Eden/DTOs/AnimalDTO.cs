namespace Api_Eden.DTOs
{
    public class AnimalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Raza { get; set; }
        public string? EstadoSalud { get; set; }
        public string? EstadoGeneral { get; set; }
        public string? Zona { get; set; }
    }
}
