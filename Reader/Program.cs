using System.Text.Json;
using Shared;

const string connString = "Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";
const string server = "redpanda:9092";
const string group = "reader";
const string mailsTopic = "mails";
const string clicksTopic = "clicks";

var db = new DataBase(connString);
var users = db.GetUsersThemes().ToArray();

var redPanda = new RedPanda(server, group);
redPanda.Subscribe(mailsTopic);

var random = new Random();

while (true)
{
    var mailJson = redPanda.Consume().Message.Value;
    var mail = JsonSerializer.Deserialize<Mail>(mailJson);
    for (var i = 0; i < users.Length; i++)
    {
        var clickProbability = 10 + mail.themes.Intersect(users[i]).Count() * 10;
        if (clickProbability > 90)
            clickProbability = 90;
        if (random.Next(100) < clickProbability)
            redPanda.Produce(clicksTopic, new Click
            {
                userId = i + 1,
                mailThemes = mail.themes
            });
    }
}