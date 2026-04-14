using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Interfaces;

public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> GetAllAsync();
    Task<Paciente?> GetByIdAsync(int id);
    Task<Paciente?> GetByDocumentoAsync(string numeroDocumento);
    Task AddAsync(Paciente paciente);
    Task UpdateAsync(Paciente paciente);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> HasCitasActivasAsync(int id);
}