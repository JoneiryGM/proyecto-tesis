using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

namespace Api_Eden.Services.TratamientoService
{
    public class TratamientoService : ITratamientoService
    {
        private readonly AppDbContext _db;

        public TratamientoService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool ok, string mensaje, int? id)> RegistrarTratamiento(RegistrarTratamientoDto dto)
        {
            // 🔍 Validar historial
            var historial = await _db.Historialmedicos.FindAsync(dto.HistorialMedicoId);
            if (historial is null)
                return (false, "Historial médico no encontrado.", null);

            // 🔍 Validar medicamento
            var medicamento = await _db.Medicamentos.FindAsync(dto.MedicamentoId);
            if (medicamento is null)
                return (false, "Medicamento no encontrado.", null);

            // 🔥 Obtener animal DESDE el historial (SIN usar dto)
            var animal = await _db.Animales.FindAsync(historial.AnimalId);
            if (animal is null)
                return (false, "Animal no encontrado.", null);

            // 🧠 Validación de fechas
            if (dto.FechaFin < dto.FechaInicio)
                return (false, "La fecha fin no puede ser menor que la fecha inicio.", null);

            // 🔥 Estado automático del tratamiento
            var estado = dto.FechaFin < DateTime.UtcNow ? "Finalizado" : "Activo";

            // 🧠 Crear tratamiento (SIN AnimalId)
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
                Estado = estado,
                FechaCreacion = DateTime.UtcNow
            };

            _db.Tratamientos.Add(tratamiento);

            // 🔥 Lógica de negocio
            animal.EstadoSalud = "EnTratamiento";

            await _db.SaveChangesAsync();

            return (true, "Tratamiento registrado correctamente.", tratamiento.Id);
        }
    public async Task<(bool ok, string mensaje)> ActualizarEstadoTratamiento(int id, string estado, int veterinarioId)
        {
            // 🔍 Validar estados
            var estadosValidos = new[] { "Activo", "Completado", "Suspendido" };
            if (!estadosValidos.Contains(estado))
                return (false, "Estado inválido. Usa: Activo, Completado o Suspendido.");

            // 🔍 Buscar tratamiento
            var tratamiento = await _db.Tratamientos.FindAsync(id);
            if (tratamiento is null)
                return (false, "Tratamiento no encontrado.");

            // 🔥 Actualizar estado
            tratamiento.Estado = estado;

            // 🔥 Si se completa → actualizar animal
            if (estado == "Completado")
            {
                // 🔍 Buscar historial
                var historial = await _db.Historialmedicos.FindAsync(tratamiento.HistorialMedicoId);
                if (historial is null)
                    return (false, "Historial médico no encontrado.");

                // 🔍 Buscar animal
                var animal = await _db.Animales.FindAsync(historial.AnimalId);
                if (animal is null)
                    return (false, "Animal no encontrado.");

                // 🔥 Lógica de negocio
                var tieneActivos = await _db.Tratamientos
                    .AnyAsync(t => t.HistorialMedicoId == historial.Id && t.Estado == "Activo");

                if (!tieneActivos)
                {
                    animal.EstadoSalud = "Recuperado";
                }
            }

            await _db.SaveChangesAsync();

            return (true, $"Tratamiento actualizado a {estado}.");
        } } 
    }
