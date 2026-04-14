namespace SistemaMedico.Domain.Entities;

public class Medico
{
    public int IdMedico { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;

    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
}