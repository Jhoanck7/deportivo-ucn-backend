using DeportivoUCN.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.Infrastructure.Data;

public class DeportivoUCNContext(DbContextOptions<DeportivoUCNContext> options) : DbContext(options)
{
    public DbSet<SportBranch> SportBranches { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Athlete> Athletes { get; set; }

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
    }
}