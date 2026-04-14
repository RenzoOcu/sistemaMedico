using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Interfaces;

public interface ICitaRepository
{
    Task<IEnumerable<Cita>> GetAllAsync();
    Task<Cita?> GetByIdAsync(int id);
    Task<IEnumerable<Cita>> GetByMedicoAsync(int idMedico, DateTime? fecha = null, string? estado = null);
    Task<IEnumerable<Cita>> GetByPacienteAsync(int idPaciente);
    Task<bool> ExisteHorario(int idMedico, DateTime fecha, TimeSpan hora, int? idCitaExcluir = null);
    Task AddAsync(Cita cita);
    Task UpdateAsync(Cita cita);
    Task DeleteAsync(int id);
}