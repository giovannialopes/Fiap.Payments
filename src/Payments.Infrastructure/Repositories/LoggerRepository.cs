using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Data;

namespace Payments.Infrastructure.Repositories;

public class LoggerRepository : ILoggerRepository
{
    private readonly DbPayments _dbPayments;

    public LoggerRepository(DbPayments dbPayments) {
        _dbPayments = dbPayments;
    }

    public async Task Commit() => await _dbPayments.SaveChangesAsync();

    public async Task AddILogger(ILoggerEnt loggerEnt) => await _dbPayments.LOGS.AddAsync(loggerEnt);
}

