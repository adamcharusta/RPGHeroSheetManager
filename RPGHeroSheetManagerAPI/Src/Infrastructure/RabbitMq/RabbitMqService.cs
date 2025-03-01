using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPGHeroSheetManagerAPI.Infrastructure.RabbitMq.Common;

namespace RPGHeroSheetManagerAPI.Infrastructure.RabbitMq;

public interface IRabbitMqService
{
    Task SendMessageAsync<T>(T message) where T : BaseMessage;
    Task StartListeningAsync<T>(Action<T> handleReceivedMessage) where T : BaseMessage;
}

public class RabbitMqService(RabbitMqSettings options) : IRabbitMqService, IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private readonly ConnectionFactory _factory = new()
    {
        HostName = options.Host!, Port = options.Port, UserName = options.UserName!, Password = options.Password!
    };

    private IChannel? _channel;
    private IConnection? _connection;

    public async ValueTask DisposeAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection is { IsOpen: true })
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SendMessageAsync<T>(T message) where T : BaseMessage
    {
        await EnsureConnectionAsync();

        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        await _channel.QueueDeclareAsync(message.QueueName, message.Durable, message.Exclusive,
            message.AutoDelete, message.Arguments);

        var body = message.Body;

        await _channel.BasicPublishAsync(message.Exchange, message.QueueName, message.Mandatory,
            message.BasicProperties, body);
        Log.Information("[RabbitMQ] Sent: {MessageType} to {QueueName}", typeof(T).Name, message.QueueName);
    }

    public async Task StartListeningAsync<T>(Action<T> handleReceivedMessage) where T : BaseMessage
    {
        await EnsureConnectionAsync();

        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var queueName = Activator.CreateInstance<T>().QueueName;
        await _channel.QueueDeclareAsync(queueName, false, false, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(body);

                if (message != null)
                {
                    Log.Information("[RabbitMQ] Received: {MessageType} from {QueueName}", typeof(T).Name, queueName);
                    handleReceivedMessage(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error processing RabbitMQ message: {Error}", ex.Message);
            }

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(queueName, true, consumer);
        Log.Information("[RabbitMQ] Listening on queue: {QueueName}", queueName);
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return;
        }

        await _connectionLock.WaitAsync();
        try
        {
            if (_connection is not { IsOpen: true })
            {
                _connection = await _factory.CreateConnectionAsync();
                Log.Information("RabbitMQ: Connection established.");
            }

            if (_channel is not { IsOpen: true })
            {
                _channel = await _connection.CreateChannelAsync();
                Log.Information("RabbitMQ: Channel created.");
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }
}
