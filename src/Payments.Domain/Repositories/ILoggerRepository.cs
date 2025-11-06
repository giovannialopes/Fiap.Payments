using Payments.Domain.Entities;

namespace Payments.Domain.Repositories;

public interface ILoggerRepository : ICommit
{
    Task AddILogger(ILoggerEnt loggerEnt);
}
