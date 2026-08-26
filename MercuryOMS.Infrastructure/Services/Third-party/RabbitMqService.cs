using MercuryOMS.Application.IServices;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MercuryOMS.Infrastructure.Services;

public sealed class RabbitMqService : IMessageBus, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        ReadOnlyMemory<byte> body = message switch
        {
            string str => Encoding.UTF8.GetBytes(str),
            _ => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))
        };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: routingKey,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();

        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}