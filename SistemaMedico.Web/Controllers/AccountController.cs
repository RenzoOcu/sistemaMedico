using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaMedico.Application.Interfaces;

namespace SistemaMedico.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var (success, message, usuario) = await _authService.LoginAsync(username, password);

        if (!success)
        {
            ViewBag.Error = message;
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim("UserId", usuario!.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        if (usuario.IdMedico.HasValue)
        {
            claims.Add(new Claim("MedicoId", usuario.IdMedico.Value.ToString()));
        }

        var claimsIdentity = new ClaimsIdentity(
            claims, "CookieAuth");

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await HttpContext.SignInAsync(
            "CookieAuth",
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (usuario.Rol == "Recepcionista")
        {
            return RedirectToAction("Index", "Home");
        }
        
        return RedirectToAction("MisCitas", "Citas");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}