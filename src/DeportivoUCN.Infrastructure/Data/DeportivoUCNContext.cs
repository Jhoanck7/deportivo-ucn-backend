using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Data;

public class DeportivoUCNContext(DbContextOptions<DeportivoUCNContext> options) : DbContext(options)
{
    public DbSet<SportBranch> SportBranches { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación 1 a N entre Coach y SportBranch
        modelBuilder.Entity<SportBranch>()
            .HasOne(sb => sb.Coach)
            .WithMany(c => c.SportBranches)
            .HasForeignKey(sb => sb.CoachId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación 1 a N entre SportBranch y Athlete
        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.SportBranch)
            .WithMany(sb => sb.Athletes)
            .HasForeignKey(a => a.SportBranchId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configuración de User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Rut)
            .IsUnique();

        // Configuración de Court
        modelBuilder.Entity<Court>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // Relación 1 a N entre User y Booking
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1 a N entre Court y Booking
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Court)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CourtId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexar Date y CourtId para optimizar búsquedas de disponibilidad
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.CourtId, b.Date });
    }
}