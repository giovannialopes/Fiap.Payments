using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Enum;
using Games.Domain.Queue.Event;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Data;
using System.Threading;

namespace Payments.Infrastructure.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly DbPayments _dbPayments;

    public PaymentsRepository(DbPayments dbPayments) {
        _dbPayments = dbPayments;
    }

    public Task Commit() => _dbPayments.SaveChangesAsync();

    public async Task AtualizarStatus(PaymentCreatedEvent @event, PaymentStatus status) {

        var payment = await _dbPayments.PAGAMENTOS
            .FirstOrDefaultAsync(p => p.PerfilId == @event.PerfilId && p.JogoId == @event.JogoId);

        if (payment == null) {
            payment = new PaymentsEnt {
                PerfilId = @event.PerfilId,
                JogoId = @event.JogoId,
                StatusPedido = status.ToString()
            };

            await _dbPayments.PAGAMENTOS.AddAsync(payment);
        }
        else {
            payment.StatusPedido = status.ToString();
            payment.LastUpdate = DateTime.UtcNow;
        }

        await _dbPayments.SaveChangesAsync();

        await Task.Delay(TimeSpan.FromSeconds(10));
    }

    public async Task<PaymentsEnt> TrazPagamentos(Guid perfilId, Guid jogoId) =>
        await _dbPayments.PAGAMENTOS
        .FirstOrDefaultAsync(p => p.PerfilId == perfilId && p.JogoId == jogoId);

}
