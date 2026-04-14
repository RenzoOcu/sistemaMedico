using System.Text.RegularExpressions;
using SistemaMedico.Application.DTOs;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacienteService(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<IEnumerable<PacienteDto>> GetAllAsync()
    {
        var pacientes = await _pacienteRepository.GetAllAsync();
        return MapToDto(pacientes);
    }

    public async Task<PacienteDto?> GetByIdAsync(int id)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(id);
        return paciente != null ? MapToDto(paciente) : null;
    }

    public async Task<(bool Success, string Message, PacienteDto? Paciente)> CreateAsync(PacienteDto dto)
    {
        var validation = await ValidatePacienteAsync(dto);
        if (!validation.IsValid)
        {
            return (false, validation.Message, null);
        }

        var paciente = MapToEntity(dto);
        await _pacienteRepository.AddAsync(paciente);

        dto.IdPaciente = paciente.IdPaciente;
        return (true, "Paciente registrado exitosamente.", dto);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(PacienteDto dto)
    {
        var exists = await _pacienteRepository.ExistsAsync(dto.IdPaciente);
        if (!exists)
        {
            return (false, "El paciente no existe.");
        }

        var validation = await ValidatePacienteAsync(dto, dto.IdPaciente);
        if (!validation.IsValid)
        {
            return (false, validation.Message);
        }

        var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
        if (paciente == null)
        {
            return (false, "El paciente no existe.");
        }

        paciente.TipoDocumento = dto.TipoDocumento;
        paciente.NumeroDocumento = dto.NumeroDocumento;
        paciente.Nombres = dto.Nombres;
        paciente.Apellidos = dto.Apellidos;
        paciente.FechaNacimiento = dto.FechaNacimiento;
        paciente.Sexo = dto.Sexo;
        paciente.Telefono = dto.Telefono;
        paciente.Email = dto.Email;
        paciente.Direccion = dto.Direccion;
        paciente.Observaciones = dto.Observaciones;

        await _pacienteRepository.UpdateAsync(paciente);
        return (true, "Paciente actualizado exitosamente.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var exists = await _pacienteRepository.ExistsAsync(id);
        if (!exists)
        {
            return (false, "El paciente no existe.");
        }

        var hasCitas = await _pacienteRepository.HasCitasActivasAsync(id);
        if (hasCitas)
        {
            return (false, "No se puede eliminar el paciente porque tiene citas programadas.");
        }

        await _pacienteRepository.DeleteAsync(id);
        return (true, "Paciente eliminado exitosamente.");
    }

    private async Task<(bool IsValid, string Message)> ValidatePacienteAsync(PacienteDto dto, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.TipoDocumento))
        {
            return (false, "El tipo de documento es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
        {
            return (false, "El número de documento es obligatorio.");
        }

        if (dto.TipoDocumento == "DNI" && !Regex.IsMatch(dto.NumeroDocumento, @"^\d{8}$"))
        {
            return (false, "El DNI debe tener 8 dígitos.");
        }

        if (dto.TipoDocumento == "Pasaporte" && dto.NumeroDocumento.Length < 6)
        {
            return (false, "El pasaporte debe tener al menos 6 caracteres.");
        }

        var existing = await _pacienteRepository.GetByDocumentoAsync(dto.NumeroDocumento);
        if (existing != null && (!excludeId.HasValue || existing.IdPaciente != excludeId.Value))
        {
            return (false, "Ya existe un paciente con ese número de documento.");
        }

        if (string.IsNullOrWhiteSpace(dto.Nombres))
        {
            return (false, "Los nombres son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            return (false, "Los apellidos son obligatorios.");
        }

        if (dto.FechaNacimiento > DateTime.Now)
        {
            return (false, "La fecha de nacimiento no puede ser futura.");
        }

        if (string.IsNullOrWhiteSpace(dto.Sexo))
        {
            return (false, "El sexo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(dto.Telefono))
        {
            return (false, "El teléfono es obligatorio.");
        }

        if (!Regex.IsMatch(dto.Telefono, @"^[\d\s\-\+\(\)]+$"))
        {
            return (false, "El formato del teléfono es inválido.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return (false, "El correo electrónico es obligatorio.");
        }

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(dto.Email, emailPattern))
        {
            return (false, "El formato del correo electrónico es inválido.");
        }

        return (true, string.Empty);
    }

    private PacienteDto MapToDto(Paciente paciente)
    {
        return new PacienteDto
        {
            IdPaciente = paciente.IdPaciente,
            TipoDocumento = paciente.TipoDocumento,
            NumeroDocumento = paciente.NumeroDocumento,
            Nombres = paciente.Nombres,
            Apellidos = paciente.Apellidos,
            FechaNacimiento = paciente.FechaNacimiento,
            Sexo = paciente.Sexo,
            Telefono = paciente.Telefono,
            Email = paciente.Email,
            Direccion = paciente.Direccion,
            Observaciones = paciente.Observaciones
        };
    }

    private IEnumerable<PacienteDto> MapToDto(IEnumerable<Paciente> pacientes)
    {
        return pacientes.Select(MapToDto);
    }

    private Paciente MapToEntity(PacienteDto dto)
    {
        return new Paciente
        {
            TipoDocumento = dto.TipoDocumento,
            NumeroDocumento = dto.NumeroDocumento,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = dto.Sexo,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion,
            Observaciones = dto.Observaciones,
            Estado = true,
            FechaCreacion = DateTime.Now
        };
    }
}