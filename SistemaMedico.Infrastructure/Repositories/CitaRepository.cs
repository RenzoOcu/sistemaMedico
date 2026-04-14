using Microsoft.EntityFrameworkCore;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Infrastructure.Data;

namespace SistemaMedico.Infrastructure.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly ApplicationDbContext _context;

    public CitaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cita>> GetAllAsync()
    {
        return await _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.Hora)
            .ToListAsync();
    }

    public async Task<Cita?> GetByIdAsync(int id)
    {
        return await _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.IdCita == id);
    }

    public async Task<IEnumerable<Cita>> GetByMedicoAsync(int idMedico, DateTime? fecha = null, string? estado = null)
    {
        var query = _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .Where(c => c.IdMedico == idMedico);

        if (fecha.HasValue)
        {
            query = query.Where(c => c.Fecha.Date == fecha.Value.Date);
        }

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(c => c.Estado == estado);
        }

        return await query
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.Hora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cita>> GetByPacienteAsync(int idPaciente)
    {
        return await _context.Citas
            .Include(c => c.Medico)
            .Where(c => c.IdPaciente == idPaciente)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.Hora)
            .ToListAsync();
    }

    public async Task<bool> ExisteHorario(int idMedico, DateTime fecha, TimeSpan hora, int? idCitaExcluir = null)
    {
        var query = _context.Citas
            .Where(c => c.IdMedico == idMedico 
                     && c.Fecha.Date == fecha.Date 
                     && c.Hora == hora
                     && c.Estado != "Cancelada");

        if (idCitaExcluir.HasValue)
        {
            query = query.Where(c => c.IdCita != idCitaExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Cita cita)
    {
        await _context.Citas.AddAsync(cita);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cita cita)
    {
        _context.Citas.Update(cita);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cita = await _context.Citas.FindAsync(id);
        if (cita != null)
        {
            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
        }
    }
}