using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class UpdateOrderConsumer : RabbitMqConsumerBase<PaymentPaidMessage>
    {
        protected override string QueueName => QueueNames.PaymentPaid;

        public UpdateOrderConsumer(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override async Task HandleMessageAsync(IServiceScope serviceScope, PaymentPaidMessage message, CancellationToken cancellationToken)
        {
            var uow = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var order = await uow
                .GetRepository<Order>()
                .GetByIdAsync(message.OrderId, default);

            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng: {message.OrderId}");

            order.MarkAsCompleted();

            await uow.SaveChangesAsync();
        }
    }
}