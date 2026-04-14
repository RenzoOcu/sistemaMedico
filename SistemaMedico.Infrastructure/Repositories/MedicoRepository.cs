using Microsoft.EntityFrameworkCore;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Infrastructure.Data;

namespace SistemaMedico.Infrastructure.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly ApplicationDbContext _context;

    public MedicoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Medico>> GetAllAsync()
    {
        return await _context.Medicos
            .Where(m => m.Estado)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Medico>> GetActivosAsync()
    {
        return await _context.Medicos
            .Where(m => m.Estado)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Medico?> GetByIdAsync(int id)
    {
        return await _context.Medicos.FindAsync(id);
    }
}