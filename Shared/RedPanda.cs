using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Shared
{
    public class RedPanda
    {
        private readonly IConsumer<Ignore, string> consumer;
        private readonly IProducer<Null, string> producer;

        public RedPanda(string server, string group)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = group,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = server
            };

            consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public void Subscribe(string topic)
        {
            consumer.Subscribe(topic);
        }

        public ConsumeResult<Ignore, string> Consume()
        {
            return consumer.Consume();
        }

        public Task Produce(string topic, object message)
        {
            return producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message)
            }).ContinueWith(a => Console.WriteLine($"produced: {a.Result.Message.Value}"));
        }
    }
}