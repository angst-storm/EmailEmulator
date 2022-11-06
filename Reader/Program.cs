using System.Text.Json;
using Shared;

const string connString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";
const string server = "localhost:9092";
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
    if (mail is null)
        throw new ArgumentNullException(nameof(mail), "Mail is null");
    for (var i = 0; i < users.Length; i++)
    {
        if (mail.themes.Intersect(users[i]).Any() && random.Next(5) == 0)
            redPanda.Produce(clicksTopic, new Click
            {
                userId = i + 1,
                mailThemes = mail.themes
            });
    }
}