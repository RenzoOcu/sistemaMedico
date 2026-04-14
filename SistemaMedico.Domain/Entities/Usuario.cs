namespace SistemaMedico.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? IdMedico { get; set; }
    public bool Estado { get; set; } = true;

    public virtual Medico? Medico { get; set; }
}