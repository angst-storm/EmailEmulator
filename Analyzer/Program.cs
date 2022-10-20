using System.Text.Json;
using Analyzer;
using Confluent.Kafka;

const string server = "redpanda:9092";
const string clicks = "clicks";

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = server,
    GroupId = "group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
consumer.Subscribe(clicks);

var db = new DataBase("Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;");
var users = db.GetUsers();
var userStats = users.ToDictionary(u => u.Item1, u => new UserStat(u.Item2));

var i = 0;
while (true)
{
    var cr = consumer.Consume();
    var click = JsonSerializer.Deserialize<Click>(cr.Message.Value);
    userStats[click.username].AddThemes(click.mailThemes);
    i++;
    if (i != 0 && i % 50 == 0)
    {
        Console.WriteLine("---");
        foreach (var kvp in userStats)
        {
            Console.WriteLine($"user {kvp.Key}:");
            Console.WriteLine(kvp.Value);
        }
        Console.WriteLine("---");
    }
}

class Click
{
    public string username { get; set; }
    public int[] mailThemes { get; set; }
}

class UserStat
{
    private readonly int[] clicksOnTheme = new int[50];
    private (int, int, int) themesInDb;

    public UserStat(int[] themes)
    {
        themesInDb = (themes[0], themes[1], themes[2]);
    }

    public void AddThemes(int[] themes)
    {
        foreach (var theme in themes)
            clicksOnTheme[theme - 1]++;
    }

    public override string ToString()
    {
        var result = clicksOnTheme
            .Select((n, i) => (i, n))
            .OrderByDescending(n => n.n)
            .ToArray();
        return $"calculate: [{result[0].i + 1}, {result[1].i + 1}, {result[2].i + 1}]\n" +
               $"on db: [{themesInDb.Item1}, {themesInDb.Item2}, {themesInDb.Item3}]";
    }
}