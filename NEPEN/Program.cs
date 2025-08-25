using Desafio_NEPEN.Com.Nepen.Api.Mapping;
using Desafio_NEPEN.Com.Nepen.Api.Validators;
using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Desafio_NEPEN.Com.Nepen.Core.Interfaces;
using Desafio_NEPEN.Com.Nepen.Core.Services;
using Desafio_NEPEN.Com.Nepen.Infra.Persistence.Repositories;
using Serilog;
using Serilog.Formatting.Compact;
using Desafio_NEPEN.Com.Nepen.Core.Middlewares;
using Desafio_NEPEN.Com.Nepen.Infra.Persistence.Contexts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = builder.Configuration["Redis:Connection"];



Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("CorrelationId", Guid.NewGuid()) 
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "/app/logs/log.json", rollingInterval: Serilog.RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddScoped<IMedidorRepository, MedidorRepository>();
builder.Services.AddScoped<ILeituraRepository, LeituraRepository>();
builder.Services.AddScoped<ILeituraService, LeituraService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddValidatorsFromAssemblyContaining<LeituraCreateValidator>();
builder.Services.AddAutoMapper(cfg =>
{ cfg.AddProfile<MappingProfile>();
}, typeof(MappingProfile).Assembly);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuditMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    
    if (!dbContext.Medidores.Any())
    {
        var medidores = Enumerable.Range(1, 10)
            .Select(i => new Medidor()
            {
                MedidorId = $"MED{i:D3}"
            })
            .ToList();

        dbContext.Medidores.AddRange(medidores);
        dbContext.SaveChanges();
    }
}

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Servidor falhou ao iniciar");
}
finally
{
    await Log.CloseAndFlushAsync();
}