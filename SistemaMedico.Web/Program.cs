using Microsoft.EntityFrameworkCore;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Application.Services;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Infrastructure.Data;
using SistemaMedico.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "SistemaMedico.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Recepcionista", policy => policy.RequireRole("Recepcionista"));
    options.AddPolicy("Medico", policy => policy.RequireRole("Medico"));
    options.AddPolicy("RecepcionistaOrMedico", policy => policy.RequireRole("Recepcionista", "Medico"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Medicos.Any())
    {
        context.Medicos.AddRange(
            new Medico { Nombre = "Dra. Ana García", Especialidad = "Medicina General", Estado = true },
            new Medico { Nombre = "Dr. Carlos López", Especialidad = "Cardiología", Estado = true },
            new Medico { Nombre = "Dra. María Rodriguez", Especialidad = "Pediatría", Estado = true },
            new Medico { Nombre = "Dr. José Martinez", Especialidad = "Dermatología", Estado = true }
        );
    }

    if (!context.Usuarios.Any())
    {
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        string password1 = authService.HashPassword("recep123", out _);
        string password2 = authService.HashPassword("med123", out _);

        context.Usuarios.AddRange(
            new Usuario 
            { 
                Username = "recepcionista", 
                Password = password1, 
                Rol = "Recepcionista",
                Estado = true 
            },
            new Usuario 
            { 
                Username = "doctor", 
                Password = password2, 
                Rol = "Medico",
                IdMedico = 1,
                Estado = true 
            }
        );
    }

    context.SaveChanges();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();