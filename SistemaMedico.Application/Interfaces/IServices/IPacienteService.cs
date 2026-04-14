using SistemaMedico.Application.DTOs;

namespace SistemaMedico.Application.Interfaces;

public interface IPacienteService
{
    Task<IEnumerable<PacienteDto>> GetAllAsync();
    Task<PacienteDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, PacienteDto? Paciente)> CreateAsync(PacienteDto paciente);
    Task<(bool Success, string Message)> UpdateAsync(PacienteDto paciente);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}