using MercuryOMS.Application.IRepository;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using static System.Formats.Asn1.AsnWriter;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class DeductInventoryConsumer : RabbitMqConsumerBase<PaymentPaidMessage>
    {
        public DeductInventoryConsumer(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string QueueName => QueueNames.PaymentPaid;

        protected override async Task HandleMessageAsync(IServiceScope serviceScope, PaymentPaidMessage message, CancellationToken cancellationToken)
        {
            var uow = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var inventoryRepository = serviceScope.ServiceProvider.GetRequiredService<IInventoryRepository>();
            var order = await uow
                .GetRepository<Order>()
                .GetByIdAsync(message.OrderId, default);

            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng: {message.OrderId}");

            foreach (var item in order.Items)
            {
                var inventory = await inventoryRepository
                    .GetByVariantIdAsync(item.ProductVariantId);

                if (inventory == null)
                    throw new Exception(
                        $"Không tìm thấy tồn kho cho biến thể: {item.ProductVariantId}");

                inventory.Commit(item.Quantity, message.OrderId);
            }

            await uow.SaveChangesAsync();
        }
    }
}