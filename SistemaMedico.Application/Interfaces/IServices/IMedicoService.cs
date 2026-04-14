using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Interfaces;

public interface IMedicoService
{
    Task<IEnumerable<Medico>> GetAllAsync();
    Task<IEnumerable<Medico>> GetActivosAsync();
    Task<Medico?> GetByIdAsync(int id);
}