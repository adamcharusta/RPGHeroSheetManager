using RabbitMQ.Client;

namespace RPGHeroSheetManagerAPI.Infrastructure.RabbitMq.Common;

public abstract class BaseMessage
{
    public virtual string QueueName => GetType().Name;
    public virtual bool Durable => false;
    public virtual bool Exclusive => false;
    public virtual bool AutoDelete => false;
    public virtual IDictionary<string, object?>? Arguments { get; } = null;
    public string Exchange { get; set; } = string.Empty;
    public virtual bool Mandatory => true;
    public BasicProperties BasicProperties { get; set; } = new();
    public byte[] Body { get; set; } = null!;
}
