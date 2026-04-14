using SistemaMedico.Application.DTOs;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;

    public CitaService(
        ICitaRepository citaRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository)
    {
        _citaRepository = citaRepository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository = medicoRepository;
    }

    public async Task<IEnumerable<CitaDto>> GetAllAsync()
    {
        var citas = await _citaRepository.GetAllAsync();
        return MapToDto(citas);
    }

    public async Task<CitaDto?> GetByIdAsync(int id)
    {
        var cita = await _citaRepository.GetByIdAsync(id);
        return cita != null ? MapToDto(cita) : null;
    }

    public async Task<(bool Success, string Message, CitaDto? Cita)> CreateAsync(CitaCreateDto dto)
    {
        var validation = await ValidateCitaAsync(dto);
        if (!validation.IsValid)
        {
            return (false, validation.Message, null);
        }

        var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
        var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);

        var cita = new Cita
        {
            IdPaciente = dto.IdPaciente,
            IdMedico = dto.IdMedico,
            Fecha = dto.Fecha.Date,
            Hora = dto.Hora,
            Motivo = dto.Motivo,
            Observaciones = dto.Observaciones,
            Estado = "Programada",
            FechaCreacion = DateTime.Now
        };

        await _citaRepository.AddAsync(cita);

        var resultDto = new CitaDto
        {
            IdCita = cita.IdCita,
            IdPaciente = dto.IdPaciente,
            IdMedico = dto.IdMedico,
            Fecha = dto.Fecha,
            Hora = dto.Hora,
            Motivo = dto.Motivo,
            Observaciones = dto.Observaciones,
            Estado = "Programada",
            NombrePaciente = paciente != null ? $"{paciente.Apellidos}, {paciente.Nombres}" : "",
            NumeroDocumento = paciente?.NumeroDocumento,
            NombreMedico = medico?.Nombre,
            Especialidad = medico?.Especialidad
        };

        return (true, "Cita registrada exitosamente.", resultDto);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(CitaDto dto)
    {
        var citaExistente = await _citaRepository.GetByIdAsync(dto.IdCita);
        if (citaExistente == null)
        {
            return (false, "La cita no existe.");
        }

        if (citaExistente.Estado == "Atendida" || citaExistente.Estado == "Cancelada")
        {
            return (false, "No se puede modificar una cita que ya ha sido atendida o cancelada.");
        }

        var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);

        var tieneConflicto = await _citaRepository.ExisteHorario(
            dto.IdMedico,
            dto.Fecha,
            dto.Hora,
            dto.IdCita);

        if (tieneConflicto)
        {
            return (false, "El horario seleccionado ya no está disponible.");
        }

        citaExistente.IdPaciente = dto.IdPaciente;
        citaExistente.IdMedico = dto.IdMedico;
        citaExistente.Fecha = dto.Fecha.Date;
        citaExistente.Hora = dto.Hora;
        citaExistente.Motivo = dto.Motivo;
        citaExistente.Observaciones = dto.Observaciones;
        citaExistente.Estado = dto.Estado;

        await _citaRepository.UpdateAsync(citaExistente);
        return (true, "Cita actualizada exitosamente.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var cita = await _citaRepository.GetByIdAsync(id);
        if (cita == null)
        {
            return (false, "La cita no existe.");
        }

        await _citaRepository.DeleteAsync(id);
        return (true, "Cita eliminada exitosamente.");
    }

    public async Task<IEnumerable<CitaDto>> GetByMedicoAsync(int idMedico, CitaFilterDto? filtros = null)
    {
        var citas = await _citaRepository.GetByMedicoAsync(idMedico, filtros?.Fecha, filtros?.Estado);
        return MapToDto(citas);
    }

    public async Task<IEnumerable<CitaDto>> GetByFilterAsync(CitaFilterDto filtros, int? idMedicoLogueado = null)
    {
        var allCitas = await _citaRepository.GetAllAsync();

        var query = allCitas.AsEnumerable();

        if (idMedicoLogueado.HasValue)
        {
            query = query.Where(c => c.IdMedico == idMedicoLogueado.Value);
        }

        if (filtros.Fecha.HasValue)
        {
            query = query.Where(c => c.Fecha.Date == filtros.Fecha.Value.Date);
        }

        if (filtros.FechaInicio.HasValue && filtros.FechaFin.HasValue)
        {
            query = query.Where(c => c.Fecha.Date >= filtros.FechaInicio.Value.Date && c.Fecha.Date <= filtros.FechaFin.Value.Date);
        }

        if (filtros.IdMedico.HasValue)
        {
            query = query.Where(c => c.IdMedico == filtros.IdMedico.Value);
        }

        if (!string.IsNullOrEmpty(filtros.Estado))
        {
            query = query.Where(c => c.Estado == filtros.Estado);
        }

        return MapToDto(query.OrderBy(c => c.Fecha).ThenBy(c => c.Hora));
    }

    public async Task<IEnumerable<TimeSpan>> GetHorariosDisponibles(int idMedico, DateTime fecha)
    {
        var todasLasCitas = await _citaRepository.GetByMedicoAsync(idMedico, fecha, null);
        var citasDelDia = todasLasCitas.Where(c => c.Estado != "Cancelada").Select(c => c.Hora).ToHashSet();

        var horarios = new List<TimeSpan>();
        var horaInicio = new TimeSpan(8, 0, 0);
        var horaFin = new TimeSpan(17, 0, 0);

        for (var hora = horaInicio; hora < horaFin; hora = hora.Add(TimeSpan.FromMinutes(30)))
        {
            if (!citasDelDia.Contains(hora))
            {
                horarios.Add(hora);
            }
        }

        return horarios;
    }

    private async Task<(bool IsValid, string Message)> ValidateCitaAsync(CitaCreateDto dto)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
        if (paciente == null || !paciente.Estado)
        {
            return (false, "El paciente no existe o no está activo.");
        }

        var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
        if (medico == null || !medico.Estado)
        {
            return (false, "El médico no existe o no está activo.");
        }

        if (dto.Fecha.Date < DateTime.Now.Date)
        {
            return (false, "No se puede registrar una cita en una fecha pasada.");
        }

        if (dto.Fecha.Date == DateTime.Now.Date && dto.Hora < DateTime.Now.TimeOfDay)
        {
            return (false, "No se puede registrar una cita en una hora pasada.");
        }

        if (dto.Hora < new TimeSpan(8, 0, 0) || dto.Hora >= new TimeSpan(17, 0, 0))
        {
            return (false, "El horario de atención es de 8:00 a 17:00.");
        }

        if ((dto.Hora.Minutes % 30) != 0)
        {
            return (false, "Los citas se programan cada 30 minutos.");
        }

        var tieneConflicto = await _citaRepository.ExisteHorario(dto.IdMedico, dto.Fecha, dto.Hora);
        if (tieneConflicto)
        {
            return (false, "El horario seleccionado ya está ocupado.");
        }

        if (string.IsNullOrWhiteSpace(dto.Motivo))
        {
            return (false, "El motivo de la cita es obligatorio.");
        }

        return (true, string.Empty);
    }

    private CitaDto MapToDto(Cita cita)
    {
        return new CitaDto
        {
            IdCita = cita.IdCita,
            IdPaciente = cita.IdPaciente,
            IdMedico = cita.IdMedico,
            Fecha = cita.Fecha,
            Hora = cita.Hora,
            Motivo = cita.Motivo,
            Observaciones = cita.Observaciones,
            Estado = cita.Estado,
            NombrePaciente = cita.Paciente != null ? $"{cita.Paciente.Apellidos}, {cita.Paciente.Nombres}" : "",
            NumeroDocumento = cita.Paciente?.NumeroDocumento,
            NombreMedico = cita.Medico?.Nombre,
            Especialidad = cita.Medico?.Especialidad
        };
    }

    private IEnumerable<CitaDto> MapToDto(IEnumerable<Cita> citas)
    {
        return citas.Select(MapToDto);
    }
}