using System.Diagnostics;
using Payments.Domain.Services.Interface;

namespace Payments.Domain.Services.Class;

/// <summary>
/// Serviço de métricas (SRP - Single Responsibility: apenas gerencia métricas)
/// </summary>
public class MetricsService : IMetricsService
{
    private long _paymentsProcessed = 0;
    private long _paymentsCompleted = 0;
    private long _paymentsFailed = 0;
    private long _paymentsQueried = 0;
    private static readonly Process _process = Process.GetCurrentProcess();

    public void IncrementPaymentsProcessed() => Interlocked.Increment(ref _paymentsProcessed);
    public void IncrementPaymentsCompleted() => Interlocked.Increment(ref _paymentsCompleted);
    public void IncrementPaymentsFailed() => Interlocked.Increment(ref _paymentsFailed);
    public void IncrementPaymentsQueried() => Interlocked.Increment(ref _paymentsQueried);
    public void RecordPaymentDuration(double seconds) { /* YAGNI - não implementado ainda */ }

    public long GetPaymentsProcessed() => _paymentsProcessed;
    public long GetPaymentsCompleted() => _paymentsCompleted;
    public long GetPaymentsFailed() => _paymentsFailed;
    public long GetPaymentsQueried() => _paymentsQueried;

    public double GetCpuUsage() => _process.TotalProcessorTime.TotalSeconds;
    public long GetMemoryUsage() => _process.WorkingSet64;
    public TimeSpan GetUptime() => DateTime.UtcNow - _process.StartTime.ToUniversalTime();
}

