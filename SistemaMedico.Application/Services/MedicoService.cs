using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicoRepository;

    public MedicoService(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<IEnumerable<Medico>> GetAllAsync()
    {
        return await _medicoRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Medico>> GetActivosAsync()
    {
        return await _medicoRepository.GetActivosAsync();
    }

    public async Task<Medico?> GetByIdAsync(int id)
    {
        return await _medicoRepository.GetByIdAsync(id);
    }
}