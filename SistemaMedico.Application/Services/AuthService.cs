using System.Security.Cryptography;
using System.Text;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<(bool Success, string Message, Usuario? User)> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "El nombre de usuario es obligatorio.", null);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "La contraseña es obligatoria.", null);
        }

        var usuario = await _usuarioRepository.GetByUsernameAsync(username);
        if (usuario == null)
        {
            return (false, "Usuario o contraseña incorrectos.", null);
        }

        var parts = usuario.Password.Split('.');
        if (parts.Length != 2)
        {
            return (false, "Usuario o contraseña incorrectos.", null);
        }

        var salt = parts[0];
        var hash = parts[1];

        if (!VerifyPassword(password, hash, salt))
        {
            return (false, "Usuario o contraseña incorrectos.", null);
        }

        return (true, "Login exitoso.", usuario);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPasswordInternal(password, salt);
        return computedHash == hash;
    }

    public string HashPassword(string password, out string salt)
    {
        salt = GenerateSalt();
        var hash = HashPasswordInternal(password, salt);
        return $"{salt}.{hash}";
    }

    private static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPasswordInternal(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            saltBytes,
            100000,
            HashAlgorithmName.SHA256);
        
        var hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }
}