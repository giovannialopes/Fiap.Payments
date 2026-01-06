using Payments.Domain.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Payments.Api.Controllers;

/// <summary>
/// Controller para expor métricas e estatísticas do sistema
/// SRP - Responsabilidade única: apenas expor endpoints de métricas
/// </summary>
[Route("api/v1/metrics")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly IPrometheusFormatter _formatter;

    public MetricsController(IMetricsService metricsService, IPrometheusFormatter formatter)
    {
        _metricsService = metricsService;
        _formatter = formatter;
    }

    /// <summary>
    /// Obtém métricas do sistema em formato Prometheus
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetMetrics()
    {
        var metrics = _formatter.FormatMetrics(_metricsService);
        return Content(metrics, "text/plain");
    }

    /// <summary>
    /// Obtém estatísticas do sistema em formato JSON
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            metrics = new
            {
                payments = new
                {
                    processed = _metricsService.GetPaymentsProcessed(),
                    completed = _metricsService.GetPaymentsCompleted(),
                    failed = _metricsService.GetPaymentsFailed(),
                    queried = _metricsService.GetPaymentsQueried()
                },
                system = new
                {
                    cpuUsage = _metricsService.GetCpuUsage(),
                    memoryUsage = _metricsService.GetMemoryUsage(),
                    uptime = _metricsService.GetUptime()
                }
            }
        });
    }
}

