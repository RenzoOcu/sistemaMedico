using Microsoft.EntityFrameworkCore;
using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Cita> Citas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente);
            entity.Property(e => e.TipoDocumento).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Sexo).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.IdMedico);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Especialidad).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdCita);
            entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(e => e.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Medico)
                .WithMany(m => m.Citas)
                .HasForeignKey(e => e.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.Medico)
                .WithMany()
                .HasForeignKey(e => e.IdMedico)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}