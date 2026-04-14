using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMedico.Application.DTOs;
using SistemaMedico.Application.Interfaces;

namespace SistemaMedico.Web.Controllers;

[Authorize(Roles = "Recepcionista")]
public class PacientesController : Controller
{
    private readonly IPacienteService _pacienteService;

    public PacientesController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    public async Task<IActionResult> Index()
    {
        var pacientes = await _pacienteService.GetAllAsync();
        return View(pacientes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PacienteDto paciente)
    {
        var (success, message, _) = await _pacienteService.CreateAsync(paciente);

        if (!success)
        {
            ViewBag.Error = message;
            return View(paciente);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var paciente = await _pacienteService.GetByIdAsync(id);
        if (paciente == null)
        {
            return NotFound();
        }
        return View(paciente);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PacienteDto paciente)
    {
        var (success, message) = await _pacienteService.UpdateAsync(paciente);

        if (!success)
        {
            ViewBag.Error = message;
            return View(paciente);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _pacienteService.DeleteAsync(id);

        if (!success)
        {
            TempData["Error"] = message;
        }
        else
        {
            TempData["Success"] = message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var paciente = await _pacienteService.GetByIdAsync(id);
        if (paciente == null)
        {
            return NotFound();
        }
        return View(paciente);
    }
}