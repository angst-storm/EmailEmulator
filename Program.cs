using EmailEmulator;

var db = new DataBase("Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;");
db.AddUser("test-1");
db.AddUser("test-2");
db.AddUser("test-3");
var kafka = new Kafka("kafka:9092");

while (true)
{
    var cr = kafka.Consume();
    Console.WriteLine($"consumed: {cr.Message.Value}");
    var crSplit = cr.Message.Value.Split(" ", 2);
    switch (crSplit[0])
    {
        case "user":
            db.AddUser(crSplit[1]);
            break;
        case "message":
            var message = crSplit[1];
            db.AddMessage(message);
            foreach (var user in db.GetUsers())
                kafka.ProduceEmail(user, message).ContinueWith(t =>
                    Console.WriteLine($"produced: '{t.Result.Message.Value}'"));
            break;
    }
}