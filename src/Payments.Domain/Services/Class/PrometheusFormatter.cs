using Payments.Domain.Services.Interface;

namespace Payments.Domain.Services.Class;

/// <summary>
/// Formatador de métricas Prometheus (SRP - Single Responsibility: apenas formata)
/// </summary>
public class PrometheusFormatter : IPrometheusFormatter
{
    public string FormatMetrics(IMetricsService metricsService)
    {
        var metrics = new System.Text.StringBuilder();
        
        // DRY - Método auxiliar para evitar duplicação
        AppendCounter(metrics, "payments_processed_total", "Total payments processed", metricsService.GetPaymentsProcessed());
        AppendCounter(metrics, "payments_completed_total", "Total payments completed", metricsService.GetPaymentsCompleted());
        AppendCounter(metrics, "payments_failed_total", "Total payments failed", metricsService.GetPaymentsFailed());
        AppendCounter(metrics, "payments_queried_total", "Total payments queried", metricsService.GetPaymentsQueried());
        
        // Métricas do sistema
        AppendCounter(metrics, "process_cpu_seconds_total", "Total CPU time used", metricsService.GetCpuUsage());
        AppendGauge(metrics, "process_memory_bytes", "Memory usage in bytes", metricsService.GetMemoryUsage());

        return metrics.ToString();
    }

    // DRY - Evita duplicação de código
    private static void AppendCounter(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} counter");
        sb.AppendLine($"{name} {value}");
    }

    private static void AppendGauge(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} gauge");
        sb.AppendLine($"{name} {value}");
    }
}

