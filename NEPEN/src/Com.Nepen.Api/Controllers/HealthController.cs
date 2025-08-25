using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using Desafio_NEPEN.Com.Nepen.Infra.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Desafio_NEPEN.Com.Nepen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IDatabase _redis;
    private readonly AppDbContext _dbContext;
    private readonly Process _currentProcess;

    public HealthController(IConnectionMultiplexer redis, AppDbContext dbContext)
    {
        _redis = redis.GetDatabase();
        _dbContext = dbContext;
        _currentProcess = Process.GetCurrentProcess();
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "Healthy" });

    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        var response = new Dictionary<string, object>();
        
        try
        {
            await _redis.StringSetAsync("healthcheck", "1", TimeSpan.FromSeconds(1));
            var redisOk = await _redis.StringGetAsync("healthcheck");
            response["redis"] = redisOk == "1" ? "ok" : "failed";
        }
        catch
        {
            response["redis"] = "failed";
        }
        
        try
        {
            var dbOk = await _dbContext.Database.CanConnectAsync();
            response["database"] = dbOk ? "ok" : "failed";
        }
        catch
        {
            response["database"] = "failed";
        }
        
        response["cpuUsagePercent"] = GetCpuUsagePercent();
        response["memoryUsageMB"] = _currentProcess.WorkingSet64 / 1024.0 / 1024.0;
        response["threads"] = _currentProcess.Threads.Count;
        
        bool allHealthy = response.Values.All(v => v?.ToString() == "ok" || v is double || v is int);
        return allHealthy ? Ok(response) : StatusCode(503, response);
    }

    private double GetCpuUsagePercent()
    {
        try
        {
            var cpuTime = _currentProcess.TotalProcessorTime.TotalMilliseconds;
            var uptime = Environment.TickCount64;
            return Math.Round(cpuTime / uptime * 100, 2);
        }
        catch
        {
            return -1;
        }
    }
}
