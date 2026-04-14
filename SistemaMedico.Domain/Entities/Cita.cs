namespace SistemaMedico.Domain.Entities;

public class Cita
{
    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = "Programada";
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public virtual Paciente? Paciente { get; set; }
    public virtual Medico? Medico { get; set; }
}