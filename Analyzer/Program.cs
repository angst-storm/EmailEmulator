using System.Text.Json;
using Analyzer;
using Shared;

const string connString = "Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";
const string server = "redpanda:9092";
const string clicks = "clicks";

var redPanda = new RedPanda(server);
redPanda.Subscribe(clicks);
var db = new DataBase(connString);
var users = db.GetUsers();
var userStats = users.ToDictionary(u => u.Item1, u => new UserStat(u.Item2));

var i = 0;
while (true)
{
    var cr = redPanda.Consume();
    var click = JsonSerializer.Deserialize<Click>(cr.Message.Value);
    if (click is null)
        throw new Exception();
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