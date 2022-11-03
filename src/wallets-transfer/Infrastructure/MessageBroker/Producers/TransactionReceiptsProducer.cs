using System.Text;
using Infrastructure.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Infrastructure.MessageBroker.Producers;

public class TransactionReceiptsProducer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    public TransactionReceiptsProducer(IConnectionFactory factory)
    {
        _connection = factory.CreateConnection();
        using var channel = _connection.CreateModel();
        channel.QueueDeclare("transactions", durable: true);
    }

    public async Task PublishAsync(TransactionReceiptMessage message)
    {
        var encoded = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        _channel.BasicPublish(routingKey: "transactions", exchange:"", body: encoded);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}