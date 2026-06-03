using DeportivoUCN.API.Middlewares;
using DeportivoUCN.Application.Interfaces;
using DeportivoUCN.Application.Services;
using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure PostgreSQL Database
builder.Services.AddDbContext<DeportivoUCNContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Dependency Injection (DI) Registration
builder.Services.AddScoped<ISportBranchRepository, SportBranchRepository>();
builder.Services.AddScoped<ISportBranchService, SportBranchService>();
builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<ICoachService, CoachService>();

// 3. Add Controllers and OpenAPI/Swagger configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

// 4. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. Use Custom Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

// 6. Map Controller Endpoints
app.MapControllers();

app.Run();