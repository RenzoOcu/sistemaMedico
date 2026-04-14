using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaMedico.Web.Controllers;

public class HomeController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        ViewBag.Rol = rol;
        return View();
    }
}