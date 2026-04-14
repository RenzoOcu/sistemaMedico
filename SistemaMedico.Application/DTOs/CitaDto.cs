namespace SistemaMedico.Application.DTOs;

public class CitaDto
{
    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = "Programada";

    public string? NombrePaciente { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? NombreMedico { get; set; }
    public string? Especialidad { get; set; }
}

public class CitaCreateDto
{
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public string? Especialidad { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

public class CitaFilterDto
{
    public DateTime? Fecha { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? IdMedico { get; set; }
    public string? Estado { get; set; }
}