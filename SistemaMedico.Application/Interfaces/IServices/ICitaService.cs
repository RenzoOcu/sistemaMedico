using SistemaMedico.Application.DTOs;

namespace SistemaMedico.Application.Interfaces;

public interface ICitaService
{
    Task<IEnumerable<CitaDto>> GetAllAsync();
    Task<CitaDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, CitaDto? Cita)> CreateAsync(CitaCreateDto cita);
    Task<(bool Success, string Message)> UpdateAsync(CitaDto cita);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<IEnumerable<CitaDto>> GetByMedicoAsync(int idMedico, CitaFilterDto? filtros = null);
    Task<IEnumerable<CitaDto>> GetByFilterAsync(CitaFilterDto filtros, int? idMedicoLogueado = null);
    Task<IEnumerable<TimeSpan>> GetHorariosDisponibles(int idMedico, DateTime fecha);
}