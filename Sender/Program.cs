using System.Text.Json;
using Shared;

const string server = "redpanda:9092";
const string group = "sender";
const string commandsTopic = "commands";
const string mailsTopic = "mails";

var redPanda = new RedPanda(server, group);
redPanda.Subscribe(commandsTopic);

var themes = Enumerable.Range(1, 50).ToArray();
var random = new Random();

while (true)
{
    var command = redPanda.Consume().Message.Value;
    try
    {
        command = JsonSerializer.Deserialize<string>(command);
    }
    catch (JsonException e)
    {
        command = "send 0";
    }
    var parameters = command.Split();
    if (parameters.Length == 2)
        if (parameters[0] == "send")
            for (var i = 0; i < int.Parse(parameters[1]); i++)
                redPanda.Produce(mailsTopic, new Mail
                {
                    themes = themes.Where(_ => random.Next(10) == 0).ToArray()
                });
}