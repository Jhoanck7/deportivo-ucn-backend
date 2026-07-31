using DeportivoUCN.Models.Entities;
using DeportivoUCN.Models.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace DeportivoUCN.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(DeportivoUCNContext context)
    {
        // Ensure database is created and migrations are applied
        await context.Database.MigrateAsync();

        // 1. Seed Courts
        if (!await context.Courts.AnyAsync())
        {
            var courts = new List<Court>
            {
                new() { Name = "Cancha 1 - Pasto Sintético", Description = "Fútbol 7 con iluminación LED de alta potencia", Status = CourtStatus.Available, PricePerHour = 25000m },
                new() { Name = "Cancha 2 - Pasto Sintético", Description = "Fútbol 7 con iluminación estándar", Status = CourtStatus.Available, PricePerHour = 20000m },
                new() { Name = "Cancha de Tenis 1", Description = "Superficie de arcilla y cierres perimetrales", Status = CourtStatus.Available, PricePerHour = 15000m }
            };

            await context.Courts.AddRangeAsync(courts);
            await context.SaveChangesAsync();
        }

        // 2. Seed Users (Admin, Coaches, and Test Users)
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new()
                {
                    Rut = "1-9",
                    Email = "admin@deportivoucn.cl",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123"),
                    FirstName = "Administrador",
                    LastName = "General",
                    Phone = "+56911112222",
                    Role = UserRole.Admin
                },
                new()
                {
                    Rut = "2-7",
                    Email = "dt.futbol@deportivoucn.cl",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("CoachPassword123"),
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Phone = "+56933334444",
                    Role = UserRole.Coach
                },
                new()
                {
                    Rut = "3-5",
                    Email = "dt.tenis@deportivoucn.cl",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("CoachPassword123"),
                    FirstName = "María",
                    LastName = "González",
                    Phone = "+56955556666",
                    Role = UserRole.Coach
                },
                new()
                {
                    Rut = "4-3",
                    Email = "usuario@correo.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("UserPassword123"),
                    FirstName = "Jhoan",
                    LastName = "Castro",
                    Phone = "+56977778888",
                    Role = UserRole.User
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        // 3. Seed Coaches (Entrenadores para las Ramas)
        if (!await context.Coaches.AnyAsync())
        {
            var coaches = new List<Coach>
            {
                new() { FirstName = "Juan", LastName = "Pérez", Email = "dt.futbol@deportivoucn.cl" },
                new() { FirstName = "María", LastName = "González", Email = "dt.tenis@deportivoucn.cl" }
            };

            await context.Coaches.AddRangeAsync(coaches);
            await context.SaveChangesAsync();
        }

        // 4. Seed Sport Branches (Ramas Deportivas)
        if (!await context.SportBranches.AnyAsync())
        {
            var coachFutbol = await context.Coaches.FirstOrDefaultAsync(c => c.Email == "dt.futbol@deportivoucn.cl");
            var coachTenis = await context.Coaches.FirstOrDefaultAsync(c => c.Email == "dt.tenis@deportivoucn.cl");

            var branches = new List<SportBranch>
            {
                new()
                {
                    Name = "Fútbol Masculino",
                    TrainingDays = "Lunes, Miércoles",
                    TrainingHours = "18:00 - 20:00",
                    TrainingSector = "Cancha 1 - Pasto Sintético",
                    AthleteLimit = 20,
                    CoachId = coachFutbol?.Id
                },
                new()
                {
                    Name = "Tenis Selección",
                    TrainingDays = "Martes, Jueves",
                    TrainingHours = "16:00 - 18:00",
                    TrainingSector = "Cancha de Tenis 1",
                    AthleteLimit = 8,
                    CoachId = coachTenis?.Id
                }
            };

            await context.SportBranches.AddRangeAsync(branches);
            await context.SaveChangesAsync();
        }

        // 5. Seed Athletes (Deportistas)
        if (!await context.Athletes.AnyAsync())
        {
            var branchFutbol = await context.SportBranches.FirstOrDefaultAsync(b => b.Name == "Fútbol Masculino");
            var branchTenis = await context.SportBranches.FirstOrDefaultAsync(b => b.Name == "Tenis Selección");

            var athletes = new List<Athlete>
            {
                new()
                {
                    FirstName = "Alexis",
                    LastName = "Sánchez",
                    Rut = "15.999.888-7",
                    Email = "alexis@futbol.cl",
                    Phone = "+56999999991",
                    BirthDate = new DateTime(1998, 12, 19, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    SportBranchId = branchFutbol?.Id
                },
                new()
                {
                    FirstName = "Arturo",
                    LastName = "Vidal",
                    Rut = "16.888.777-6",
                    Email = "arturo@futbol.cl",
                    Phone = "+56999999992",
                    BirthDate = new DateTime(1997, 5, 22, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    SportBranchId = branchFutbol?.Id
                },
                new()
                {
                    FirstName = "Nicolás",
                    LastName = "Jarry",
                    Rut = "19.777.666-5",
                    Email = "nico@tenis.cl",
                    Phone = "+56999999993",
                    BirthDate = new DateTime(2000, 10, 11, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    SportBranchId = branchTenis?.Id
                },
                new()
                {
                    FirstName = "Alejandro",
                    LastName = "Tabilo",
                    Rut = "18.666.555-4",
                    Email = "tabilo@tenis.cl",
                    Phone = "+56999999994",
                    BirthDate = new DateTime(2001, 6, 2, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    SportBranchId = branchTenis?.Id
                }
            };

            await context.Athletes.AddRangeAsync(athletes);
            await context.SaveChangesAsync();
        }
    }
}
