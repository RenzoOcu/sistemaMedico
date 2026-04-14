using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMedico.Application.DTOs;
using SistemaMedico.Application.Interfaces;

namespace SistemaMedico.Web.Controllers;

[Authorize]
public class CitasController : Controller
{
    private readonly ICitaService _citaService;
    private readonly IPacienteService _pacienteService;
    private readonly IMedicoService _medicoService;

    public CitasController(
        ICitaService citaService,
        IPacienteService pacienteService,
        IMedicoService medicoService)
    {
        _citaService = citaService;
        _pacienteService = pacienteService;
        _medicoService = medicoService;
    }

    [Authorize(Roles = "Recepcionista")]
    public async Task<IActionResult> Index()
    {
        var citas = await _citaService.GetAllAsync();
        return View(citas);
    }

    [Authorize(Roles = "Recepcionista")]
    public IActionResult Create()
    {
        ViewBag.Pacientes = _pacienteService.GetAllAsync().Result;
        ViewBag.Medicos = _medicoService.GetActivosAsync().Result;
        ViewBag.Especialidades = _medicoService.GetActivosAsync().Result.Select(m => m.Especialidad).Distinct().ToList();
        return View();
    }

    [Authorize(Roles = "Recepcionista")]
    [HttpPost]
    public async Task<IActionResult> Create(CitaCreateDto cita)
    {
        ViewBag.Pacientes = await _pacienteService.GetAllAsync();
        ViewBag.Medicos = await _medicoService.GetActivosAsync();
        ViewBag.Especialidades = (await _medicoService.GetActivosAsync()).Select(m => m.Especialidad).Distinct().ToList();

        var (success, message, _) = await _citaService.CreateAsync(cita);

        if (!success)
        {
            ViewBag.Error = message;
            return View(cita);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Recepcionista")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _citaService.DeleteAsync(id);

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

    [HttpGet]
    public async Task<IActionResult> MisCitas()
    {
        var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (rol != "Medico")
        {
            return RedirectToAction("Index", "Home");
        }

        var medicoIdClaim = User.FindFirst("MedicoId");
        if (medicoIdClaim == null || !int.TryParse(medicoIdClaim.Value, out int medicoId))
        {
            return RedirectToAction("Index", "Home");
        }

        var medico = await _medicoService.GetByIdAsync(medicoId);
        ViewBag.Medico = medico;

        var citas = await _citaService.GetByMedicoAsync(medicoId);
        return View(citas);
    }

    [HttpPost]
    public async Task<IActionResult> MisCitas(CitaFilterDto filtros)
    {
        var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (rol != "Medico")
        {
            return RedirectToAction("Index", "Home");
        }

        var medicoIdClaim = User.FindFirst("MedicoId");
        if (medicoIdClaim == null || !int.TryParse(medicoIdClaim.Value, out int medicoId))
        {
            return RedirectToAction("Index", "Home");
        }

        var medico = await _medicoService.GetByIdAsync(medicoId);
        ViewBag.Medico = medico;
        ViewBag.Filtros = filtros;

        bool tieneFiltro = filtros.Fecha.HasValue || 
                          filtros.FechaInicio.HasValue || 
                          filtros.FechaFin.HasValue || 
                          !string.IsNullOrEmpty(filtros.Estado);

        IEnumerable<CitaDto> citas;

        if (tieneFiltro)
        {
            if (filtros.FechaInicio.HasValue && filtros.FechaFin.HasValue && filtros.FechaInicio > filtros.FechaFin)
            {
                ViewBag.Error = "La fecha de inicio no puede ser mayor que la fecha fin.";
                citas = await _citaService.GetByMedicoAsync(medicoId);
            }
            else
            {
                citas = await _citaService.GetByMedicoAsync(medicoId, filtros);
                
                if (!citas.Any())
                {
                    ViewBag.Advertencia = "No se encontraron citas con los filtros seleccionados.";
                }
            }
        }
        else
        {
            ViewBag.Advertencia = "Debe seleccionar al menos un criterio de búsqueda.";
            citas = await _citaService.GetByMedicoAsync(medicoId);
        }

        return View(citas);
    }

    [Authorize(Roles = "Recepcionista")]
    [HttpGet]
    public async Task<IActionResult> GetHorariosDisponibles(int idMedico, DateTime fecha)
    {
        var horarios = await _citaService.GetHorariosDisponibles(idMedico, fecha);
        return Json(horarios.Select(h => h.ToString(@"hh\:mm")));
    }
}