namespace Payments.Domain.Services.Interface;

/// <summary>
/// Interface para serviço de métricas (DIP - Dependency Inversion Principle)
/// </summary>
public interface IMetricsService
{
    void IncrementPaymentsProcessed();
    void IncrementPaymentsCompleted();
    void IncrementPaymentsFailed();
    void IncrementPaymentsQueried();
    void RecordPaymentDuration(double seconds);
    
    // Getters para exposição de métricas
    long GetPaymentsProcessed();
    long GetPaymentsCompleted();
    long GetPaymentsFailed();
    long GetPaymentsQueried();
    
    // Métricas do sistema
    double GetCpuUsage();
    long GetMemoryUsage();
    TimeSpan GetUptime();
}

