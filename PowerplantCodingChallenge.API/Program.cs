using PowerplantCodingChallenge.API.Infrastructure.Middleware;
using PowerplantCodingChallenge.Domain.Services;
using PowerplantCodingChallenge.Infrastructure.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddScoped<IProductionPlanCalculationService, ProductionPlanCalculationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler(err => err.UseCustomErrors());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
