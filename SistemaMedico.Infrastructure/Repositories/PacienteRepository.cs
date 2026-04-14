using Microsoft.EntityFrameworkCore;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Infrastructure.Data;

namespace SistemaMedico.Infrastructure.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly ApplicationDbContext _context;

    public PacienteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Paciente>> GetAllAsync()
    {
        return await _context.Pacientes
            .Where(p => p.Estado)
            .OrderBy(p => p.Apellidos)
            .ThenBy(p => p.Nombres)
            .ToListAsync();
    }

    public async Task<Paciente?> GetByIdAsync(int id)
    {
        return await _context.Pacientes.FindAsync(id);
    }

    public async Task<Paciente?> GetByDocumentoAsync(string numeroDocumento)
    {
        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.NumeroDocumento == numeroDocumento);
    }

    public async Task AddAsync(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Paciente paciente)
    {
        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente != null)
        {
            paciente.Estado = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Pacientes.AnyAsync(p => p.IdPaciente == id && p.Estado);
    }

    public async Task<bool> HasCitasActivasAsync(int id)
    {
        return await _context.Citas.AnyAsync(c => c.IdPaciente == id && c.Estado == "Programada");
    }
}