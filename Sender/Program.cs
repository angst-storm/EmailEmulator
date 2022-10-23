using Shared;

const string server = "redpanda:9092";
const string topic = "mails";

var redPanda = new RedPanda(server);

var themes = Enumerable.Range(1, 50).ToArray();
var random = new Random();
var tasks = new List<Task>();

for (var i = 0; i < 1000; i++)
{
    tasks.Add(redPanda.Produce(topic, new Mail
    {
        themes = themes.Where(t => random.Next(10) == 0).ToArray()
    }));
}

foreach (var task in tasks)
{
    await task;
}