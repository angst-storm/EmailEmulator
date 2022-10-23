using System.Text.Json;
using Shared;

const string connString = "Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";
const string server = "redpanda:9092";
const string mails = "mails";
const string clicks = "clicks";

var db = new DataBase(connString);
var redPanda = new RedPanda(server);

var random = new Random();

redPanda.Subscribe(mails);
db.InitializeTables();
for (var i = 0; i < 50; i++)
    db.AddUser($"user{i + 1}", new[] { random.Next(1, 51), random.Next(1, 51), random.Next(1, 51) });
var users = db.GetUsers().ToArray();

while (true)
{
    var cr = redPanda.Consume();
    var mail = JsonSerializer.Deserialize<Mail>(cr.Message.Value);
    if (mail is null)
        throw new Exception();
    foreach (var user in users)
    {
        if (mail.themes.Intersect(user.Item2).Any())
        {
            redPanda.Produce(clicks, new Click
            {
                username = user.Item1,
                mailThemes = mail.themes
            });
        }
    }
}