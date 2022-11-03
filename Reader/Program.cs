using System.Text.Json;
using Shared;

const string connString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";
const string server = "localhost:9092";
const string group = "reader";
const string mailsTopic = "mails";
const string clicksTopic = "clicks";

var db = new DataBase(connString);
var users = db.GetUsers().ToArray();

var redPanda = new RedPanda(server, group);
redPanda.Subscribe(mailsTopic);

while (true)
{
    var mailJson = redPanda.Consume().Message.Value;
    var mail = JsonSerializer.Deserialize<Mail>(mailJson);
    if (mail is null)
        throw new ArgumentNullException(nameof(mail), "Mail is null");
    foreach (var user in users)
        if (mail.themes.Intersect(user.Item2).Any())
            redPanda.Produce(clicksTopic, new Click
            {
                username = user.Item1,
                mailThemes = mail.themes
            });
}