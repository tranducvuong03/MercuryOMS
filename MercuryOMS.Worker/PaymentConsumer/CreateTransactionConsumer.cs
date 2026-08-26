using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class CreateTransactionConsumer : RabbitMqConsumerBase<PaymentPaidMessage>
    {
        public CreateTransactionConsumer(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string QueueName => QueueNames.PaymentPaid;

        protected override async Task HandleMessageAsync(IServiceScope serviceScope, PaymentPaidMessage message, CancellationToken cancellationToken)
        {
            var uow = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var repo = uow.GetRepository<Transaction>();

            var existed = repo.Query.Any(x => x.PaymentId == message.PaymentId);
            if (existed)
                return;

            var transaction = new Transaction(
                message.PaymentId,
                message.OrderId,
                message.Amount,
                TransactionType.Payment
            );

            await repo.AddAsync(transaction);

            await uow.SaveChangesAsync();
        }
    }
}