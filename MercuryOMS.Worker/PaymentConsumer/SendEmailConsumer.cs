using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class SendEmailConsumer : RabbitMqConsumerBase<PaymentPaidMessage>
    {
        private readonly IConfiguration _config;

        public SendEmailConsumer(IServiceScopeFactory serviceScopeFactory, 
                                IConfiguration config) : base(serviceScopeFactory)
        {
            _config = config;
        }

        protected override string QueueName => QueueNames.PaymentPaid;

        protected override async Task HandleMessageAsync(IServiceScope serviceScope, PaymentPaidMessage message, CancellationToken cancellationToken)
        {
            var notificationService = serviceScope.ServiceProvider.GetRequiredService<INotificationService>();

            await notificationService.SendEmailAsync(
                message.Email,
                "[PAYMENT SUCCESSFUL] - MercuryOMS",
                EmailTemplates.PaymentSuccess(
                    message.FullName,
                    message.OrderId.ToString(),
                    message.Amount.ToString("N0"),
                    $"{_config["App:FrontendUrl"]}/orders/{message.OrderId}"
                )
            );
        }
    }
}
