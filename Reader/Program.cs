using System.Text.Json;
using Confluent.Kafka;
using Reader;

var db = new DataBase("Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;");

const string server = "redpanda:9092";
const string mails = "mails";
const string clicks = "clicks";

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

var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
consumer.Subscribe(mails);

var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

var random = new Random();
for (var i = 0; i < 50; i++)
    db.AddUser($"user{i + 1}", new[] { random.Next(1, 51), random.Next(1, 51), random.Next(1, 51) });
var users = db.GetUsers().ToArray();

while (true)
{
    var cr = consumer.Consume();
    var mail = JsonSerializer.Deserialize<Mail>(cr.Message.Value);
    foreach (var user in users)
    {
        if (mail.themes.Intersect(user.Item2).Any())
        {
            producer.ProduceAsync(clicks, new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(new
                {
                    username = user.Item1,
                    mailThemes = mail.themes
                })
            });
        }
    }
}

class Mail
{
    public int[] themes { get; set; }
}