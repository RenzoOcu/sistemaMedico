namespace SistemaMedico.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, Domain.Entities.Usuario? User)> LoginAsync(string username, string password);
    bool VerifyPassword(string password, string hash, string salt);
    string HashPassword(string password, out string salt);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Domain.Entities.Usuario? User { get; set; }
}