using Confluent.Kafka;

namespace Mailer;

public class Kafka
{
    private readonly IConsumer<Ignore, string> consumer;
    private readonly IProducer<Null, string> producer;

    public Kafka(string server)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = server,
            GroupId = "group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = server
        };

        consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe("commands");

        producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public ConsumeResult<Ignore, string> ConsumeCommand()
    {
        return consumer.Consume();
    }

    public Task<DeliveryResult<Null, string>> ProduceEmail(string username, string message)
    {
        return producer.ProduceAsync("emails", new Message<Null, string>
        {
            Value = $"user: {username}, message: {message}"
        });
    }
}