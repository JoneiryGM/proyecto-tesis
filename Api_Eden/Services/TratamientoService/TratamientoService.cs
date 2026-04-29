using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Services.TratamientoService
{
    public class TratamientoService : ITratamientoService
    {
        private readonly AppDbContext _db;

        public TratamientoService(AppDbContext db) => _db = db;

        public async Task<(bool ok, string mensaje, int? id)> RegistrarTratamiento(RegistrarTratamientoDto dto)
        {
            var historial = await _db.Historialmedicos.FindAsync(dto.HistorialMedicoId);
            if (historial is null)
                return (false, "Historial médico no encontrado.", null);

            var medicamento = await _db.Medicamentos.FindAsync(dto.MedicamentoId);
            if (medicamento is null)
                return (false, "Medicamento no encontrado.", null);

            var animal = await _db.Animales.FindAsync(historial.AnimalId);
            if (animal is null)
                return (false, "Animal no encontrado.", null);

            if (dto.FechaFin < dto.FechaInicio)
                return (false, "La fecha fin no puede ser menor que la fecha inicio.", null);

            var tratamiento = new Tratamiento
            {
                HistorialMedicoId = dto.HistorialMedicoId,
                MedicamentoId = dto.MedicamentoId,
                Dosis = dto.Dosis,
                Frecuencia = dto.Frecuencia,
                ViaAdministracion = dto.ViaAdministracion,
                FechaInicio = DateOnly.FromDateTime(dto.FechaInicio),
                FechaFin = DateOnly.FromDateTime(dto.FechaFin),
                VeterinarioId = dto.VeterinarioId,
                Observaciones = dto.Observaciones,
                Estado = "Activo", // ← siempre Activo al crear
                FechaCreacion = DateTime.UtcNow
            };

            _db.Tratamientos.Add(tratamiento);

            // Lógica de negocio — actualizar estado de salud del animal
            _db.Entry(animal).State = EntityState.Detached;
            var animalFresh = await _db.Animales.FindAsync(historial.AnimalId);
            if (animalFresh is not null)
            {
                animalFresh.EstadoSalud = "EnTratamiento";
                await _db.SaveChangesAsync();
            }


            return (true, "Tratamiento registrado correctamente.", tratamiento.Id);
        }

        public async Task<(bool ok, string mensaje)> ActualizarEstadoTratamiento(int id, string estado, int veterinarioId)
        {
            var estadosValidos = new[] { "Activo", "Completado", "Suspendido" };
            if (!estadosValidos.Contains(estado))
                return (false, "Estado inválido. Usa: Activo, Completado o Suspendido.");

            var tratamiento = await _db.Tratamientos.FindAsync(id);
            if (tratamiento is null)
                return (false, "Tratamiento no encontrado.");

            tratamiento.Estado = estado;

            if (estado == "Completado")
            {
                var historial = await _db.Historialmedicos.FindAsync(tratamiento.HistorialMedicoId);
                if (historial is null)
                    return (false, "Historial médico no encontrado.");

                var animal = await _db.Animales.FindAsync(historial.AnimalId);
                if (animal is null)
                    return (false, "Animal no encontrado.");

                // Solo marca como Recuperado si no tiene otros tratamientos activos
                var tieneActivos = await _db.Tratamientos
                    .AnyAsync(t => t.HistorialMedicoId == historial.Id
                                && t.Id != id
                                && t.Estado == "Activo");

                if (!tieneActivos)
                    animal.EstadoSalud = "Recuperado";
            }

            await _db.SaveChangesAsync();

            return (true, $"Tratamiento actualizado a {estado}.");
        }
    }
}