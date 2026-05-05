// Ubicación: Api_Eden/Services/TratamientoService/MedicoService.cs

using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Services.TratamientoService
{
    public class MedicoService : IMedicoService
    {
        private readonly AppDbContext _db;

        public MedicoService(AppDbContext db) => _db = db;

        // ── Historial médico ──────────────────────────────────────────────────
        public async Task<IEnumerable<object>> GetHistorialAsync(int animalId)
        {
            // Devuelve [] si no hay historial — nunca 404
            return await _db.Historialmedicos
                .Where(h => h.AnimalId == animalId)
                .Include(h => h.Tratamientos)
                    .ThenInclude(t => t.Medicamento)
                .OrderByDescending(h => h.Fecha)
                .Select(h => (object)new
                {
                    h.Id,
                    h.AnimalId,
                    h.Fecha,
                    h.Diagnostico,
                    h.Sintomas,
                    h.Peso,
                    h.Temperatura,
                    h.Observaciones,
                    Tratamientos = h.Tratamientos.Select(t => new
                    {
                        t.Id,
                        Medicamento = t.Medicamento.Nombre,
                        t.Dosis,
                        t.Frecuencia,
                        t.ViaAdministracion,
                        t.Estado,
                        t.FechaInicio,
                        t.FechaFin
                    })
                })
                .ToListAsync();
        }

        public async Task<(bool ok, string mensaje, int? id)> RegistrarHistorialAsync(
            RegistrarHistorialDto dto)
        {
            var animal = await _db.Animales.FindAsync(dto.AnimalId);
            if (animal is null)
                return (false, "Animal no encontrado.", null);

            var responsable = await _db.Usuarios.FindAsync(dto.VeterinarioId);
            if (responsable is null ||
                (responsable.Rol != "Veterinario" && responsable.Rol != "Administrador"))
                return (false, "El usuario responsable no existe o no tiene permisos.", null);

            var historial = new Historialmedico
            {
                AnimalId = dto.AnimalId,
                Diagnostico = dto.Diagnostico,
                Sintomas = dto.Sintomas,
                Peso = dto.Peso,
                Temperatura = dto.Temperatura,
                VeterinarioId = dto.VeterinarioId,
                Observaciones = dto.Observaciones,
                Fecha = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow,
            };

            _db.Historialmedicos.Add(historial);
            await _db.SaveChangesAsync();
            return (true, "Historial registrado correctamente.", historial.Id);
        }

        // ── Tratamientos ──────────────────────────────────────────────────────
        public async Task<IEnumerable<object>> GetTratamientosAsync()
        {
            return await _db.Tratamientos
                .Include(t => t.Medicamento)
                .Include(t => t.HistorialMedico)
                    .ThenInclude(h => h.Animal)
                        .ThenInclude(a => a.Especie)
                .OrderByDescending(t => t.FechaInicio)
                .Select(t => (object)new
                {
                    t.Id,
                    AnimalId = t.HistorialMedico.AnimalId,
                    Animal = t.HistorialMedico.Animal.Nombre,
                    FotografiaUrl = t.HistorialMedico.Animal.FotografiaUrl,
                    Especie = t.HistorialMedico.Animal.Especie != null
                                          ? t.HistorialMedico.Animal.Especie.Nombre : null,
                    Diagnostico = t.HistorialMedico.Diagnostico,
                    Medicamento = t.Medicamento.Nombre,
                    t.Dosis,
                    t.Frecuencia,
                    t.ViaAdministracion,
                    t.Estado,
                    FechaInicio = t.FechaInicio.ToString("yyyy-MM-dd"),
                    FechaFin = t.FechaFin != null
                                          ? t.FechaFin.Value.ToString("yyyy-MM-dd") : null,
                    HistorialMedicoId = t.HistorialMedicoId,
                })
                .ToListAsync();
        }

        // Mismo query — ruta /tratamientosall existe por compatibilidad
        public async Task<IEnumerable<object>> GetAllTratamientosAsync() =>
            await GetTratamientosAsync();

        // ── Medicamentos ──────────────────────────────────────────────────────
        public async Task<IEnumerable<object>> GetMedicamentosAsync()
        {
            return await _db.Medicamentos
                .Where(m => m.Activo == true)
                .OrderBy(m => m.Nombre)
                .Select(m => (object)new
                {
                    m.Id,
                    m.Nombre,
                    m.PrincipioActivo,
                    m.Presentacion,
                    m.Concentracion
                })
                .ToListAsync();
        }

        public async Task<(bool ok, object? data)> CrearMedicamentoAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return (false, null);

            var existente = await _db.Medicamentos
                .FirstOrDefaultAsync(m => m.Nombre.ToLower() == nombre.ToLower().Trim());

            if (existente != null)
                return (true, new { id = existente.Id, nombre = existente.Nombre });

            var nuevo = new Medicamento
            {
                Nombre = nombre.Trim(),
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
            };

            _db.Medicamentos.Add(nuevo);
            await _db.SaveChangesAsync();
            return (true, new { id = nuevo.Id, nombre = nuevo.Nombre });
        }

        public async Task<IEnumerable<object>> GetTiposVacunaAsync()
        {
            return await _db.Tiposvacunas
                .Where(t => t.Activa == true)
                .OrderBy(t => t.Nombre)
                .Select(t => (object)new
                  {
                      t.Id,
                      t.Nombre,
                      t.EspecieId,
                      t.Descripcion,
                      t.DuracionMeses,
                      t.Obligatoria
                  })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVeterinariosAsync()
        {
            return await _db.Usuarios
                .Where(u => (u.Rol == "Veterinario" || u.Rol == "Administrador") && u.Activo == true)
                .OrderBy(u => u.Nombre)
                .Select(u => new { u.Id, u.Nombre, u.Apellido, u.Rol })
                .ToListAsync();
        }
    }
    }