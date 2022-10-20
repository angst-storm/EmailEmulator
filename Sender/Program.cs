using System.Text.Json;
using Confluent.Kafka;

const string server = "redpanda:9092";
const string topic = "mails";

var producerConfig = new ProducerConfig
{
    BootstrapServers = server
};
var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

var themes = new int[50];
for (var i = 1; i <= 50; i++)
{
    themes[i - 1] = i;
}

var random = new Random();

var j = 0;
while (true)
{
    if (j < 3)
    {
        producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(new
            {
                themes = themes.Where(t => random.Next(10) == 0).ToArray()
            })
        }).ContinueWith(a => Console.WriteLine($"produced: {a.Result.Message.Value}"));
        j++;
    }
}